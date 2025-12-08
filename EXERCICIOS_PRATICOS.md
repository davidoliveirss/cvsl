# 💡 EXEMPLOS PRÁTICOS - Aprender Fazendo

## 🎓 EXERCÍCIOS PROGRESSIVOS

### NÍVEL 1: Entender o código existente

#### Exercício 1.1: Adicionar log
**Objetivo:** Ver o que acontece quando recebes um request

```csharp
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    // ADICIONA ISTO no início do método:
    Console.WriteLine($"📧 Tentativa de login: {model.Email}");
    
    if (model.Email == "clinica@teste.pt" && model.Password == "teste123")
    {
        Console.WriteLine("✅ Login bem-sucedido!");
        // ... resto do código
    }
    else
    {
        Console.WriteLine("❌ Credenciais inválidas");
    }
}
```

**Como testar:**
1. Reinicia a API
2. Faz login
3. Vê no terminal da API aparecer os logs!

---

#### Exercício 1.2: Mudar duração do token
**Objetivo:** Perceber como funciona a expiração

```csharp
private string GenerateJwtToken(...)
{
    // ... código existente ...
    
    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"] ?? "ClinicaAPI",
        audience: jwtSettings["Audience"] ?? "ClinicaWebApp",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),  // ← MUDA ISTO
        //                              ↑
        // Tenta: .AddMinutes(5)  = 5 minutos
        //        .AddDays(1)     = 1 dia
        //        .AddHours(24)   = 24 horas
        signingCredentials: credentials
    );
}
```

**Como testar:**
1. Muda para `.AddMinutes(1)`
2. Faz login
3. Copia o token
4. Cola em https://jwt.io
5. Vê o campo "exp" (timestamp de expiração)

---

### NÍVEL 2: Modificar funcionalidades

#### Exercício 2.1: Adicionar campo à resposta
**Objetivo:** Retornar mais informação no login

```csharp
// 1. Modifica AuthResponse.cs
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;  // ← NOVO!
}

// 2. Modifica o Controller
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    if (model.Email == "clinica@teste.pt" && model.Password == "teste123")
    {
        var token = GenerateJwtToken(1, "clinica@teste.pt", 
                                     "Clínica Veterinária Teste", "Clinica");
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = "clinica@teste.pt",
            Nome = "Clínica Veterinária Teste",
            Role = "Clinica"  // ← ADICIONA ISTO
        });
    }
    
    return Unauthorized(new { message = "Email ou password inválidos" });
}
```

**Como testar:**
```bash
curl -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{"email":"clinica@teste.pt","password":"teste123"}'
```

Agora a resposta terá:
```json
{
  "token": "...",
  "email": "clinica@teste.pt",
  "nome": "Clínica Veterinária Teste",
  "role": "Clinica"  ← NOVO!
}
```

---

#### Exercício 2.2: Adicionar nova credencial mockup
**Objetivo:** Criar outro utilizador de teste

```csharp
[HttpPost("login/funcionario")]
public async Task<IActionResult> LoginFuncionario([FromBody] LoginModel model)
{
    // ... código existente para vet@ e rececionista@ ...
    
    // ADICIONA ISTO:
    if (model.Email == "enfermeiro@teste.pt" && model.Password == "teste123")
    {
        var token = GenerateJwtToken(3, "enfermeiro@teste.pt", 
                                     "Carlos Martins", "Funcionario", 1);
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = "enfermeiro@teste.pt",
            Nome = "Carlos Martins"
        });
    }

    return Unauthorized(new { message = "Email ou password inválidos" });
}
```

**Como testar:**
```bash
curl -X POST http://localhost:5000/api/auth/login/funcionario \
  -H "Content-Type: application/json" \
  -d '{"email":"enfermeiro@teste.pt","password":"teste123"}'
```

---

### NÍVEL 3: Criar novos endpoints

#### Exercício 3.1: Endpoint "Quem Sou Eu?"
**Objetivo:** Criar endpoint que devolve dados do utilizador logado

```csharp
// Em AuthController.cs, ADICIONA:

[HttpGet("me")]
[Authorize]  // ← Requer autenticação (token válido)
public IActionResult GetCurrentUser()
{
    // Extrai claims do token
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    var nome = User.FindFirst(ClaimTypes.Name)?.Value;
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    
    // Tenta obter ClinicaId (só funcionários têm)
    var clinicaId = User.FindFirst("ClinicaId")?.Value;
    
    return Ok(new
    {
        Id = userId,
        Email = email,
        Nome = nome,
        Role = role,
        ClinicaId = clinicaId
    });
}
```

**Como testar:**
```bash
# 1. Faz login e guarda o token
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{"email":"clinica@teste.pt","password":"teste123"}' \
  | jq -r '.token')

# 2. Usa o token para aceder ao endpoint protegido
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer $TOKEN"
```

Resposta:
```json
{
  "id": "1",
  "email": "clinica@teste.pt",
  "nome": "Clínica Veterinária Teste",
  "role": "Clinica",
  "clinicaId": null
}
```

---

#### Exercício 3.2: Validar password forte
**Objetivo:** Adicionar validação customizada

```csharp
// Cria novo ficheiro: Helpers/PasswordValidator.cs
namespace api.Helpers;

public static class PasswordValidator
{
    public static bool IsValid(string password)
    {
        if (password.Length < 8)
            return false;
            
        if (!password.Any(char.IsUpper))
            return false;  // Precisa de maiúscula
            
        if (!password.Any(char.IsDigit))
            return false;  // Precisa de número
            
        return true;
    }
    
    public static string GetErrorMessage()
    {
        return "Password deve ter mínimo 8 caracteres, " +
               "1 maiúscula e 1 número";
    }
}

// No Controller, usa assim:
[HttpPost("register")]
public IActionResult Register([FromBody] RegisterModel model)
{
    if (!PasswordValidator.IsValid(model.Password))
    {
        return BadRequest(new { 
            message = PasswordValidator.GetErrorMessage() 
        });
    }
    
    // ... resto do código de registo ...
}
```

---

### NÍVEL 4: Trabalhar com a Base de Dados

#### Exercício 4.1: Ativar login com BD
**Objetivo:** Usar dados reais em vez de mockup

**PASSO 1:** Insere dados de teste na BD
```sql
-- No psql ou pgAdmin:
INSERT INTO clinicas (nome, email, password, cp, nif, iban)
VALUES ('Clínica Teste Real', 'real@clinica.pt', 'senha123', 
        '1000-100', '123456789', 'PT50000000000000000000001');
```

**PASSO 2:** Modifica o Controller
```csharp
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    // COMENTA o código mockup
    /*
    if (model.Email == "clinica@teste.pt" && model.Password == "teste123")
    {
        // ...
    }
    */
    
    // DESCOMENTA isto:
    var clinica = await _context.Clinicas
        .SingleOrDefaultAsync(c => c.Email == model.Email && 
                                   c.Password == model.Password);
    
    if (clinica == null)
    {
        return Unauthorized(new { message = "Email ou password inválidos" });
    }

    var token = GenerateJwtToken(clinica.Id, clinica.Email, 
                                 clinica.Nome, "Clinica");
    
    return Ok(new AuthResponse
    {
        Token = token,
        Email = clinica.Email,
        Nome = clinica.Nome
    });
}
```

**PASSO 3:** Testa
```bash
curl -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{"email":"real@clinica.pt","password":"senha123"}'
```

---

#### Exercício 4.2: Ver queries SQL geradas
**Objetivo:** Perceber o que EF Core faz

```csharp
// No ClinicaDbContext.cs, adiciona:
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = _config.GetSection("DB")["ConnectionString"];
        optionsBuilder
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information);  // ← ADICIONA ISTO
            //     ↑ Mostra SQL no console!
    }
}
```

Agora quando fizer login, verás no console:
```sql
info: 12/08/2024 14:15:38.665 RelationalEventId.CommandExecuting[20100]
      Executing DbCommand [Parameters=[@__model_Email_0='?' (Size = 100), 
      @__model_Password_1='?' (Size = 255)], CommandType='Text', ...]
      SELECT c.id, c.nome, c.email, c.password, c.cp, c.nif, c.iban
      FROM clinicas AS c
      WHERE (c.email = @__model_Email_0) AND (c.password = @__model_Password_1)
      LIMIT 1
```

---

### NÍVEL 5: Criar Controller completo

#### Exercício 5.1: ClientesController (CRUD básico)

```csharp
// Cria ficheiro: Controllers/ClientesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Context;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // Todos endpoints requerem autenticação
public class ClientesController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public ClientesController(ClinicaDbContext context)
    {
        _context = context;
    }

    // GET /api/clientes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes);
    }

    // GET /api/clientes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        
        if (cliente == null)
            return NotFound(new { message = "Cliente não encontrado" });
        
        return Ok(cliente);
    }

    // POST /api/clientes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cliente cliente)
    {
        // Validações
        if (string.IsNullOrWhiteSpace(cliente.Nome))
            return BadRequest(new { message = "Nome é obrigatório" });
        
        if (string.IsNullOrWhiteSpace(cliente.Telefone))
            return BadRequest(new { message = "Telefone é obrigatório" });

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        //     ↑ Retorna HTTP 201 Created com Location header
    }

    // PUT /api/clientes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var existente = await _context.Clientes.FindAsync(id);
        if (existente == null)
            return NotFound(new { message = "Cliente não encontrado" });

        // Atualiza campos
        existente.Nome = cliente.Nome;
        existente.Nif = cliente.Nif;
        existente.Morada = cliente.Morada;
        existente.Telefone = cliente.Telefone;
        existente.Email = cliente.Email;

        await _context.SaveChangesAsync();
        
        return Ok(existente);
    }

    // DELETE /api/clientes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        
        if (cliente == null)
            return NotFound(new { message = "Cliente não encontrado" });

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        
        return NoContent();  // HTTP 204 No Content
    }
}
```

**Como testar:**

```bash
# GET todos clientes (precisa de token!)
TOKEN="..." # token do login
curl -X GET http://localhost:5000/api/clientes \
  -H "Authorization: Bearer $TOKEN"

# POST criar cliente
curl -X POST http://localhost:5000/api/clientes \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "telefone": "912345678",
    "email": "joao@email.pt",
    "clinicaId": 1
  }'

# GET cliente específico
curl -X GET http://localhost:5000/api/clientes/1 \
  -H "Authorization: Bearer $TOKEN"

# PUT atualizar cliente
curl -X PUT http://localhost:5000/api/clientes/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "nome": "João Silva Atualizado",
    "telefone": "912345678",
    "clinicaId": 1
  }'

# DELETE remover cliente
curl -X DELETE http://localhost:5000/api/clientes/1 \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🐛 DEBUGGING - Como Encontrar Erros

### Técnica 1: Console.WriteLine (mais simples)

```csharp
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    Console.WriteLine("=== INÍCIO DO LOGIN ===");
    Console.WriteLine($"Email recebido: {model.Email}");
    Console.WriteLine($"Password recebido: {model.Password}");
    
    var clinica = await _context.Clinicas
        .SingleOrDefaultAsync(c => c.Email == model.Email);
    
    Console.WriteLine($"Clinica encontrada: {clinica != null}");
    
    if (clinica == null)
    {
        Console.WriteLine("ERRO: Clinica não encontrada!");
        return Unauthorized(...);
    }
    
    Console.WriteLine($"Password na BD: {clinica.Password}");
    Console.WriteLine($"Password match: {clinica.Password == model.Password}");
    
    // ... resto
}
```

### Técnica 2: Try-Catch para erros

```csharp
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    try
    {
        var clinica = await _context.Clinicas
            .SingleOrDefaultAsync(c => c.Email == model.Email);
        
        // ... resto do código ...
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERRO: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        return StatusCode(500, new { 
            message = "Erro interno do servidor",
            error = ex.Message
        });
    }
}
```

### Técnica 3: Verificar valores nulos

```csharp
[HttpPost("login/clinica")]
public async Task<IActionResult> LoginClinica([FromBody] LoginModel model)
{
    // Validações defensivas
    if (model == null)
        return BadRequest(new { message = "Dados inválidos" });
    
    if (string.IsNullOrWhiteSpace(model.Email))
        return BadRequest(new { message = "Email é obrigatório" });
    
    if (string.IsNullOrWhiteSpace(model.Password))
        return BadRequest(new { message = "Password é obrigatória" });
    
    // Agora sim, continua...
}
```

---

## 🎯 CHECKLIST - Entendi tudo?

Marca ✅ quando conseguires fazer sem ajuda:

### Básico
- [ ] Explicar o que é uma classe
- [ ] Explicar o que é uma propriedade { get; set; }
- [ ] Explicar a diferença entre `string` e `string?`
- [ ] Explicar o que faz `[Table("clientes")]`
- [ ] Explicar o que faz `[Column("id_clinica")]`

### Models & DbContext
- [ ] Criar um novo Model do zero
- [ ] Adicionar um DbSet ao ClinicaDbContext
- [ ] Fazer uma query simples com LINQ
- [ ] Entender a diferença entre Model (classe) e DbSet (coleção)

### Controllers
- [ ] Criar um endpoint GET simples
- [ ] Criar um endpoint POST que recebe JSON
- [ ] Retornar Ok(objeto), BadRequest(), NotFound()
- [ ] Usar [FromBody] para receber dados
- [ ] Usar [Authorize] para proteger endpoint

### JWT
- [ ] Explicar as 3 partes de um token JWT
- [ ] Ver claims dentro de um token
- [ ] Usar o token num request autenticado
- [ ] Explicar porque usamos JWT em vez de sessions

### Async/Await
- [ ] Explicar porque async é melhor que sync
- [ ] Usar await corretamente
- [ ] Saber quando usar Task<T>

### Dependency Injection
- [ ] Explicar como funciona DI
- [ ] Registar um serviço no Program.cs
- [ ] Injetar um serviço no construtor

---

## 📚 PRÓXIMOS PASSOS

Depois de dominares tudo isto:

1. **Implementa hash de passwords com BCrypt**
2. **Cria Models para as outras tabelas** (Animal, Consulta, etc)
3. **Cria Controllers CRUD** para cada entidade
4. **Adiciona validações customizadas**
5. **Implementa paginação** nos endpoints GET
6. **Adiciona filtros de pesquisa**
7. **Implementa refresh tokens**

---

Experimenta fazer os exercícios! A melhor forma de aprender é fazendo. 
Qualquer dúvida, pergunta! 🚀
