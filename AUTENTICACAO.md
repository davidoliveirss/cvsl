# Sistema de Autenticação - Centro Veterinário S.Lourenço

## ✅ Implementação Concluída

### Backend (C# API)
- ✅ JWT Authentication configurado
- ✅ AuthController com endpoint `/api/auth/login`
- ✅ CORS configurado para Vue.js
- ✅ Modelos: LoginModel, User, AuthResponse

### Frontend (Vue.js + Quasar)
- ✅ Auth Service (com Fetch API nativo)
- ✅ Auth Store (Pinia)
- ✅ LoginView atualizado
- ✅ Router com proteção de rotas
- ✅ Botão de Logout na página de conta

## 🚀 Como Testar

### 1. Iniciar a API (Backend)
```bash
cd /home/david/dev/cvsl/src/api
dotnet restore  # Apenas primeira vez
dotnet run
```

**Nota a URL da API** (exemplo: `https://localhost:5001` ou `http://localhost:5000`)

### 2. Configurar URL da API no Frontend

Editar `/home/david/dev/cvsl/src/web/src/services/authService.ts`:
```typescript
const API_URL = 'http://localhost:5000/api'; // Ajustar para a porta correta
```

### 3. Iniciar o Frontend (Vue.js)
```bash
cd /home/david/dev/cvsl/src/web
npm run dev
```

Aceder a `http://localhost:5173`

### 4. Fazer Login

**Credenciais de Teste:**

**Administrador:**
- Email: `admin@clinica.pt`
- Password: `admin123`

**Utilizador Normal:**
- Email: `user@clinica.pt`
- Password: `user123`

## 📋 Funcionalidades

### ✅ Autenticação
- Login com email e password
- Token JWT (válido por 8 horas)
- Armazenamento no localStorage
- Proteção automática de rotas

### ✅ Segurança
- Rotas protegidas (redirect para /login se não autenticado)
- Logout automático em caso de token expirado (401)
- CORS configurado
- JWT com claims (id, email, nome, role)

### ✅ UX
- Mensagens de erro/sucesso com Quasar Notify
- Loading spinner durante login
- Enter key para submeter formulário
- Redirect automático após login

## 🔧 Próximos Passos

### Base de Dados
Substituir utilizadores de teste em `AuthController.cs`:
```csharp
private User? ValidateUser(string email, string password)
{
    // TODO: Query à base de dados
    // Exemplo com Entity Framework:
    // var user = _context.Users.FirstOrDefault(u => u.Email == email);
    // if (user != null && BCrypt.Verify(password, user.PasswordHash))
    //     return user;
    // return null;
}
```

### Hash de Passwords
Adicionar pacote BCrypt:
```bash
dotnet add package BCrypt.Net-Next
```

### Gestão de Utilizadores (Admin)
- Criar endpoint para administradores registarem novos utilizadores
- View de gestão de utilizadores
- Roles e permissões

### Refresh Token
- Implementar refresh token para renovar sessões
- Evitar logout forçado após expiração

## 🐛 Troubleshooting

### CORS Error
Verificar que a API está a permitir o origin do Vue:
```csharp
// Program.cs
policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
```

### 401 Unauthorized
- Verificar que o token está a ser enviado no header
- Verificar se a SecretKey é a mesma no appsettings.json
- Verificar se o token não expirou

### Redirect Loop
- Verificar se a rota `/login` não tem `requiresAuth: true`
