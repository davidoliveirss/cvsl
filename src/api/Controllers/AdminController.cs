using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Context;
using BCrypt.Net;
using Swashbuckle.AspNetCore.Annotations;

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


    [HttpGet("clinicas")] //pesquisas clinicas em paginas
    [SwaggerOperation(
        Summary = "Listar clinicas",
        Description = "Metodo para listar clinicas, retorna em paginas"
    )]
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

    [HttpGet("clinicas/{id}")] //procurar clinica
    [SwaggerOperation(
        Summary = "Pesquisar uma clinicas",
        Description = "Metodo para pesquisar apenas clinicas"
    )]
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

    [HttpPost("clinicas")] //criar clinica 
    [SwaggerOperation(
        Summary = "Registar clinicas",
        Description = "Metodo para registar clinicas"
    )]
    public async Task<IActionResult> CreateClinica([FromBody] ClinicaCreateDto dto)
    {
        // Verificar se email já existe
        if (await _context.Clinicas.AnyAsync(c => c.Email == dto.Email))
            return BadRequest(new { message = "Email já está em uso" });

        dto.Iban = dto.Iban?.Replace(" ", "");
        dto.Nif = dto.Nif?.Replace(" ", ""); // também por segurança

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

    [HttpPut("clinicas/{id}")] //alterar informacoes clinica
    [SwaggerOperation(
        Summary = "Atualizar clinicas",
        Description = "Metodo para atualizar clinicas"
    )]
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

    // ========== ESTATÍSTICAS GLOBAIS ==========

    [HttpGet("dashboard/stats")]
    [SwaggerOperation(
        Summary = "Stats da aplicacao",
        Description = "Metodo para ver os stats da aplicacao"
    )]
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
    [Authorize(Roles = "Admin")] // Apenas super_admin deveria ver isso idealmente -- nao sei se está apenas superadmin
    [SwaggerOperation(
        Summary = "Pesquisar admins",
        Description = "Metodo para pesquisar admins"
    )]
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
    [SwaggerOperation(
        Summary = "Registar admins",
        Description = "Metodo para registar admins"
    )]
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

    // ========== GESTÃO DE FUNCIONÁRIOS/clinicas (ADMIN) ==========

    [HttpDelete("funcionarios/{id}")] //endpoint para desativar funcionario
    [SwaggerOperation(
        Summary = "Desativar funcionarios",
        Description = "Metodo para desativar funcionarios"
    )]
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

    [HttpDelete("clinicas/{id}")] //endpoint para desativar clinica
    [SwaggerOperation(
        Summary = "Desativar clinicas",
        Description = "Metodo para desativar clinicas"
    )]
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

    [HttpPatch("funcionarios/{id}/ativo")] //endpoint para ativar funcionario
    public async Task<IActionResult> UpdateAtivoFuncionario(int id, [FromBody] UpdateAtivoAdminModel model)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null)
            return NotFound(new { message = "Funcionário não encontrado" });

        funcionario.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Funcionário {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = funcionario.Ativo });
    }

    [HttpPatch("clinicas/{id}/ativo")] //endpoint para ativar clinica
    [SwaggerOperation(
        Summary = "Ativar clinicas",
        Description = "Metodo para ativar clinicas"
    )]
    public async Task<IActionResult> UpdateAtivoClinica(int id, [FromBody] UpdateAtivoAdminModel model)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        if (clinica == null)
            return NotFound(new { message = "Clínica não encontrada" });

        clinica.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Clínica {(model.Ativo ? "ativada" : "desativada")} com sucesso", ativo = clinica.Ativo });
    }

    // ========== GESTÃO DE CATEGORIAS ==========

    [HttpPost("categorias")]
    [SwaggerOperation(
        Summary = "Criar categoria global",
        Description = "Metodo para criar uma categoria (disponível para todas as clínicas)"
    )]
    public async Task<IActionResult> CreateCategoria([FromBody] CreateCategoriaModel model)
    {
        var novaCategoria = new Categoria
        {
            Nome = model.Nome,
            Descricao = model.Descricao,
            Iva = model.Iva
        };

        _context.Categorias.Add(novaCategoria);
        await _context.SaveChangesAsync();

        return Ok(new 
        {
            message = "Categoria criada com sucesso",
            id = novaCategoria.Id,
            nome = novaCategoria.Nome,
            descricao = novaCategoria.Descricao,
            iva = novaCategoria.Iva
        });
    }

    [HttpGet("categorias")]
    [SwaggerOperation(
        Summary = "Listar categorias globais",
        Description = "Metodo para listar todas as categorias"
    )]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _context.Categorias
            .Select(c => new
            {
                id = c.Id,
                nome = c.Nome,
                descricao = c.Descricao,
                iva = c.Iva
            })
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("categorias/{categoriaId}")]
    [SwaggerOperation(
        Summary = "Pesquisar categoria",
        Description = "Metodo para pesquisar uma categoria"
    )]
    public async Task<IActionResult> GetCategoria(int categoriaId)
    {
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaId);
        
        if (categoria == null)
        {
            return NotFound(new { message = "Categoria não encontrada" });
        }

        return Ok(new
        {
            id = categoria.Id,
            nome = categoria.Nome,
            descricao = categoria.Descricao,
            iva = categoria.Iva
        });
    }

    [HttpPut("categorias/{categoriaId}")]
    [SwaggerOperation(
        Summary = "Atualizar categoria",
        Description = "Metodo para atualizar informações de uma categoria"
    )]
    public async Task<IActionResult> UpdateCategoria(int categoriaId, [FromBody] UpdateCategoriaModel model)
    {
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaId);
        
        if (categoria == null)
        {
            return NotFound(new { message = "Categoria não encontrada" });
        }

        categoria.Nome = model.Nome;
        categoria.Descricao = model.Descricao;
        categoria.Iva = model.Iva;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Categoria atualizada com sucesso" });
    }

    [HttpDelete("categorias/{categoriaId}")]
    [SwaggerOperation(
        Summary = "Apagar categoria",
        Description = "Metodo para apagar uma categoria"
    )]
    public async Task<IActionResult> DeleteCategoria(int categoriaId)
    {
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaId);
        
        if (categoria == null)
        {
            return NotFound(new { message = "Categoria não encontrada" });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Categoria eliminada com sucesso" });
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

// Models para Categorias
public class CreateCategoriaModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Iva { get; set; }
}

public class UpdateCategoriaModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Iva { get; set; }
}
