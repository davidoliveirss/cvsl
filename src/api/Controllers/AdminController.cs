using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Context;
using BCrypt.Net;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public AdminController(ClinicaDbContext context)
    {
        _context = context;
    }


    [HttpGet("clinicas")]
    public async Task<IActionResult> GetAllClinicas([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool incluirInativos = false)
    {
        var skip = (page - 1) * pageSize;
        
        var query = _context.Clinicas.AsQueryable();
        
        if (!incluirInativos)
        {
            query = query.Where(c => c.Ativo);
        }
        
        var clinicas = await query
            .OrderBy(c => c.Nome)
            .Skip(skip)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Nome,
                c.Email,
                c.Cp,
                c.Nif,
                c.Iban,
                c.Ativo,
                NumeroFuncionarios = _context.Funcionarios.Count(f => f.ClinicaId == c.Id)
            })
            .ToListAsync();

        var total = await query.CountAsync();

        return Ok(new
        {
            data = clinicas,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet("clinicas/{id}")]
    public async Task<IActionResult> GetClinica(int id)
    {
        var clinica = await _context.Clinicas
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Nome,
                c.Email,
                c.Cp,
                c.Nif,
                c.Iban,
                c.Ativo,
                Funcionarios = _context.Funcionarios
                    .Where(f => f.ClinicaId == c.Id)
                    .Select(f => new { f.Id, f.Nome, f.Email, f.Especialidade, f.Ativo })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (clinica == null)
            return NotFound(new { message = "Clínica não encontrada" });

        return Ok(clinica);
    }

    [HttpPost("clinicas")]
    public async Task<IActionResult> CreateClinica([FromBody] ClinicaCreateDto dto)
    {
        // Verificar se email já existe
        if (await _context.Clinicas.AnyAsync(c => c.Email == dto.Email))
            return BadRequest(new { message = "Email já está em uso" });

        var clinica = new Clinica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Cp = dto.Cp,
            Nif = dto.Nif,
            Iban = dto.Iban
        };

        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClinica), new { id = clinica.Id }, new
        {
            clinica.Id,
            clinica.Nome,
            clinica.Email,
            clinica.Cp,
            clinica.Nif,
            clinica.Iban
        });
    }

    [HttpPut("clinicas/{id}")]
    public async Task<IActionResult> UpdateClinica(int id, [FromBody] ClinicaUpdateDto dto)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        if (clinica == null)
            return NotFound(new { message = "Clínica não encontrada" });

        // Verificar se email já existe em outra clínica
        if (await _context.Clinicas.AnyAsync(c => c.Email == dto.Email && c.Id != id))
            return BadRequest(new { message = "Email já está em uso" });

        clinica.Nome = dto.Nome;
        clinica.Email = dto.Email;
        clinica.Cp = dto.Cp;
        clinica.Nif = dto.Nif;
        clinica.Iban = dto.Iban;

        // Só atualizar password se fornecida
        if (!string.IsNullOrEmpty(dto.Password))
        {
            clinica.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            clinica.Id,
            clinica.Nome,
            clinica.Email,
            clinica.Cp,
            clinica.Nif,
            clinica.Iban
        });
    }

    [HttpDelete("clinicas/{id}")]
    public async Task<IActionResult> DeleteClinica(int id)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        if (clinica == null)
            return NotFound(new { message = "Clínica não encontrada" });

        // Soft delete - desativa a clínica
        clinica.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Clínica desativada com sucesso" });
    }

    // ========== ESTATÍSTICAS GLOBAIS ==========

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetGlobalStats()
    {
        var totalClinicas = await _context.Clinicas.CountAsync(c => c.Ativo);
        var totalFuncionarios = await _context.Funcionarios.CountAsync(f => f.Ativo);
        var totalClientes = await _context.Clientes.CountAsync();

        return Ok(new
        {
            totalClinicas,
            totalFuncionarios,
            totalClientes
        });
    }

    // ========== GESTÃO DE ADMINS ==========

    [HttpGet("admins")]
    [Authorize(Roles = "Admin")] // Apenas super_admin deveria ver isso idealmente
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _context.Admins
            .Select(a => new
            {
                a.Id,
                a.Nome,
                a.Email,
                a.Nivel,
                a.Ativo
            })
            .ToListAsync();

        return Ok(admins);
    }

    [HttpPost("admins")]
    [Authorize(Roles = "Admin")] // Apenas super_admin deveria criar
    public async Task<IActionResult> CreateAdmin([FromBody] AdminCreateDto dto)
    {
        if (await _context.Admins.AnyAsync(a => a.Email == dto.Email))
            return BadRequest(new { message = "Email já está em uso" });

        var admin = new Admin
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Nivel = dto.Nivel ?? "admin"
        };

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllAdmins), new { id = admin.Id }, new
        {
            admin.Id,
            admin.Nome,
            admin.Email,
            admin.Nivel
        });
    }

    // ========== GESTÃO DE FUNCIONÁRIOS (ADMIN) ==========

    [HttpDelete("funcionarios/{id}")]
    public async Task<IActionResult> DeleteFuncionario(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null)
            return NotFound(new { message = "Funcionário não encontrado" });

        // Soft delete - desativa o funcionário
        funcionario.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Funcionário desativado com sucesso" });
    }

    [HttpPatch("funcionarios/{id}/ativo")]
    public async Task<IActionResult> UpdateAtivoFuncionario(int id, [FromBody] UpdateAtivoAdminModel model)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null)
            return NotFound(new { message = "Funcionário não encontrado" });

        funcionario.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Funcionário {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = funcionario.Ativo });
    }

    [HttpPatch("clinicas/{id}/ativo")]
    public async Task<IActionResult> UpdateAtivoClinica(int id, [FromBody] UpdateAtivoAdminModel model)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        if (clinica == null)
            return NotFound(new { message = "Clínica não encontrada" });

        clinica.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Clínica {(model.Ativo ? "ativada" : "desativada")} com sucesso", ativo = clinica.Ativo });
    }
}

// DTOs
public class UpdateAtivoAdminModel
{
    public bool Ativo { get; set; }
}


public class ClinicaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Cp { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string Iban { get; set; } = string.Empty;
}

public class ClinicaUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Cp { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string Iban { get; set; } = string.Empty;
}

public class AdminCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Nivel { get; set; }
}
