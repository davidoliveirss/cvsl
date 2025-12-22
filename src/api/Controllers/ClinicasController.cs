using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

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

    // Helper method para validar se a clínica logada tem acesso
    private int? GetClinicaIdFromToken()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
        // Admins têm acesso a tudo (retorna null para indicar admin)
        if (userRole == "Admin")
        {
            return -1; // Valor especial para admin
        }
        
        // Clínicas - retorna o ID da clínica do token
        if (userRole == "Clinica")
        {
            var clinicaIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (clinicaIdFromToken != null && int.TryParse(clinicaIdFromToken, out int clinicaId))
            {
                return clinicaId;
            }
        }
        
        return null;
    }

    private bool ValidateClinicaAccess(int clinicaId)
    {
        var clinicaIdFromToken = GetClinicaIdFromToken();
        
        // Se não conseguiu obter ID do token, não tem acesso
        if (clinicaIdFromToken == null)
        {
            return false;
        }
        
        // Admin tem acesso a tudo
        if (clinicaIdFromToken == -1)
        {
            return true;
        }
        
        // Clínica só tem acesso aos seus próprios dados
        return clinicaIdFromToken == clinicaId;
    }

    [Authorize(Roles = "Clinica")]
    [HttpGet("perfil")]
    [SwaggerOperation(
        Summary = "Perfil clinica",
        Description = "Com o token do login listar as defenições da clinica"
    )]
    public async Task<IActionResult> GetClinica()
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == clinicaId.Value);
        
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

    [Authorize(Roles = "Clinica")]
    [HttpPut("perfil")]
    [SwaggerOperation(
        Summary = "Atualizar informações da clinica",
        Description = "Metodo para atualizar as informações da clinica"
    )]
    public async Task<IActionResult> UpdateClinica([FromBody] UpdateClinicaModel model)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == clinicaId.Value);
        
        if (clinica == null)
        {
            return NotFound(new { message = "Clínica não encontrada" });
        }

        // Atualiza apenas CP, NIF e IBAN
        clinica.Cp = model.Cp;
        clinica.Nif = model.Nif;
        clinica.Iban = model.Iban;
        
        await _context.SaveChangesAsync();

        return Ok(new { message = "Clínica atualizada com sucesso" });
    }

    // ========== GESTÃO DE FUNCIONÁRIOS ==========

    [Authorize(Roles = "Clinica")]
    [HttpPost("funcionarios")]
    [SwaggerOperation(
        Summary = "Registar funcionario",
        Description = "Metodo para registar um funcionario na clinica que está logada"
    )]
    public async Task<IActionResult> RegisterFuncionario([FromBody] RegisterFuncionarioModel model)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
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
            ClinicaId = clinicaId.Value
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

    [Authorize(Roles = "Clinica")]
    [HttpGet("funcionarios")]
    [SwaggerOperation(
        Summary = "Listar funcionarios",
        Description = "Metodo para listar funcionarios"
    )]
    public async Task<IActionResult> GetFuncionarios([FromQuery] bool incluirInativos = false)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var query = _context.Funcionarios.Where(f => f.ClinicaId == clinicaId.Value);
        
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

    [Authorize(Roles = "Clinica")]
    [HttpGet("funcionarios/{funcionarioId}")]
    [SwaggerOperation(
        Summary = "Pesquisar um funcionario",
        Description = "Metodo para pesquisar um funcionario"
    )]
    public async Task<IActionResult> GetFuncionario(int funcionarioId)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId.Value);
        
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
            ativo = funcionario.Ativo
        });
    }

    [Authorize(Roles = "Clinica")]
    [HttpPut("funcionarios/{funcionarioId}")]
    [SwaggerOperation(
        Summary = "Atualizar funcionario",
        Description = "Metodo para atualizar informações do funcionario"
    )]
    public async Task<IActionResult> UpdateFuncionario(int funcionarioId, [FromBody] UpdateFuncionarioModel model)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId.Value);
        
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

    [Authorize(Roles = "Clinica")]
    [HttpDelete("funcionarios/{funcionarioId}")]
    [SwaggerOperation(
        Summary = "Desativar funcionario",
        Description = "Metodo para desativar funcionario"
    )]
    public async Task<IActionResult> DeleteFuncionario(int funcionarioId)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId.Value);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        // Soft delete - desativa o funcionário
        funcionario.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Funcionário desativado com sucesso" });
    }

    [Authorize(Roles = "Clinica")]
    [HttpPatch("funcionarios/{funcionarioId}/ativo")]
    [SwaggerOperation(
        Summary = "Reativar funcionario",
        Description = "Metodo para reativar funcionario"
    )]
    public async Task<IActionResult> UpdateAtivoFuncionario(int funcionarioId, [FromBody] UpdateAtivoFuncionarioModel model)
    {
        var clinicaId = GetClinicaIdFromToken();
        
        if (clinicaId == null || clinicaId == -1)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Id == funcionarioId && f.ClinicaId == clinicaId.Value);
        
        if (funcionario == null)
        {
            return NotFound(new { message = "Funcionário não encontrado" });
        }

        funcionario.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Funcionário {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = funcionario.Ativo });
    }

    // ========== VISUALIZAÇÃO DE CATEGORIAS (somente leitura) ==========

    [Authorize(Roles = "Clinica")]
    [HttpGet("categorias")]
    [SwaggerOperation(
        Summary = "Listar categorias",
        Description = "Metodo para listar categorias globais"
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
