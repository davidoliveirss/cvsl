# 🎨 DIAGRAMAS VISUAIS - Arquitetura da API

## 📊 ESTRUTURA GERAL DO PROJETO

```
┌─────────────────────────────────────────────────────────────┐
│                      FRONTEND (Vue.js)                       │
│                   http://localhost:5173                      │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTP Request (JSON)
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    ASP.NET Core API                          │
│                   http://localhost:5000                      │
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │              Program.cs (Bootstrap)                 │    │
│  │  • Configura serviços                              │    │
│  │  • Regista DbContext                               │    │
│  │  • Configura JWT, CORS                             │    │
│  └────────────────────┬───────────────────────────────┘    │
│                       │                                      │
│                       ▼                                      │
│  ┌────────────────────────────────────────────────────┐    │
│  │          HTTP Pipeline (Middlewares)                │    │
│  │  1. HTTPS Redirect                                 │    │
│  │  2. CORS ← Permite frontend aceder                │    │
│  │  3. Authentication ← Valida JWT                    │    │
│  │  4. Authorization ← Verifica permissões            │    │
│  │  5. Routing → Encontra Controller                 │    │
│  └────────────────────┬───────────────────────────────┘    │
│                       │                                      │
│                       ▼                                      │
│  ┌────────────────────────────────────────────────────┐    │
│  │            Controllers/ (Endpoints)                 │    │
│  │  ┌──────────────────────────────────────────┐     │    │
│  │  │      AuthController.cs                    │     │    │
│  │  │  • POST /api/auth/login/clinica          │     │    │
│  │  │  • POST /api/auth/login/funcionario      │     │    │
│  │  └──────────────────┬───────────────────────┘     │    │
│  └────────────────────┼───────────────────────────────┘    │
│                       │                                      │
│                       ▼                                      │
│  ┌────────────────────────────────────────────────────┐    │
│  │         Context/ClinicaDbContext.cs                 │    │
│  │  • DbSet<Clinica> Clinicas                         │    │
│  │  • DbSet<Funcionario> Funcionarios                 │    │
│  │  • DbSet<Cliente> Clientes                         │    │
│  └────────────────────┬───────────────────────────────┘    │
│                       │ Entity Framework Core                │
└───────────────────────┼─────────────────────────────────────┘
                        │ SQL Queries
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                  PostgreSQL Database                         │
│                    localhost:5432                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   clinicas   │  │ funcionarios │  │   clientes   │     │
│  │──────────────│  │──────────────│  │──────────────│     │
│  │ id           │  │ id           │  │ id           │     │
│  │ nome         │  │ nome         │  │ nome         │     │
│  │ email        │  │ email        │  │ nif          │     │
│  │ password     │  │ password     │  │ morada       │     │
│  │ cp           │  │ especialidade│  │ telefone     │     │
│  │ nif          │  │ telefone     │  │ email        │     │
│  │ iban         │  │ salario      │  │ id_clinica   │     │
│  └──────────────┘  │ id_clinica   │  └──────────────┘     │
│                     └──────────────┘                        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 FLUXO DE UM REQUEST DE LOGIN

```
┌─────────────┐
│  FRONTEND   │  1. User clica "Login"
│   (Vue.js)  │
└──────┬──────┘
       │ fetch('/api/auth/login/clinica', {
       │   method: 'POST',
       │   body: JSON.stringify({
       │     email: "clinica@teste.pt",
       │     password: "teste123"
       │   })
       │ })
       ▼
┌──────────────────────────────────────────────┐
│  2. ASP.NET Core recebe HTTP Request         │
│     POST /api/auth/login/clinica              │
│     Content-Type: application/json            │
│     Body: {"email":"...","password":"..."}    │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  3. CORS Middleware                           │
│     ✓ Origem permitida?                      │
│     ✓ localhost:5173 → SIM                   │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  4. Model Binding                             │
│     JSON → LoginModel object                  │
│     model.Email = "clinica@teste.pt"         │
│     model.Password = "teste123"              │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  5. Dependency Injection                      │
│     Cria/injeta:                             │
│     • IConfiguration                         │
│     • ClinicaDbContext                       │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  6. AuthController.LoginClinica(model)        │
│                                               │
│     if (model.Email == "clinica@teste.pt")   │
│     {                                         │
│         // MOCKUP: retorna sucesso           │
│     }                                         │
│                                               │
│     OU (quando usar BD real):                │
│                                               │
│     var clinica = await _context.Clinicas    │
│         .SingleOrDefaultAsync(               │
│             c => c.Email == model.Email &&   │
│                  c.Password == model.Password│
│         );                                    │
└──────┬───────────────────────────────────────┘
       │
       │ (se usar BD)
       ▼
┌──────────────────────────────────────────────┐
│  7. Entity Framework Core                     │
│     Converte LINQ para SQL:                  │
│                                               │
│     SELECT * FROM clinicas                   │
│     WHERE email = 'clinica@teste.pt'         │
│       AND password = 'teste123'              │
│     LIMIT 1;                                 │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  8. PostgreSQL executa query                  │
│     Retorna row ou NULL                      │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  9. EF Core converte resultado                │
│     Row → objeto Clinica                     │
│     {                                         │
│       Id = 1,                                │
│       Nome = "Clínica Veterinária",          │
│       Email = "clinica@teste.pt",            │
│       ...                                    │
│     }                                         │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  10. Controller valida                        │
│      if (clinica == null)                    │
│          return Unauthorized();              │
│                                               │
│      ✓ Clinica encontrada!                   │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  11. GenerateJwtToken()                       │
│                                               │
│      Claims:                                 │
│      • NameIdentifier: "1"                   │
│      • Email: "clinica@teste.pt"             │
│      • Name: "Clínica Veterinária"           │
│      • Role: "Clinica"                       │
│                                               │
│      Assina com SecretKey                    │
│      Define expiração: 8 horas               │
│                                               │
│      Token: "eyJhbGciOiJIUzI1NiIsInR5..."   │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  12. return Ok(new AuthResponse {...})        │
│                                               │
│      Cria objeto:                            │
│      {                                        │
│        Token = "eyJhbGc...",                 │
│        Email = "clinica@teste.pt",           │
│        Nome = "Clínica Veterinária"          │
│      }                                        │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  13. JSON Serialization                       │
│      C# object → JSON string                 │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│  14. HTTP Response                            │
│      Status: 200 OK                          │
│      Content-Type: application/json          │
│      Body:                                    │
│      {                                        │
│        "token": "eyJhbGc...",                │
│        "email": "clinica@teste.pt",          │
│        "nome": "Clínica Veterinária Teste"   │
│      }                                        │
└──────┬───────────────────────────────────────┘
       │
       ▼
┌──────────────┐
│   FRONTEND   │  15. Recebe resposta
│   localStorage.setItem('token', data.token)
│   Redireciona para dashboard
└──────────────┘
```

---

## 🗂️ DEPENDENCY INJECTION - Como Funciona

```
┌─────────────────────────────────────────────────────────────┐
│                    Program.cs                                │
│                                                              │
│  // REGISTO de serviços                                     │
│  builder.Services.AddDbContext<ClinicaDbContext>();         │
│                                                              │
│  Diz ao ASP.NET Core:                                       │
│  "Quando alguém pedir ClinicaDbContext,                     │
│   cria um novo e injeta automaticamente"                    │
└─────────────────────────┬───────────────────────────────────┘
                          │
                          │ Request chega
                          ▼
┌─────────────────────────────────────────────────────────────┐
│              AuthController é criado                         │
│                                                              │
│  public AuthController(                                     │
│      IConfiguration configuration,  ← ASP.NET injeta        │
│      ClinicaDbContext context       ← ASP.NET injeta        │
│  )                                                           │
│  {                                                           │
│      _configuration = configuration;                        │
│      _context = context;                                    │
│  }                                                           │
│                                                              │
│  TU NÃO ESCREVES:                                           │
│  ❌ var context = new ClinicaDbContext(...)                 │
│                                                              │
│  ASP.NET CORE FAZ AUTOMATICAMENTE! ✓                        │
└─────────────────────────────────────────────────────────────┘
```

**VANTAGENS:**
1. Menos código repetido
2. Fácil trocar implementações (útil para testes)
3. ASP.NET gere o lifetime (cria quando precisa, destroi quando acaba)

---

## 🔐 JWT TOKEN - Estrutura

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bW...
└────────┬───────────────────────┘ └────────┬──────────────────────...
       HEADER                              PAYLOAD
         │                                    │
         │                                    │
         ▼                                    ▼
    ┌────────────┐                    ┌──────────────────┐
    │  Header    │                    │    Payload       │
    │  {         │                    │    {             │
    │   "alg":   │                    │     "nameid": 1, │
    │   "HS256", │                    │     "email": "..│
    │   "typ":   │                    │     "name": "..."│
    │   "JWT"    │                    │     "role": "..." │
    │  }         │                    │     "exp": 12345 │
    └────────────┘                    │    }             │
                                      └──────────────────┘
                                               │
                           ....K7WpR2_DhC1iG3oFixmvI5gL4gYOa
                           └────────┬───────────────────┘
                                 SIGNATURE
                                    │
                                    ▼
                           ┌─────────────────────┐
                           │   Assinado com:     │
                           │   SecretKey         │
                           │   (HMAC-SHA256)     │
                           └─────────────────────┘
```

**Como o token é validado:**

```
Frontend envia request:
  Authorization: Bearer eyJhbGciOiJIUzI1...
         │
         ▼
┌────────────────────────────────────────┐
│  ASP.NET Core Authentication           │
│                                         │
│  1. Extrai token do header             │
│  2. Separa em 3 partes                 │
│  3. Verifica assinatura com SecretKey  │
│  4. Verifica expiração                 │
│  5. Verifica issuer/audience           │
│                                         │
│  ✓ Token válido!                       │
│  → Extrai claims                       │
│  → User.Identity.Name = "Clínica..."  │
│  → User.IsInRole("Clinica") = true    │
└────────────────────────────────────────┘
```

---

## 🔄 ASYNC/AWAIT - Diferença Visual

### ❌ SEM async (BLOQUEANTE):

```
Thread 1: [Request A]──────┐
                            │ Espera BD (2s)
                            │ ████████████
                            │ (thread BLOQUEADA)
                            ▼
                        [Responde A]

Thread 2: [Request B]──────┐
                            │ Espera BD (2s)
                            │ ████████████
                            ▼
                        [Responde B]

Threads disponíveis: 2
Requests atendidos: 2 por vez
Se chegarem 3 requests: 3º espera!
```

### ✅ COM async (NÃO-BLOQUEANTE):

```
Thread 1: [Request A]──┐
                       │ Lança query async
                       │ Thread LIBERTA
                       ▼
         [Request C]───────── (usa mesma thread!)
         [Request E]───────── (usa mesma thread!)
                       
         BD responde A ───┐
                          ▼
                       [Responde A]

Thread 2: [Request B]──┐
                       │ Lança query async
                       ▼ Thread LIBERTA
         [Request D]───────── (usa mesma thread!)
         
         BD responde B ───┐
                          ▼
                       [Responde B]

Threads disponíveis: 2
Requests atendidos: MUITOS simultaneamente!
Threads nunca bloqueiam esperando
```

**RESUMO:**
- `async/await` = thread não espera, vai fazer outras coisas
- Muito mais eficiente para I/O (BD, API externa, ficheiros)

---

## 📦 MODEL vs DbSet vs Table - Relação

```
┌─────────────────────────────────────────────────────────────┐
│                  CÓDIGO C#                                   │
│                                                              │
│  ┌──────────────────────────┐                              │
│  │  Model (Cliente.cs)      │                              │
│  │  ─────────────────────   │                              │
│  │  public class Cliente    │  ← Representa UMA linha      │
│  │  {                       │                               │
│  │    public int Id         │                               │
│  │    public string Nome    │                               │
│  │    ...                   │                               │
│  │  }                       │                               │
│  └──────────────────────────┘                              │
│               ▲                                              │
│               │ Mapeia para                                 │
│               │                                              │
│  ┌──────────────────────────┐                              │
│  │  DbSet (em DbContext)    │                              │
│  │  ─────────────────────   │                              │
│  │  public DbSet<Cliente>   │  ← Representa COLEÇÃO        │
│  │    Clientes { get; set; }│     de linhas (tabela)       │
│  │                          │                               │
│  │  Permite fazer:          │                               │
│  │  _context.Clientes       │                               │
│  │    .Where(...)           │                               │
│  │    .ToList()             │                               │
│  └──────────────────────────┘                              │
│               │                                              │
└───────────────┼─────────────────────────────────────────────┘
                │ Entity Framework converte para SQL
                ▼
┌─────────────────────────────────────────────────────────────┐
│                  POSTGRESQL                                  │
│                                                              │
│  ┌──────────────────────────┐                              │
│  │  Table: clientes         │                              │
│  │  ──────────────────────  │                              │
│  │  id  │ nome   │ nif     │                              │
│  │  ────┼────────┼─────────│                              │
│  │  1   │ João   │ 123...  │  ← Row                       │
│  │  2   │ Maria  │ 456...  │  ← Row                       │
│  │  3   │ Pedro  │ 789...  │  ← Row                       │
│  └──────────────────────────┘                              │
└─────────────────────────────────────────────────────────────┘
```

**EXEMPLO PRÁTICO:**

```csharp
// Buscar um cliente (retorna 1 objeto Cliente)
Cliente cliente = _context.Clientes
    .Where(c => c.Id == 1)
    .FirstOrDefault();

// Buscar todos clientes (retorna List<Cliente>)
List<Cliente> clientes = _context.Clientes
    .Where(c => c.Nome.StartsWith("J"))
    .ToList();

// Adicionar novo cliente
Cliente novo = new Cliente 
{ 
    Nome = "Ana", 
    Telefone = "912345678" 
};
_context.Clientes.Add(novo);
_context.SaveChanges();  // Executa INSERT
```

**Entity Framework converte para SQL:**

```sql
-- Where + FirstOrDefault
SELECT * FROM clientes WHERE id = 1 LIMIT 1;

-- Where + ToList
SELECT * FROM clientes WHERE nome LIKE 'J%';

-- Add + SaveChanges
INSERT INTO clientes (nome, telefone, ...) 
VALUES ('Ana', '912345678', ...);
```

---

## ⚙️ appsettings.json - Configuração

```json
{
  "JwtSettings": {
    "SecretKey": "ChaveSecreta...",
    "Issuer": "ClinicaAPI",
    "Audience": "ClinicaWebApp"
  },
  "DB": {
    "ConnectionString": "Server=localhost;Database=cvsl;..."
  }
}
```

**Como é usado no código:**

```csharp
// Em qualquer classe com IConfiguration injetado:

var secretKey = _configuration
    .GetSection("JwtSettings")["SecretKey"];
// → "ChaveSecreta..."

var connString = _configuration
    .GetSection("DB")["ConnectionString"];
// → "Server=localhost;..."
```

**PORQUÊ usar ficheiro de config?**
- Diferentes ambientes (dev/prod) = diferentes configs
- Não expõe secrets no código
- Fácil alterar sem recompilar

---

Agora compreendes melhor como tudo funciona? Queres que explique alguma parte específica ainda mais detalhadamente?
