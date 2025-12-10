using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public FuncionariosController(ClinicaDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterFuncionario([FromBody] RegisterFuncionarioModel model)
    {
        // Verifica se o email já existe
        var emailExiste = await _context.Funcionarios
            .AnyAsync(f => f.Email == model.Email);
        
        if (emailExiste)
        {
            return BadRequest(new { message = "Email já está em uso" });
        }

        // Verifica se a clínica existe
        var clinicaExiste = await _context.Clinicas
            .AnyAsync(c => c.Id == model.ClinicaId);
        
        if (!clinicaExiste)
        {
            return BadRequest(new { message = "Clínica não encontrada" });
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
            ClinicaId = model.ClinicaId
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFuncionario(int id)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == id);
        
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
            clinicaId = funcionario.ClinicaId
        });
    }

    [HttpGet("clinica/{clinicaId}")]
    public async Task<IActionResult> GetFuncionariosByClinica(int clinicaId)
    {
        var funcionarios = await _context.Funcionarios
            .Where(f => f.ClinicaId == clinicaId)
            .Select(f => new
            {
                id = f.Id,
                nome = f.Nome,
                especialidade = f.Especialidade,
                telefone = f.Telefone,
                email = f.Email,
                salario = f.Salario
            })
            .ToListAsync();

        return Ok(funcionarios);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFuncionario(int id, [FromBody] UpdateFuncionarioModel model)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == id);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        // Verifica se o novo email já existe (exceto o próprio)
        if (model.Email != funcionario.Email)
        {
            var emailExiste = await _context.Funcionarios
                .AnyAsync(f => f.Email == model.Email && f.Id != id);
            
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFuncionario(int id)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == id);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        _context.Funcionarios.Remove(funcionario);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Funcionário removido com sucesso" });
    }
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
    public int ClinicaId { get; set; }
}

// Model para atualizar funcionário
public class UpdateFuncionarioModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } // Opcional na atualização
    public decimal Salario { get; set; }
}
