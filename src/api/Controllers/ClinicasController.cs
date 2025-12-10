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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterClinica([FromBody] RegisterClinicaModel model)
    {
        // Verifica se o email já existe
        var emailExiste = await _context.Clinicas
            .AnyAsync(c => c.Email == model.Email);
        
        if (emailExiste)
        {
            return BadRequest(new { message = "Email já está em uso" });
        }

        // Cria a nova clínica
        var novaClinica = new Clinica
        {
            Nome = model.Nome,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Cp = model.Cp,
            Nif = model.Nif,
            Iban = model.Iban
        };

        _context.Clinicas.Add(novaClinica);
        await _context.SaveChangesAsync();

        return Ok(new 
        {
            message = "Clínica criada com sucesso",
            id = novaClinica.Id,
            nome = novaClinica.Nome,
            email = novaClinica.Email
        });
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
            iban = clinica.Iban
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

        // Verifica se o novo email já existe (exceto o próprio)
        if (model.Email != clinica.Email)
        {
            var emailExiste = await _context.Clinicas
                .AnyAsync(c => c.Email == model.Email && c.Id != id);
            
            if (emailExiste)
            {
                return BadRequest(new { message = "Email já está em uso" });
            }
        }

        // Atualiza os dados
        clinica.Nome = model.Nome;
        clinica.Email = model.Email;
        clinica.Cp = model.Cp;
        clinica.Nif = model.Nif;
        clinica.Iban = model.Iban;

        if (!string.IsNullOrEmpty(model.Password))
        {
            clinica.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Clinica atualizada com sucesso" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinica(int id)
    {
        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (clinica == null)
        {
            return NotFound(new { message = "Clínica não encontrada" });
        }

        _context.Clinicas.Remove(clinica);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Clínica removida com sucesso" });
    }
}

// Model para criar funcionário
public class RegisterClinicaModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Cp { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string Iban { get; set; } = string.Empty;
}

// Model para atualizar funcionário
public class UpdateClinicaModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Cp { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string Iban { get; set; } = string.Empty;
}
