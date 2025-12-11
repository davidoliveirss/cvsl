using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicasController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public ClinicasController(ClinicaDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClinica(int id)
    {
        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (clinica == null)
        {
            return NotFound(new { message = "Clínica não encontrada" });
        }

        return Ok(new
        {
            id = clinica.Id,
            nome = clinica.Nome,
            email = clinica.Email,
            cp = clinica.Cp,
            nif = clinica.Nif,
            iban = clinica.Iban,
            ativo = clinica.Ativo
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClinica(int id, [FromBody] UpdateClinicaModel model)
    {
        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (clinica == null)
        {
            return NotFound(new { message = "Clinica não encontrada" });
        }

        // Atualiza apenas CP, NIF e IBAN
        clinica.Cp = model.Cp;
        clinica.Nif = model.Nif;
        clinica.Iban = model.Iban;
        
        await _context.SaveChangesAsync();

        return Ok(new { message = "Clinica atualizada com sucesso" });
    }

    // ========== GESTÃO DE FUNCIONÁRIOS ==========

    [HttpPost("{clinicaId}/funcionarios")]
    public async Task<IActionResult> RegisterFuncionario(int clinicaId, [FromBody] RegisterFuncionarioModel model)
    {
        // Verifica se a clínica existe
        var clinicaExiste = await _context.Clinicas
            .AnyAsync(c => c.Id == clinicaId);
        
        if (!clinicaExiste)
        {
            return BadRequest(new { message = "Clínica não encontrada" });
        }

        // Verifica se o email já existe
        var emailExiste = await _context.Funcionarios
            .AnyAsync(f => f.Email == model.Email);
        
        if (emailExiste)
        {
            return BadRequest(new { message = "Email já está em uso" });
        }

        // Cria o novo funcionário
        var novoFuncionario = new Funcionario
        {
            Nome = model.Nome,
            Especialidade = model.Especialidade,
            Telefone = model.Telefone,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Salario = model.Salario,
            ClinicaId = clinicaId
        };

        _context.Funcionarios.Add(novoFuncionario);
        await _context.SaveChangesAsync();

        return Ok(new 
        {
            message = "Funcionário criado com sucesso",
            id = novoFuncionario.Id,
            nome = novoFuncionario.Nome,
            email = novoFuncionario.Email
        });
    }

    [HttpGet("{clinicaId}/funcionarios")]
    public async Task<IActionResult> GetFuncionariosByClinica(int clinicaId, [FromQuery] bool incluirInativos = false)
    {
        var query = _context.Funcionarios.Where(f => f.ClinicaId == clinicaId);
        
        if (!incluirInativos)
        {
            query = query.Where(f => f.Ativo);
        }
        
        var funcionarios = await query
            .Select(f => new
            {
                id = f.Id,
                nome = f.Nome,
                especialidade = f.Especialidade,
                telefone = f.Telefone,
                email = f.Email,
                salario = f.Salario,
                ativo = f.Ativo
            })
            .ToListAsync();

        return Ok(funcionarios);
    }

    [HttpGet("{clinicaId}/funcionarios/{funcionarioId}")]
    public async Task<IActionResult> GetFuncionario(int clinicaId, int funcionarioId)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        return Ok(new
        {
            id = funcionario.Id,
            nome = funcionario.Nome,
            especialidade = funcionario.Especialidade,
            telefone = funcionario.Telefone,
            email = funcionario.Email,
            salario = funcionario.Salario,
            clinicaId = funcionario.ClinicaId,
            ativo = funcionario.Ativo
        });
    }

    [HttpPut("{clinicaId}/funcionarios/{funcionarioId}")]
    public async Task<IActionResult> UpdateFuncionario(int clinicaId, int funcionarioId, [FromBody] UpdateFuncionarioModel model)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        // Verifica se o novo email já existe (exceto o próprio)
        if (model.Email != funcionario.Email)
        {
            var emailExiste = await _context.Funcionarios
                .AnyAsync(f => f.Email == model.Email && f.Id != funcionarioId);
            
            if (emailExiste)
            {
                return BadRequest(new { message = "Email já está em uso" });
            }
        }

        // Atualiza os dados
        funcionario.Nome = model.Nome;
        funcionario.Especialidade = model.Especialidade;
        funcionario.Telefone = model.Telefone;
        funcionario.Email = model.Email;
        funcionario.Salario = model.Salario;

        if (!string.IsNullOrEmpty(model.Password))
        {
            funcionario.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Funcionário atualizado com sucesso" });
    }

    [HttpDelete("{clinicaId}/funcionarios/{funcionarioId}")]
    public async Task<IActionResult> DeleteFuncionario(int clinicaId, int funcionarioId)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        // Soft delete - desativa o funcionário
        funcionario.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Funcionário desativado com sucesso" });
    }

    [HttpPatch("{clinicaId}/funcionarios/{funcionarioId}/ativo")]
    public async Task<IActionResult> UpdateAtivoFuncionario(int clinicaId, int funcionarioId, [FromBody] UpdateAtivoFuncionarioModel model)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        funcionario.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Funcionário {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = funcionario.Ativo });
    }

}

// Model para atualizar clínica (apenas CP, NIF e IBAN)
public class UpdateClinicaModel
{
    public string Cp { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string Iban { get; set; } = string.Empty;
}

// Model para atualizar status ativo do funcionário
public class UpdateAtivoFuncionarioModel
{
    public bool Ativo { get; set; }
}

// Model para criar funcionário
public class RegisterFuncionarioModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public decimal Salario { get; set; }
}

// Model para atualizar funcionário
public class UpdateFuncionarioModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public decimal Salario { get; set; }
}
