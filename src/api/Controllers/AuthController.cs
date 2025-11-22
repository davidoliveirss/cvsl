using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // TODO: Substituir por validação real com base de dados
        // Por agora, credenciais de teste
        var user = ValidateUser(model.Email, model.Password);
        
        if (user == null)
        {
            return Unauthorized(new { message = "Email ou password inválidos" });
        }

        var token = GenerateJwtToken(user);
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Nome = user.Nome
        });
    }

    private User? ValidateUser(string email, string password)
    {
        // TODO: Substituir por query à base de dados
        // Utilizadores de teste (REMOVER em produção)
        if (email == "admin@clinica.pt" && password == "admin123")
        {
            return new User
            {
                Id = 1,
                Email = email,
                Nome = "Administrador",
                Role = "Admin"
            };
        }

        if (email == "user@clinica.pt" && password == "user123")
        {
            return new User
            {
                Id = 2,
                Email = email,
                Nome = "Utilizador Teste",
                Role = "User"
            };
        }

        return null;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "ChaveSecretaSuperSegura123456789012345678901234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Role, user.Role)
        };

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
