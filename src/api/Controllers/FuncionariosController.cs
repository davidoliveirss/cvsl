using Microsoft.AspNetCore.Mvc;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

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

    private int? GetFuncionarioIdFromToken()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userRole == "Funcionario")
        {
            var funcionarioIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (funcionarioIdFromToken != null && int.TryParse(funcionarioIdFromToken, out int funcionarioId))
            {
                return funcionarioId;
            }
        }
        
        return null;
    }

    // ========== GESTÃO DE STOCK ==========

    [Authorize(Roles = "Funcionario")]
    [HttpPost("produtos/{produtoId}/adicionar-stock")]
    [SwaggerOperation(
        Summary = "Adicionar stock ao produto",
        Description = "Adiciona uma ou mais caixas ao stock do produto. O valor adicionado é calculado automaticamente com base nas unidades por caixa."
    )]
    public async Task<IActionResult> AdicionarStock(int produtoId, [FromBody] AdicionarStockModel model)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        if (model.NumeroCaixas <= 0)
        {
            return BadRequest(new { message = "Número de caixas deve ser maior que zero" });
        }

        // Obter funcionário e clínica em uma única query
        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.Id, f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        // Buscar produto primeiro para validação explícita
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == produtoId);
        
        if (produto == null)
        {
            return NotFound(new { message = "Produto não encontrado" });
        }

        // Validação explícita de acesso
        if (produto.IdClinica != funcionario.ClinicaId)
        {
            return Forbid(); // 403 - mais apropriado que 404
        }

        var unidadesAdicionadas = produto.UnidadesPorCaixa * model.NumeroCaixas;
        var stockAnterior = produto.QuantidadeStock;
        produto.QuantidadeStock += unidadesAdicionadas;

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Stock adicionado com sucesso",
            produtoId = produto.Id,
            produtoNome = produto.Nome,
            caixasAdicionadas = model.NumeroCaixas,
            unidadesAdicionadas = unidadesAdicionadas,
            stockAnterior = stockAnterior,
            stockAtual = produto.QuantidadeStock
        });
    }

    // ========== GESTÃO DE CLIENTES ==========

    [Authorize(Roles = "Funcionario")]
    [HttpPost("clientes")]
    [SwaggerOperation(
        Summary = "Registar cliente",
        Description = "Metodo para registar um cliente na clinica do funcionario logado"
    )]
    public async Task<IActionResult> CreateCliente([FromBody] CreateClienteModel model)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        var novoCliente = new Cliente
        {
            Nome = model.Nome,
            Nif = model.Nif,
            Morada = model.Morada,
            Telefone = model.Telefone,
            Email = model.Email,
            ClinicaId = funcionario.ClinicaId
        };

        _context.Clientes.Add(novoCliente);
        await _context.SaveChangesAsync();

        return Ok(new 
        {
            message = "Cliente criado com sucesso",
            id = novoCliente.Id,
            nome = novoCliente.Nome,
            telefone = novoCliente.Telefone
        });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpGet("clientes")]
    [SwaggerOperation(
        Summary = "Listar clientes",
        Description = "Metodo para listar clientes da clinica do funcionario logado"
    )]
    public async Task<IActionResult> GetClientes([FromQuery] string? pesquisa = null)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        var query = _context.Clientes.Where(c => c.ClinicaId == funcionario.ClinicaId);
        
        if (!string.IsNullOrEmpty(pesquisa))
        {
            query = query.Where(c => 
                c.Nome.Contains(pesquisa) || 
                c.Telefone.Contains(pesquisa) ||
                (c.Email != null && c.Email.Contains(pesquisa)) ||
                (c.Nif != null && c.Nif.Contains(pesquisa))
            );
        }
        
        var clientes = await query
            .Select(c => new
            {
                id = c.Id,
                nome = c.Nome,
                nif = c.Nif,
                morada = c.Morada,
                telefone = c.Telefone,
                email = c.Email
            })
            .ToListAsync();

        return Ok(clientes);
    }

    [Authorize(Roles = "Funcionario")]
    [HttpGet("clientes/{clienteId}")]
    [SwaggerOperation(
        Summary = "Pesquisar cliente",
        Description = "Metodo para pesquisar um cliente"
    )]
    public async Task<IActionResult> GetCliente(int clienteId)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == clienteId);
        
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        if (cliente.ClinicaId != funcionario.ClinicaId)
        {
            return Forbid();
        }

        return Ok(new
        {
            id = cliente.Id,
            nome = cliente.Nome,
            nif = cliente.Nif,
            morada = cliente.Morada,
            telefone = cliente.Telefone,
            email = cliente.Email
        });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpPut("clientes/{clienteId}")]
    [SwaggerOperation(
        Summary = "Atualizar cliente",
        Description = "Metodo para atualizar informações do cliente"
    )]
    public async Task<IActionResult> UpdateCliente(int clienteId, [FromBody] UpdateClienteModel model)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == clienteId);
        
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        if (cliente.ClinicaId != funcionario.ClinicaId)
        {
            return Forbid();
        }

        cliente.Nome = model.Nome;
        cliente.Nif = model.Nif;
        cliente.Morada = model.Morada;
        cliente.Telefone = model.Telefone;
        cliente.Email = model.Email;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cliente atualizado com sucesso" });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpDelete("clientes/{clienteId}")]
    [SwaggerOperation(
        Summary = "Apagar cliente",
        Description = "Metodo para apagar um cliente"
    )]
    public async Task<IActionResult> DeleteCliente(int clienteId)
    {
        var funcionarioId = GetFuncionarioIdFromToken();
        
        if (funcionarioId == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var funcionario = await _context.Funcionarios
            .Where(f => f.Id == funcionarioId.Value)
            .Select(f => new { f.ClinicaId })
            .FirstOrDefaultAsync();
        
        if (funcionario == null)
        {
            return Unauthorized(new { message = "Funcionário não encontrado" });
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == clienteId);
        
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        if (cliente.ClinicaId != funcionario.ClinicaId)
        {
            return Forbid();
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cliente eliminado com sucesso" });
    }
}

// Model para adicionar stock ao produto
public class AdicionarStockModel
{
    public int NumeroCaixas { get; set; }
}

// Models para Clientes
public class CreateClienteModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string? Morada { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class UpdateClienteModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string? Morada { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
