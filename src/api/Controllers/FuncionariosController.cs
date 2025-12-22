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
    public async Task<IActionResult> GetClientes([FromQuery] string? pesquisa = null, [FromQuery] bool incluirInativos = false)
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
        
        if (!incluirInativos)
        {
            query = query.Where(c => c.Ativo);
        }
        
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
                email = c.Email,
                ativo = c.Ativo
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
            email = cliente.Email,
            ativo = cliente.Ativo
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
        Summary = "Desativar cliente",
        Description = "Metodo para desativar um cliente"
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

        // Soft delete - desativa o cliente
        cliente.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cliente desativado com sucesso" });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpPatch("clientes/{clienteId}/ativo")]
    [SwaggerOperation(
        Summary = "Alterar estado ativo do cliente",
        Description = "Metodo para ativar ou desativar um cliente"
    )]
    public async Task<IActionResult> UpdateAtivoCliente(int clienteId, [FromBody] UpdateAtivoClienteModel model)
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

        cliente.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Cliente {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = cliente.Ativo });
    }

    // ========== GESTÃO DE ANIMAIS ==========

    [Authorize(Roles = "Funcionario")]
    [HttpPost("animais")]
    [SwaggerOperation(
        Summary = "Registar animal",
        Description = "Metodo para registar um animal para um cliente da clinica"
    )]
    public async Task<IActionResult> CreateAnimal([FromBody] CreateAnimalModel model)
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

        // Verificar se o cliente existe e pertence à mesma clínica
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == model.IdCliente);
        
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        if (cliente.ClinicaId != funcionario.ClinicaId)
        {
            return BadRequest(new { message = "Cliente não pertence a esta clínica" });
        }

        // Verificar se o transponder já existe (se fornecido)
        if (!string.IsNullOrEmpty(model.Transponder))
        {
            var transponderExiste = await _context.Animais
                .AnyAsync(a => a.Transponder == model.Transponder);
            
            if (transponderExiste)
            {
                return BadRequest(new { message = "Transponder já está em uso" });
            }
        }

        var novoAnimal = new Animal
        {
            Transponder = model.Transponder,
            Nome = model.Nome,
            Especie = model.Especie,
            Raca = model.Raca,
            DataNascimento = model.DataNascimento,
            Sexo = model.Sexo,
            IdCliente = model.IdCliente,
            IdClinica = funcionario.ClinicaId
        };

        _context.Animais.Add(novoAnimal);
        await _context.SaveChangesAsync();

        return Ok(new 
        {
            message = "Animal criado com sucesso",
            id = novoAnimal.Id,
            nome = novoAnimal.Nome,
            transponder = novoAnimal.Transponder
        });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpGet("animais")]
    [SwaggerOperation(
        Summary = "Listar animais",
        Description = "Metodo para listar animais da clinica. Pode filtrar por cliente ou pesquisa."
    )]
    public async Task<IActionResult> GetAnimais(
        [FromQuery] int? clienteId = null, 
        [FromQuery] string? pesquisa = null, 
        [FromQuery] bool incluirInativos = false)
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

        var query = _context.Animais
            .Include(a => a.Cliente)
            .Where(a => a.IdClinica == funcionario.ClinicaId);
        
        if (!incluirInativos)
        {
            query = query.Where(a => a.Ativo);
        }

        if (clienteId.HasValue)
        {
            query = query.Where(a => a.IdCliente == clienteId.Value);
        }
        
        if (!string.IsNullOrEmpty(pesquisa))
        {
            query = query.Where(a => 
                a.Nome.Contains(pesquisa) || 
                (a.Transponder != null && a.Transponder.Contains(pesquisa)) ||
                (a.Especie != null && a.Especie.Contains(pesquisa)) ||
                (a.Raca != null && a.Raca.Contains(pesquisa))
            );
        }
        
        var animais = await query
            .Select(a => new
            {
                id = a.Id,
                transponder = a.Transponder,
                nome = a.Nome,
                especie = a.Especie,
                raca = a.Raca,
                dataNascimento = a.DataNascimento,
                sexo = a.Sexo,
                idCliente = a.IdCliente,
                nomeCliente = a.Cliente!.Nome,
                ativo = a.Ativo
            })
            .ToListAsync();

        return Ok(animais);
    }

    [Authorize(Roles = "Funcionario")]
    [HttpGet("animais/{animalId}")]
    [SwaggerOperation(
        Summary = "Pesquisar animal",
        Description = "Metodo para pesquisar um animal"
    )]
    public async Task<IActionResult> GetAnimal(int animalId)
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

        var animal = await _context.Animais
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.Id == animalId);
        
        if (animal == null)
        {
            return NotFound(new { message = "Animal não encontrado" });
        }

        if (animal.IdClinica != funcionario.ClinicaId)
        {
            return Forbid();
        }

        return Ok(new
        {
            id = animal.Id,
            transponder = animal.Transponder,
            nome = animal.Nome,
            especie = animal.Especie,
            raca = animal.Raca,
            dataNascimento = animal.DataNascimento,
            sexo = animal.Sexo,
            idCliente = animal.IdCliente,
            nomeCliente = animal.Cliente?.Nome,
            ativo = animal.Ativo
        });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpPut("animais/{animalId}")]
    [SwaggerOperation(
        Summary = "Atualizar animal",
        Description = "Metodo para atualizar informações do animal"
    )]
    public async Task<IActionResult> UpdateAnimal(int animalId, [FromBody] UpdateAnimalModel model)
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

        var animal = await _context.Animais
            .FirstOrDefaultAsync(a => a.Id == animalId);
        
        if (animal == null)
        {
            return NotFound(new { message = "Animal não encontrado" });
        }

        if (animal.IdClinica != funcionario.ClinicaId)
        {
            return Forbid();
        }

        // Verificar se o cliente existe e pertence à mesma clínica
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == model.IdCliente);
        
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        if (cliente.ClinicaId != funcionario.ClinicaId)
        {
            return BadRequest(new { message = "Cliente não pertence a esta clínica" });
        }

        // Verificar se o transponder já existe (se fornecido e diferente do atual)
        if (!string.IsNullOrEmpty(model.Transponder) && model.Transponder != animal.Transponder)
        {
            var transponderExiste = await _context.Animais
                .AnyAsync(a => a.Transponder == model.Transponder && a.Id != animalId);
            
            if (transponderExiste)
            {
                return BadRequest(new { message = "Transponder já está em uso" });
            }
        }

        animal.Transponder = model.Transponder;
        animal.Nome = model.Nome;
        animal.Especie = model.Especie;
        animal.Raca = model.Raca;
        animal.DataNascimento = model.DataNascimento;
        animal.Sexo = model.Sexo;
        animal.IdCliente = model.IdCliente;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Animal atualizado com sucesso" });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpDelete("animais/{animalId}")]
    [SwaggerOperation(
        Summary = "Desativar animal",
        Description = "Metodo para desativar um animal"
    )]
    public async Task<IActionResult> DeleteAnimal(int animalId)
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

        var animal = await _context.Animais
            .FirstOrDefaultAsync(a => a.Id == animalId);
        
        if (animal == null)
        {
            return NotFound(new { message = "Animal não encontrado" });
        }

        if (animal.IdClinica != funcionario.ClinicaId)
        {
            return Forbid();
        }

        // Soft delete - desativa o animal
        animal.Ativo = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Animal desativado com sucesso" });
    }

    [Authorize(Roles = "Funcionario")]
    [HttpPatch("animais/{animalId}/ativo")]
    [SwaggerOperation(
        Summary = "Alterar estado ativo do animal",
        Description = "Metodo para ativar ou desativar um animal"
    )]
    public async Task<IActionResult> UpdateAtivoAnimal(int animalId, [FromBody] UpdateAtivoAnimalModel model)
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

        var animal = await _context.Animais
            .FirstOrDefaultAsync(a => a.Id == animalId);
        
        if (animal == null)
        {
            return NotFound(new { message = "Animal não encontrado" });
        }

        if (animal.IdClinica != funcionario.ClinicaId)
        {
            return Forbid();
        }

        animal.Ativo = model.Ativo;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Animal {(model.Ativo ? "ativado" : "desativado")} com sucesso", ativo = animal.Ativo });
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

public class UpdateAtivoClienteModel
{
    public bool Ativo { get; set; }
}

// Models para Animais
public class CreateAnimalModel
{
    public string? Transponder { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Especie { get; set; }
    public string? Raca { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public string? Sexo { get; set; }
    public int IdCliente { get; set; }
}

public class UpdateAnimalModel
{
    public string? Transponder { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Especie { get; set; }
    public string? Raca { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public string? Sexo { get; set; }
    public int IdCliente { get; set; }
}

public class UpdateAtivoAnimalModel
{
    public bool Ativo { get; set; }
}
