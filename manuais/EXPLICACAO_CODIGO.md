# 📚 EXPLICAÇÃO COMPLETA - CÓDIGO C# API (Passo a Passo)

## 🎯 ÍNDICE
1. [Conceitos Básicos de C#](#conceitos-básicos-de-c)
2. [Estrutura do Projeto](#estrutura-do-projeto)
3. [Models - As Classes que representam dados](#models)
4. [DbContext - A Ponte com a Base de Dados](#dbcontext)
5. [Controllers - Os Endpoints da API](#controllers)
6. [Program.cs - O Arranque da Aplicação](#programcs)
7. [Como Tudo Funciona Junto](#como-tudo-funciona)

---

## 1. CONCEITOS BÁSICOS DE C#

### O que é uma Classe?
É como um molde/template para criar objetos. Pensa numa classe como uma "receita":

```csharp
// Isto é uma CLASSE - a "receita" para criar um Cliente
public class Cliente
{
    // Propriedades - as características do Cliente
    public int Id { get; set; }
    public string Nome { get; set; }
}

// Isto é um OBJETO - uma instância concreta da classe
Cliente cliente1 = new Cliente 
{ 
    Id = 1, 
    Nome = "João Silva" 
};
```

**Analogia:**
- **Classe** = Planta de uma casa (blueprint)
- **Objeto** = Casa construída usando essa planta

---

### Namespaces - Organização de Código

```csharp
namespace api.Models;  // Isto é como uma "pasta virtual"

// É como dizer: "Este código pertence à pasta Models do projeto api"
```

**Porquê?**
- Evita conflitos de nomes
- Organiza o código
- Similar aos `import` em JavaScript ou Python

---

### Propriedades { get; set; }

```csharp
public string Nome { get; set; } = string.Empty;
```

**Tradução linha por linha:**
- `public` = qualquer código pode aceder a isto
- `string` = tipo de dados (texto)
- `Nome` = nome da propriedade
- `{ get; set; }` = pode ler (get) e escrever (set)
- `= string.Empty` = valor inicial (texto vazio "")

**Equivalente em JavaScript:**
```javascript
class Cliente {
    nome = "";  // Isto é semelhante ao C# acima
}
```

---

### Data Annotations - Metadados

```csharp
[Table("clientes")]  // Isto diz ao EF Core qual tabela SQL usar
[Key]                // Isto marca que é a Primary Key
[Required]           // Isto diz que é obrigatório (NOT NULL)
[MaxLength(100)]     // Limite de caracteres
[Column("id_clinica")] // Mapeia para coluna específica
```

São como "etiquetas" que dão instruções ao Entity Framework.

---

### Tipos Anuláveis (Nullable Types)

```csharp
public string? Email { get; set; }   // ? significa "pode ser null"
public string Nome { get; set; }     // Sem ? = obrigatório
```

**Porquê usar?**
- `string?` = campo opcional (pode ser NULL na BD)
- `string` = campo obrigatório (NOT NULL na BD)

---

## 2. ESTRUTURA DO PROJETO

```
src/api/
├── Models/              ← Classes que representam tabelas SQL
│   ├── Cliente.cs
│   ├── Clinica.cs
│   ├── Funcionario.cs
│   ├── LoginModel.cs    ← Dados do formulário de login
│   └── AuthResponse.cs  ← Resposta do login
│
├── Context/             ← Conexão com a Base de Dados
│   └── ClinicaDbContext.cs
│
├── Controllers/         ← Endpoints da API (rotas HTTP)
│   └── AuthController.cs
│
└── Program.cs          ← Ficheiro principal (arranca tudo)
```

---

## 3. MODELS - As Classes que representam dados

### 📄 Cliente.cs - EXPLICAÇÃO LINHA POR LINHA

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ↑ Importa bibliotecas para usar [Key], [Table], etc.

namespace api.Models;
// ↑ Define que esta classe pertence ao namespace "api.Models"

[Table("clientes")]
// ↑ IMPORTANTE: Diz ao Entity Framework que esta classe mapeia
//   para a tabela SQL "clientes"

public class Cliente
// ↑ Define a classe pública "Cliente"
{
    [Key]
    // ↑ Marca esta propriedade como PRIMARY KEY
    public int Id { get; set; }
    // ↑ int = número inteiro
    // ↑ Id = nome da propriedade
    // ↑ { get; set; } = pode ler e escrever

    public string Nome { get; set; } = string.Empty;
    // ↑ string = texto
    // ↑ Sem ? = obrigatório (NOT NULL no SQL)
    // ↑ = string.Empty = inicia com "" (evita null)

    public string? Nif { get; set; }
    // ↑ string? = texto OPCIONAL (pode ser NULL)

    public string? Morada { get; set; }
    // ↑ Opcional também

    public string Telefone { get; set; } = string.Empty;
    // ↑ Obrigatório

    public string? Email { get; set; }
    // ↑ Opcional

    [Column("id_clinica")]
    // ↑ IMPORTANTE: A propriedade chama-se "ClinicaId" em C#,
    //   mas na BD a coluna chama-se "id_clinica"
    //   Esta anotação faz o mapeamento correto!
    public int ClinicaId { get; set; }
    // ↑ Foreign Key para a tabela clinicas
}
```

**PORQUÊ FIZEMOS ASSIM?**

1. **`[Table("clientes")]`** - Porque a tabela SQL chama-se "clientes" (plural, minúsculas)
2. **`[Column("id_clinica")]`** - Porque no SQL usas snake_case (id_clinica), mas em C# usamos PascalCase (ClinicaId)
3. **`string?` vs `string`** - Para respeitar exatamente o schema SQL (NULL vs NOT NULL)

---

### 📄 Clinica.cs - EXPLICAÇÃO

```csharp
[Table("clinicas")]
public class Clinica
{
    [Key]
    public int Id { get; set; }

    [Required]
    // ↑ Adiciona validação extra (NOT NULL)
    [MaxLength(100)]
    // ↑ Limita a 100 caracteres (igual ao SQL: VARCHAR(100))
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;
    // ↑ ATENÇÃO: Aqui vais guardar a password
    //   (agora em texto plano, depois com hash BCrypt)

    [Required]
    [MaxLength(8)]
    public string Cp { get; set; } = string.Empty;
    // ↑ Código Postal (formato: 1000-100)

    [MaxLength(9)]
    public string? Nif { get; set; }
    // ↑ Opcional (algumas clínicas podem não ter NIF)

    [Required]
    [MaxLength(25)]
    public string Iban { get; set; } = string.Empty;
}
```

**PORQUÊ as validações [Required] e [MaxLength]?**

1. **Segurança** - Evita que alguém envie textos gigantes
2. **Alinhamento com SQL** - VARCHAR(100) = MaxLength(100)
3. **Feedback ao utilizador** - Se enviar dados inválidos, recebe erro claro

---

### 📄 Funcionario.cs

```csharp
[Table("funcionarios")]
public class Funcionario
{
    [Key]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Especialidade { get; set; }
    // ↑ Opcional - nem todos funcionários têm especialidade
    //   (ex: rececionista não tem)

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public decimal Salario { get; set; }
    // ↑ decimal = número com casas decimais (para dinheiro)
    // ↑ IMPORTANTE: Usamos decimal (não float) para valores monetários
    //   porque float tem erros de arredondamento!
    
    [Column("id_clinica")]
    public int ClinicaId { get; set; }
    // ↑ Cada funcionário pertence a uma clínica
}
```

**PORQUÊ `decimal` e não `float`?**

```csharp
float errado = 0.1f + 0.2f;      // = 0.30000001 (ERRO!)
decimal certo = 0.1M + 0.2M;     // = 0.3 (CORRETO)
```

Para dinheiro, SEMPRE usa `decimal`!

---

### 📄 LoginModel.cs - Dados do Formulário

```csharp
namespace api.Models;

public class LoginModel
// ↑ Esta classe NÃO representa uma tabela SQL!
//   É só para receber dados do frontend
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

**PORQUÊ esta classe existe?**

Quando o utilizador faz login, envia JSON assim:
```json
{
  "email": "clinica@teste.pt",
  "password": "teste123"
}
```

O C# converte automaticamente esse JSON para um objeto `LoginModel`!

---

### 📄 AuthResponse.cs - Resposta do Login

```csharp
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
```

**PORQUÊ?**

Quando o login tem sucesso, devolvemos:
```json
{
  "token": "eyJhbGci...",
  "email": "clinica@teste.pt",
  "nome": "Clínica Veterinária"
}
```

Esta classe define a estrutura dessa resposta.

---

## 4. DbContext - A PONTE COM A BASE DE DADOS

### 📄 ClinicaDbContext.cs - EXPLICAÇÃO DETALHADA

```csharp
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context;

public class ClinicaDbContext : DbContext
// ↑ Herda de DbContext (classe do Entity Framework Core)
//   DbContext = gestor de conexão com a BD
{
    // DbSets = "representantes" das tabelas SQL
    public DbSet<Clinica> Clinicas { get; set; }
    // ↑ Este DbSet permite fazer queries à tabela "clinicas"
    //   Exemplo: _context.Clinicas.Where(c => c.Id == 1)
    
    public DbSet<Funcionario> Funcionarios { get; set; }
    // ↑ Acesso à tabela "funcionarios"
    
    public DbSet<Cliente> Clientes { get; set; }
    // ↑ Acesso à tabela "clientes"

    private readonly IConfiguration _config;
    // ↑ Guarda configurações (appsettings.json)
    //   readonly = só pode ser definido no construtor

    public ClinicaDbContext(
        DbContextOptions<ClinicaDbContext> options, 
        IConfiguration config
    ) : base(options)
    // ↑ CONSTRUTOR - executado quando criamos o objeto
    //   : base(options) = passa options para o DbContext pai
    {
        _config = config;
        // ↑ Guarda config para usar depois
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder
    )
    // ↑ Método que configura a conexão
    {
        if (!optionsBuilder.IsConfigured)
        // ↑ Só configura se ainda não foi configurado
        {
            var connectionString = _config
                .GetSection("DB")["ConnectionString"];
            // ↑ Vai buscar ao appsettings.json:
            //   "DB": { "ConnectionString": "Server=..." }
            
            optionsBuilder.UseNpgsql(connectionString);
            // ↑ Usa PostgreSQL (Npgsql = driver PostgreSQL)
        }
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    // ↑ Configurações adicionais das tabelas
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Clinica>()
            .HasIndex(c => c.Email)
            .IsUnique();
        // ↑ Cria índice ÚNICO no email da clínica
        //   (não permite emails duplicados)

        modelBuilder.Entity<Funcionario>()
            .HasIndex(f => f.Email)
            .IsUnique();
        // ↑ Mesmo para funcionários
    }
}
```

**COMO FUNCIONA NA PRÁTICA?**

```csharp
// 1. Injeção do DbContext no Controller
public AuthController(ClinicaDbContext context)
{
    _context = context;
}

// 2. Fazer query à BD
var clinica = _context.Clinicas
    .Where(c => c.Email == "clinica@teste.pt")
    .FirstOrDefault();
// ↑ Entity Framework converte isto em SQL:
//   SELECT * FROM clinicas WHERE email = 'clinica@teste.pt' LIMIT 1;
```

---

## 5. CONTROLLERS - Os ENDPOINTS da API

### 📄 AuthController.cs - EXPLICAÇÃO COMPLETA

```csharp
using Microsoft.AspNetCore.Mvc;
// ↑ Biblioteca para criar Controllers Web

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// ↑ Bibliotecas para criar JWT tokens

using api.Models;
using api.Context;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
// ↑ Marca esta classe como um Controller de API
//   Ativa validação automática, binding de JSON, etc.

[Route("api/[controller]")]
// ↑ Define a rota base: /api/auth
//   [controller] é substituído por "Auth" (nome da classe sem "Controller")
public class AuthController : ControllerBase
// ↑ Herda de ControllerBase (classe base para API controllers)
{
    private readonly IConfiguration _configuration;
    // ↑ Acesso às configurações (appsettings.json)
    
    private readonly ClinicaDbContext _context;
    // ↑ Acesso à base de dados

    public AuthController(
        IConfiguration configuration, 
        ClinicaDbContext context
    )
    // ↑ INJEÇÃO DE DEPENDÊNCIAS
    //   ASP.NET Core cria estes objetos automaticamente
    //   e passa-os para o construtor!
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login/clinica")]
    // ↑ Define um endpoint POST em /api/auth/login/clinica
    //   HttpPost = só aceita método POST
    //   "login/clinica" = rota adicional
    
    public async Task<IActionResult> LoginClinica(
        [FromBody] LoginModel model
    )
    // ↑ async = método assíncrono (não bloqueia)
    //   Task<IActionResult> = vai retornar uma resposta HTTP
    //   [FromBody] = lê o JSON do body do request
    {
        // MOCKUP DATA - Para testes sem BD
        if (model.Email == "clinica@teste.pt" && 
            model.Password == "teste123")
        {
            var token = GenerateJwtToken(
                1, 
                "clinica@teste.pt", 
                "Clínica Veterinária Teste", 
                "Clinica"
            );
            
            return Ok(new AuthResponse
            // ↑ Ok() = retorna HTTP 200 com JSON
            {
                Token = token,
                Email = "clinica@teste.pt",
                Nome = "Clínica Veterinária Teste"
            });
        }

        // Código real da BD (comentado por agora)
        /*
        var clinica = await _context.Clinicas
            .SingleOrDefaultAsync(
                c => c.Email == model.Email && 
                     c.Password == model.Password
            );
        // ↑ await = espera a query terminar
        //   SingleOrDefaultAsync = retorna 1 ou null
        //   Gera SQL: SELECT * FROM clinicas 
        //             WHERE email = ? AND password = ?
        
        if (clinica == null)
        {
            return Unauthorized(new { 
                message = "Email ou password inválidos" 
            });
            // ↑ Unauthorized() = retorna HTTP 401
        }

        var token = GenerateJwtToken(
            clinica.Id, 
            clinica.Email, 
            clinica.Nome, 
            "Clinica"
        );
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = clinica.Email,
            Nome = clinica.Nome
        });
        */
        
        return Unauthorized(new { 
            message = "Email ou password inválidos" 
        });
    }

    [HttpPost("login/funcionario")]
    public async Task<IActionResult> LoginFuncionario(
        [FromBody] LoginModel model
    )
    {
        // Similar ao login da clínica, mas:
        // 1. Procura na tabela funcionarios
        // 2. Adiciona ClinicaId ao token (importante!)
        
        if (model.Email == "vet@teste.pt" && 
            model.Password == "teste123")
        {
            var token = GenerateJwtToken(
                1, 
                "vet@teste.pt", 
                "Dr. João Silva", 
                "Funcionario", 
                1  // ← ClinicaId
            );
            
            return Ok(new AuthResponse
            {
                Token = token,
                Email = "vet@teste.pt",
                Nome = "Dr. João Silva"
            });
        }

        if (model.Email == "rececionista@teste.pt" && 
            model.Password == "teste123")
        {
            var token = GenerateJwtToken(
                2, 
                "rececionista@teste.pt", 
                "Maria Santos", 
                "Funcionario", 
                1
            );
            
            return Ok(new AuthResponse
            {
                Token = token,
                Email = "rececionista@teste.pt",
                Nome = "Maria Santos"
            });
        }

        return Unauthorized(new { 
            message = "Email ou password inválidos" 
        });
    }

    private string GenerateJwtToken(
        int userId, 
        string email, 
        string nome, 
        string role, 
        int? clinicaId = null
    )
    // ↑ int? = inteiro opcional
    //   clinicaId = null = valor padrão se não for passado
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        // ↑ Vai buscar a secção JwtSettings do appsettings.json
        
        var secretKey = jwtSettings["SecretKey"] ?? 
                        "ChaveSecretaSuperSegura...";
        // ↑ ?? = operador "null coalescing"
        //   Se SecretKey for null, usa o valor à direita
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        );
        // ↑ Converte a chave de texto para bytes
        
        var credentials = new SigningCredentials(
            key, 
            SecurityAlgorithms.HmacSha256
        );
        // ↑ Define como assinar o token (HMAC-SHA256)

        var claims = new List<Claim>
        // ↑ Claims = "afirmações" sobre o utilizador
        //   Vão dentro do token
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            // ↑ ID do utilizador
            
            new Claim(ClaimTypes.Email, email),
            // ↑ Email
            
            new Claim(ClaimTypes.Name, nome),
            // ↑ Nome completo
            
            new Claim(ClaimTypes.Role, role)
            // ↑ Role: "Clinica" ou "Funcionario"
        };

        if (clinicaId.HasValue)
        // ↑ Se foi passado um clinicaId
        {
            claims.Add(new Claim("ClinicaId", clinicaId.Value.ToString()));
            // ↑ Adiciona claim customizado com o ID da clínica
            //   IMPORTANTE para funcionários saberem de que clínica são!
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "ClinicaAPI",
            // ↑ Quem emitiu o token
            
            audience: jwtSettings["Audience"] ?? "ClinicaWebApp",
            // ↑ Para quem é o token
            
            claims: claims,
            // ↑ As afirmações
            
            expires: DateTime.UtcNow.AddHours(8),
            // ↑ Expira em 8 horas
            
            signingCredentials: credentials
            // ↑ Como assinar
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
        // ↑ Converte o token para string (formato JWT)
    }
}
```

**O QUE ACONTECE QUANDO FAZES UM REQUEST?**

1. Cliente envia POST para `/api/auth/login/clinica`:
   ```json
   {
     "email": "clinica@teste.pt",
     "password": "teste123"
   }
   ```

2. ASP.NET Core:
   - Converte JSON para objeto `LoginModel`
   - Chama `LoginClinica(model)`

3. Controller:
   - Verifica credenciais (mockup ou BD)
   - Se corretas: gera JWT token
   - Retorna HTTP 200 com AuthResponse

4. Cliente recebe:
   ```json
   {
     "token": "eyJhbGc...",
     "email": "clinica@teste.pt",
     "nome": "Clínica Veterinária Teste"
   }
   ```

---

## 6. Program.cs - O ARRANQUE DA APLICAÇÃO

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using api.Context;

var builder = WebApplication.CreateBuilder(args);
// ↑ Cria o "builder" - objeto que configura a aplicação

// ===== REGISTAR SERVIÇOS =====

builder.Services.AddControllers();
// ↑ Ativa os Controllers (API endpoints)

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ↑ Ativa Swagger (documentação automática da API)

// Configuração Database
var connectionString = builder.Configuration
    .GetSection("DB")["ConnectionString"];
// ↑ Lê a connection string do appsettings.json

builder.Services.AddDbContext<ClinicaDbContext>(options =>
    options.UseNpgsql(connectionString)
);
// ↑ REGISTA o DbContext
//   Quando um Controller pedir ClinicaDbContext,
//   ASP.NET Core cria um automaticamente!
//   Isto chama-se INJEÇÃO DE DEPENDÊNCIAS

// Configuração CORS (permitir requests do frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite dev server
                "http://localhost:3000"   // Alternativo
              )
              .AllowAnyHeader()    // Permite qualquer header
              .AllowAnyMethod()    // Permite GET, POST, etc.
              .AllowCredentials(); // Permite cookies
    });
});

// Configuração JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? 
                "ChaveSecretaSuperSegura...";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// ↑ Ativa autenticação JWT
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        // ↑ Verifica se o "issuer" do token é válido
        
        ValidateAudience = true,
        // ↑ Verifica se o "audience" é válido
        
        ValidateLifetime = true,
        // ↑ Verifica se o token expirou
        
        ValidateIssuerSigningKey = true,
        // ↑ Verifica a assinatura do token
        
        ValidIssuer = jwtSettings["Issuer"] ?? "ClinicaAPI",
        ValidAudience = jwtSettings["Audience"] ?? "ClinicaWebApp",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        )
    };
});

// ===== CONSTRUIR A APP =====

var app = builder.Build();
// ↑ Constrói a aplicação com todas as configurações

// ===== CONFIGURAR PIPELINE HTTP =====

if (app.Environment.IsDevelopment())
// ↑ Só ativa Swagger em desenvolvimento
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// ↑ Redireciona HTTP para HTTPS

app.UseCors("AllowVueApp");
// ↑ Ativa CORS (permite frontend aceder à API)

app.UseAuthentication();
// ↑ Ativa autenticação (valida JWT tokens)

app.UseAuthorization();
// ↑ Ativa autorização (verifica permissões)

app.MapControllers();
// ↑ Mapeia os Controllers para rotas HTTP

// ===== INICIAR SERVIDOR =====

app.Run();
// ↑ Inicia o servidor web (fica à escuta de requests)
```

**ORDEM É IMPORTANTE!**

O pipeline HTTP processa requests nesta ordem:
1. HTTPS Redirection
2. CORS
3. Authentication (valida token)
4. Authorization (verifica permissões)
5. Controllers (executa código)

---

## 7. COMO TUDO FUNCIONA JUNTO

### FLUXO COMPLETO DE UM LOGIN:

```
1. FRONTEND (Vue.js)
   ↓
   POST /api/auth/login/clinica
   Body: { "email": "...", "password": "..." }
   ↓
2. ASPNET CORE (Program.cs)
   ↓
   - Recebe request HTTP
   - Passa por CORS (permite?)
   - Converte JSON → LoginModel
   ↓
3. CONTROLLER (AuthController.cs)
   ↓
   - LoginClinica(model) é chamado
   - Verifica credenciais (mockup ou BD)
   ↓
4. DbContext (se usar BD real)
   ↓
   - Faz query: SELECT * FROM clinicas WHERE email = ?
   - Entity Framework converte resultado → objeto Clinica
   ↓
5. GERAR TOKEN
   ↓
   - GenerateJwtToken(...) cria JWT
   - Claims: id, email, nome, role
   ↓
6. RESPOSTA
   ↓
   - return Ok(new AuthResponse { ... })
   - ASP.NET Core converte para JSON
   ↓
7. FRONTEND recebe:
   {
     "token": "eyJhbGc...",
     "email": "clinica@teste.pt",
     "nome": "Clínica Veterinária"
   }
```

### EXEMPLO REAL PASSO A PASSO:

**Request:**
```bash
curl -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{"email":"clinica@teste.pt","password":"teste123"}'
```

**O que acontece:**

1. **ASP.NET Core** recebe o request
2. **CORS middleware** verifica se origem é permitida
3. **Routing** identifica rota: `AuthController.LoginClinica`
4. **Model Binding** converte JSON para `LoginModel`
5. **Dependency Injection** injeta `ClinicaDbContext` e `IConfiguration`
6. **Controller** executa:
   ```csharp
   if (model.Email == "clinica@teste.pt" && 
       model.Password == "teste123")
   {
       // Gera token
       var token = GenerateJwtToken(...);
       
       // Retorna resposta
       return Ok(new AuthResponse { ... });
   }
   ```
7. **Serialization** converte `AuthResponse` para JSON
8. **Response** HTTP 200 com JSON no body

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "clinica@teste.pt",
  "nome": "Clínica Veterinária Teste"
}
```

---

## DECISÕES DE DESIGN - PORQUÊ FIZEMOS ASSIM?

### 1. Porquê separar Clinica e Funcionario login?

**Duas rotas diferentes:**
- `/api/auth/login/clinica`
- `/api/auth/login/funcionario`

**Razões:**
- Clínica gere tudo (admin)
- Funcionário só faz operações do dia-a-dia
- Tokens diferentes (funcionário tem ClinicaId)
- Mais seguro (se queres mudar regras depois)

### 2. Porquê usar DbContext?

**Alternativa:** Escrever SQL manualmente
```csharp
// SEM Entity Framework (manual)
var sql = "SELECT * FROM clinicas WHERE email = @email";
var result = connection.Execute(sql, new { email });

// COM Entity Framework
var clinica = _context.Clinicas
    .Where(c => c.Email == email)
    .FirstOrDefault();
```

**Vantagens EF Core:**
- Menos SQL manual
- Type-safe (erros em compile time)
- Migrations automáticas
- Mapeamento automático de objetos

### 3. Porquê async/await?

```csharp
public async Task<IActionResult> LoginClinica(...)
{
    var clinica = await _context.Clinicas
        .SingleOrDefaultAsync(...);
}
```

**Sem async (bloqueante):**
- Request chega → thread espera BD → responde
- Se BD demora 2s, thread fica bloqueada 2s
- Limite de threads = API trava!

**Com async (não-bloqueante):**
- Request chega → thread faz query e **liberta**
- Thread atende outros requests
- Quando BD responde → thread volta e continua
- **Muito mais eficiente!**

### 4. Porquê IConfiguration e não hardcode?

```csharp
// MAU (hardcoded)
var connectionString = "Server=localhost;Database=cvsl;...";

// BOM (configurável)
var connectionString = _config.GetSection("DB")["ConnectionString"];
```

**Vantagens:**
- Diferentes configs para dev/staging/prod
- Não expões secrets no código
- Fácil mudar sem recompilar

### 5. Porquê Dependency Injection?

```csharp
// Sem DI (manual)
public class AuthController
{
    private ClinicaDbContext _context = new ClinicaDbContext(...);
}

// Com DI (automático)
public AuthController(ClinicaDbContext context)
{
    _context = context;
}
```

**Vantagens:**
- Testável (podes injetar mock em testes)
- ASP.NET Core gere lifetime (cria/destroi)
- Menos código repetido

---

## CONCEITOS-CHAVE RESUMIDOS

| Conceito | O que é | Porquê usar |
|----------|---------|-------------|
| **Model** | Classe que representa dados | Mapear tabelas SQL |
| **DbContext** | Gestor de BD | Fazer queries facilmente |
| **Controller** | Endpoints HTTP | Receber/responder requests |
| **Dependency Injection** | Objetos criados automaticamente | Menos código, testável |
| **async/await** | Código não-bloqueante | Performance |
| **JWT** | Token com claims | Autenticação stateless |
| **CORS** | Permitir requests de outros domínios | Frontend aceder API |
| **Data Annotations** | Metadados ([Key], [Required]) | Validação e mapeamento |

---

## PRÓXIMOS PASSOS SUGERIDOS

Para **dominar completamente** este código:

1. **Experimenta mudar coisas:**
   - Adiciona um campo novo ao Model
   - Cria um novo endpoint no Controller
   - Muda a duração do token JWT

2. **Debug:**
   - Coloca breakpoints no Visual Studio Code
   - Vê os valores das variáveis
   - Segue o fluxo step-by-step

3. **Lê logs:**
   - Quando a API corre, vê o console
   - Percebe cada request que entra

4. **Cria um novo Controller:**
   - Ex: `ClientesController`
   - Com endpoints GET/POST/PUT/DELETE

---

Tens alguma parte específica que queres que explique ainda mais detalhadamente?
