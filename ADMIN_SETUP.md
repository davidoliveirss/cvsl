# 🔐 Sistema de Administração - Guia Completo

## 📋 Visão Geral

Implementação do sistema de login e gestão para **administradores da aplicação**, separado dos logins de clínicas e funcionários.

### Hierarquia de Utilizadores

```
┌─────────────────────────────────────────┐
│  ADMIN (tabela admins)                  │
│  - Acesso global a todas as clínicas    │
│  - CRUD de clínicas                     │
│  - Estatísticas globais                 │
│  - Gestão de admins                     │
└─────────────────────────────────────────┘
              ▲
              │
┌─────────────┴───────────────────────────┐
│  CLÍNICA (tabela clinicas)              │
│  - Acesso apenas à sua clínica          │
│  - Gestão de funcionários/clientes      │
│  - Dados da sua clínica                 │
└─────────────────────────────────────────┘
              ▲
              │
┌─────────────┴───────────────────────────┐
│  FUNCIONÁRIO (tabela funcionarios)      │
│  - Acesso à clínica onde trabalha       │
│  - Operações do dia-a-dia               │
└─────────────────────────────────────────┘
```

---

## 🗄️ 1. Base de Dados

### Criar Tabela

```bash
psql -U postgres -d cvsl_db -f sql/schema/admins.sql
```

### Estrutura da Tabela `admins`

```sql
CREATE TABLE admins (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    nivel VARCHAR(20) DEFAULT 'admin' CHECK (nivel IN ('super_admin', 'admin')),
    ativo BOOLEAN DEFAULT TRUE
);
```

### Gerar Hash de Password

**Opção 1: Usar C# (Recomendado)**

```bash
cd src/api
dotnet run --project GenerateHash.cs admin123
```

**Opção 2: Online BCrypt Generator**
- Ir a: https://bcrypt-generator.com/
- Rounds: 11
- Copiar o hash gerado

### Inserir Admin Inicial

```sql
INSERT INTO admins (nome, email, password, nivel) VALUES
('Admin Principal', 'admin@cvsl.pt', '$2a$11$HASH_AQUI', 'super_admin');
```

**IMPORTANTE**: Substituir `$2a$11$HASH_AQUI` pelo hash real gerado!

---

## 🔌 2. API Backend

### Endpoints Criados

#### Autenticação
```http
POST /api/auth/login/admin
Content-Type: application/json

{
  "email": "admin@cvsl.pt",
  "password": "admin123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGc...",
  "email": "admin@cvsl.pt",
  "nome": "Admin Principal"
}
```

#### Gestão de Clínicas (Requer autenticação Admin)

```http
# Listar todas as clínicas (paginado)
GET /api/admin/clinicas?page=1&pageSize=10
Authorization: Bearer {token}

# Ver detalhes de uma clínica
GET /api/admin/clinicas/{id}
Authorization: Bearer {token}

# Criar nova clínica
POST /api/admin/clinicas
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Clínica Veterinária XYZ",
  "email": "clinica@exemplo.pt",
  "password": "senha123",
  "cp": "1234-567",
  "nif": "123456789",
  "iban": "PT50000000000000000000000"
}

# Atualizar clínica
PUT /api/admin/clinicas/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Novo Nome",
  "email": "novo@email.pt",
  "password": "nova_senha",  // opcional
  "cp": "1234-567",
  "nif": "123456789",
  "iban": "PT50000000000000000000000"
}

# Eliminar clínica
DELETE /api/admin/clinicas/{id}
Authorization: Bearer {token}
```

#### Estatísticas Globais

```http
GET /api/admin/dashboard/stats
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "totalClinicas": 15,
  "totalFuncionarios": 42,
  "totalClientes": 238
}
```

#### Gestão de Admins

```http
# Listar admins
GET /api/admin/admins
Authorization: Bearer {token}

# Criar novo admin
POST /api/admin/admins
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Novo Admin",
  "email": "novo@admin.pt",
  "password": "senha123",
  "nivel": "admin"  // ou "super_admin"
}
```

---

## 🌐 3. Frontend

### Login de Admin

```typescript
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();

await authStore.login('admin@cvsl.pt', 'admin123', 'admin');
```

### Verificar se é Admin

```typescript
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();

if (authStore.isAdmin) {
  // Mostrar painel de admin
}
```

### Usar Admin Service

```typescript
import { adminService } from '@/services/adminService';

// Listar clínicas
const result = await adminService.getAllClinicas(1, 10);
console.log(result.data); // Array de clínicas
console.log(result.total); // Total de clínicas

// Criar clínica
await adminService.createClinica({
  nome: 'Nova Clínica',
  email: 'clinica@teste.pt',
  password: 'senha123',
  cp: '1234-567',
  iban: 'PT50000000000000000000000'
});

// Estatísticas
const stats = await adminService.getGlobalStats();
console.log(stats); // { totalClinicas, totalFuncionarios, totalClientes }
```

### Router Guard (Exemplo)

```typescript
// router/index.ts
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/login');
  } else {
    next();
  }
});

// Rota protegida
{
  path: '/admin',
  component: AdminDashboard,
  meta: { requiresAdmin: true }
}
```

---

## 🧪 4. Testes

### Teste Manual de Login

```bash
# Login como admin
curl -X POST http://localhost:5000/api/auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@cvsl.pt","password":"admin123"}'

# Guardar o token retornado
TOKEN="eyJhbGc..."

# Testar endpoint protegido
curl -X GET http://localhost:5000/api/admin/clinicas \
  -H "Authorization: Bearer $TOKEN"
```

### Credenciais de Teste (Mockup)

No código existe um mockup ativo para testes sem BD:

- **Email**: `admin@cvsl.pt`
- **Password**: `admin123`

---

## 🔒 5. Segurança

### JWT Token

O token JWT contém:
- `NameIdentifier`: ID do admin
- `Email`: Email do admin
- `Name`: Nome do admin
- `Role`: "Admin"
- `Nivel`: "admin" ou "super_admin"
- `Expiration`: 8 horas

### Verificação no Backend

Todos os endpoints `/api/admin/*` requerem:
```csharp
[Authorize(Roles = "Admin")]
```

### Boas Práticas

1. **Passwords fortes** - Mínimo 8 caracteres
2. **Hash BCrypt** - Rounds: 11 (padrão seguro)
3. **Token expiration** - 8 horas (configurável)
4. **HTTPS em produção** - Sempre usar SSL/TLS
5. **Rate limiting** - Implementar limite de tentativas de login

---

## 📊 6. Fluxo Completo

### Criar e usar Admin

```bash
# 1. Criar tabela
psql -U postgres -d cvsl_db -f sql/schema/admins.sql

# 2. Gerar hash
cd src/api
dotnet run --project GenerateHash.cs minhasenha

# 3. Inserir admin na BD
psql -U postgres -d cvsl_db
INSERT INTO admins (nome, email, password, nivel) VALUES
('Meu Admin', 'meu@admin.pt', '$2a$11$HASH_GERADO', 'super_admin');

# 4. Testar login
curl -X POST http://localhost:5000/api/auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email":"meu@admin.pt","password":"minhasenha"}'

# 5. Usar token para criar clínica
curl -X POST http://localhost:5000/api/admin/clinicas \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Clínica Teste",
    "email": "teste@clinica.pt",
    "password": "clinica123",
    "cp": "1234-567",
    "iban": "PT50000000000000000000000"
  }'
```

---

## 🎯 7. Próximos Passos

### Funcionalidades a Implementar

- [ ] **Dashboard Admin Vue Component**
  - Lista de clínicas
  - Formulário criar/editar clínica
  - Estatísticas visuais
  
- [ ] **Auditoria/Logs**
  - Tabela de logs de ações admin
  - Registar todas as alterações
  
- [ ] **Gestão Avançada**
  - Desativar clínicas (soft delete)
  - Relatórios de faturação global
  - Exportar dados
  
- [ ] **Níveis de Admin**
  - `super_admin`: Acesso total + gestão de admins
  - `admin`: Apenas gestão de clínicas
  
- [ ] **Reset de Password**
  - Endpoint para admin resetar password de clínicas
  
- [ ] **Notificações**
  - Email quando nova clínica criada
  - Alertas de problemas

---

## 💡 Dicas

### Desenvolvimento

```bash
# Rodar API
cd src/api
dotnet run

# Rodar Frontend
cd src/web
npm run dev
```

### Resolver Problemas

**Erro: "Email ou password inválidos"**
- Verificar se admin existe na BD
- Verificar se hash está correto
- Verificar se campo `ativo = true`

**Erro: "401 Unauthorized" nos endpoints**
- Token expirado (8 horas)
- Token inválido
- Role não é "Admin"

**Erro ao criar clínica: "Email já em uso"**
- Email já existe na tabela `clinicas`
- Escolher outro email

---

## 📝 Notas

- ✅ Tabela `admins` completamente separada de `clinicas` e `funcionarios`
- ✅ Autenticação via JWT com role "Admin"
- ✅ Endpoints protegidos com `[Authorize(Roles = "Admin")]`
- ✅ Frontend preparado para múltiplos tipos de login
- ✅ CRUD completo de clínicas pelo admin
- ✅ Estatísticas globais da plataforma

---

**Autor**: Sistema CVSL  
**Data**: Dezembro 2024  
**Versão**: 1.0
