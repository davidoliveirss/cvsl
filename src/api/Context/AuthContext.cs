using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

class AuthContext:DbContext
{
    public DbSet<api.Models.Funcionario> Funcionarios { get; set; }


    private IConfiguration _config;
    public AuthContext(DbContextOptions<AuthContext> options,IConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _config.GetSection("DB")["ConnectionString"];
        optionsBuilder.UseNpgsql(connectionString);
    }

    public string Login(string email, string password)
    {
        var user = Funcionarios.SingleOrDefault(u => u.Email == email && u.Password == password);
        if (user == null)
        {
            return string.Empty; // Invalid credentials
        }

        // Generate JWT token
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(Funcionario funcionario)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "ChaveSecretaSuperSegura123456789012345678901234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, funcionario.Id.ToString()),
            new Claim(ClaimTypes.Email, funcionario.Email),
            new Claim(ClaimTypes.Name, funcionario.Nome),
            new Claim(ClaimTypes.Role, funcionario.Especialidade)
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