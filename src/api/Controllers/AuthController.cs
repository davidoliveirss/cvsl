using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using api.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ClinicaDbContext _context;

    public AuthController(IConfiguration configuration, ClinicaDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login/clinica")]
    [SwaggerOperation(
        Summary = "Login para clinicas",
        Description = "Login para clinicas, retorna token com clinicid"
    )]
    public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
    {
        // MOCKUP DATA - Para testes sem BD
        if (model.Email == "clinica@teste.pt" && model.Password == "teste123")
        {
            var clinicaToken = GenerateJwtToken(1, "clinica@teste.pt", "Clínica Veterinária Teste", "Clinica");
            
            return Ok(new AuthResponse
            {
                Token = clinicaToken,
                Email = "clinica@teste.pt",
                Nome = "Clínica Veterinária Teste"
            });
        }

        // Verificação real na BD com hash
        var clinica = await _context.Clinicas
            .SingleOrDefaultAsync(c => c.Email == model.Email && c.Ativo);
        
        if (clinica == null || !BCrypt.Net.BCrypt.Verify(model.Password, clinica.Password))
        {
            return Unauthorized(new { message = "Email ou password inválidos" });
        }

        var token = GenerateJwtToken(clinica.Id, clinica.Email, clinica.Nome, "Clinica");
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = clinica.Email,
            Nome = clinica.Nome
        });
    }

    [HttpPost("login/funcionario")]
    [SwaggerOperation(
        Summary = "Login para funcionario",
        Description = "Login para funcionario, retorna token com funcionario"
    )]
    public async Task<IActionResult> LoginFuncionario([FromBody] LoginModel model)
    {
        // MOCKUP DATA - Para testes sem BD
        if (model.Email == "vet@teste.pt" && model.Password == "teste123")
        {
            var vetToken = GenerateJwtToken(1, "vet@teste.pt", "Dr. João Silva", "Funcionario", 1);
            
            return Ok(new AuthResponse
            {
                Token = vetToken,
                Email = "vet@teste.pt",
                Nome = "Dr. João Silva"
            });
        }

        if (model.Email == "rececionista@teste.pt" && model.Password == "teste123")
        {
            var recepcionistaToken = GenerateJwtToken(2, "rececionista@teste.pt", "Maria Santos", "Funcionario", 1);
            
            return Ok(new AuthResponse
            {
                Token = recepcionistaToken,
                Email = "rececionista@teste.pt",
                Nome = "Maria Santos"
            });
        }

        // Verificação real na BD com hash
        var funcionario = await _context.Funcionarios
            .SingleOrDefaultAsync(f => f.Email == model.Email && f.Ativo);
        
        if (funcionario == null || !BCrypt.Net.BCrypt.Verify(model.Password, funcionario.Password))
        {
            return Unauthorized(new { message = "Email ou password inválidos" });
        }

        var funcionarioToken = GenerateJwtToken(funcionario.Id, funcionario.Email, funcionario.Nome, "Funcionario", funcionario.ClinicaId);
        
        return Ok(new AuthResponse
        {
            Token = funcionarioToken,
            Email = funcionario.Email,
            Nome = funcionario.Nome
        });
    }

    [HttpPost("login/admin")]
    [SwaggerOperation(
        Summary = "Login para admins",
        Description = "Login para admins, retorna token com admin"
    )]
    public async Task<IActionResult> LoginAdmin([FromBody] LoginModel model)
    {
        // MOCKUP DATA - Para testes
        if (model.Email == "admin@cvsl.pt" && model.Password == "admin123")
        {
            var adminToken = GenerateJwtToken(1, "admin@cvsl.pt", "Admin Principal", "Admin");
            
            return Ok(new AuthResponse
            {
                Token = adminToken,
                Email = "admin@cvsl.pt",
                Nome = "Admin Principal"
            });
        }

        // Verificação real na BD
        var admin = await _context.Admins
            .SingleOrDefaultAsync(a => a.Email == model.Email && a.Ativo);
        
        if (admin == null || !BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
        {
            return Unauthorized(new { message = "Email ou password inválidos" });
        }

        var token = GenerateJwtToken(admin.Id, admin.Email, admin.Nome, "Admin", null, admin.Nivel);
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = admin.Email,
            Nome = admin.Nome
        });
    }

    private string GenerateJwtToken(int userId, string email, string nome, string role, int? clinicaId = null, string? nivel = null)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "ChaveSecretaSuperSegura123456789012345678901234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nome),
            new Claim(ClaimTypes.Role, role)
        };

        if (clinicaId.HasValue)
        {
            claims.Add(new Claim("ClinicaId", clinicaId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(nivel))
        {
            claims.Add(new Claim("Nivel", nivel));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "ClinicaAPI",
            audience: jwtSettings["Audience"] ?? "ClinicaWebApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
