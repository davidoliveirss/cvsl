# ✅ Sistema de Admin - Implementado!

## 🎯 O que foi criado

### 📁 Ficheiros Novos

```
sql/
├── schema/admins.sql          # Tabela de administradores
└── seeds/admins.sql           # Dados exemplo

src/api/
├── Models/Admin.cs            # Model C# para admin
├── Controllers/AdminController.cs  # API de gestão (CRUD clínicas)
├── Context/ClinicaDbContext.cs    # ✏️ Atualizado: DbSet<Admin>
└── GenerateHash.cs            # Helper para gerar BCrypt hash

src/api/Controllers/
└── AuthController.cs          # ✏️ Atualizado: Login admin

src/web/src/
├── services/
│   ├── authService.ts         # ✏️ Atualizado: Suporte admin
│   └── adminService.ts        # Novo: API calls admin
└── stores/
    └── auth.ts                # ✏️ Atualizado: isAdmin

ADMIN_SETUP.md                 # Documentação completa
ADMIN_RESUMO.md               # Este ficheiro
```

---

## 🚀 Quick Start

### 1️⃣ Criar Tabela na BD

```bash
psql -U postgres -d cvsl_db -f sql/schema/admins.sql
```

### 2️⃣ Inserir Admin (Usar mockup por agora)

**Para testes rápidos**, já existe um **mockup** ativo:
- Email: `admin@cvsl.pt`
- Password: `admin123`

### 3️⃣ Testar Login

```bash
curl -X POST http://localhost:5000/api/auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@cvsl.pt","password":"admin123"}'
```

### 4️⃣ Frontend

```typescript
// Login como admin
await authStore.login('admin@cvsl.pt', 'admin123', 'admin');

// Verificar
if (authStore.isAdmin) {
  console.log('É admin!');
}

// Listar clínicas
const clinicas = await adminService.getAllClinicas();
```

---

## 📚 Endpoints API Admin

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/auth/login/admin` | Login de admin |
| `GET` | `/api/admin/clinicas` | Listar clínicas (paginado) |
| `GET` | `/api/admin/clinicas/{id}` | Ver clínica específica |
| `POST` | `/api/admin/clinicas` | Criar nova clínica |
| `PUT` | `/api/admin/clinicas/{id}` | Atualizar clínica |
| `DELETE` | `/api/admin/clinicas/{id}` | Eliminar clínica |
| `GET` | `/api/admin/dashboard/stats` | Estatísticas globais |
| `GET` | `/api/admin/admins` | Listar admins |
| `POST` | `/api/admin/admins` | Criar novo admin |

**Todos os `/api/admin/*` requerem token com role "Admin"!**

---

## 🔐 Autenticação

### Tabela: 3 Tipos de Login

| Tipo | Endpoint | Tabela | Role JWT |
|------|----------|--------|----------|
| **Admin** | `/api/auth/login/admin` | `admins` | `Admin` |
| **Clínica** | `/api/auth/login/clinica` | `clinicas` | `Clinica` |
| **Funcionário** | `/api/auth/login/funcionario` | `funcionarios` | `Funcionario` |

### Token JWT Admin

```json
{
  "NameIdentifier": "1",
  "Email": "admin@cvsl.pt",
  "Name": "Admin Principal",
  "Role": "Admin",
  "Nivel": "super_admin",
  "exp": "..."
}
```

---

## 🎨 Estrutura da Tabela `admins`

```sql
admins
├── id (SERIAL PRIMARY KEY)
├── nome (VARCHAR 100)
├── email (VARCHAR 100 UNIQUE)     ← Login
├── password (VARCHAR 255)          ← BCrypt hash
├── nivel (VARCHAR 20)              ← 'admin' ou 'super_admin'
└── ativo (BOOLEAN)                 ← Para desativar admin
```

---

## 💻 Exemplo Completo: Criar Clínica via Admin

```bash
# 1. Login
RESPONSE=$(curl -s -X POST http://localhost:5000/api/auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@cvsl.pt","password":"admin123"}')

TOKEN=$(echo $RESPONSE | jq -r '.token')

# 2. Criar clínica
curl -X POST http://localhost:5000/api/admin/clinicas \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Clínica Veterinária Nova",
    "email": "nova@clinica.pt",
    "password": "clinica123",
    "cp": "4000-123",
    "nif": "123456789",
    "iban": "PT50000000000000000000000"
  }'

# 3. Listar todas
curl -X GET http://localhost:5000/api/admin/clinicas \
  -H "Authorization: Bearer $TOKEN"
```

---

## ✨ Funcionalidades Implementadas

### Backend (API)
✅ Tabela `admins` na BD  
✅ Model C# `Admin`  
✅ Endpoint `POST /api/auth/login/admin`  
✅ Controller `AdminController` com:
  - CRUD completo de clínicas
  - Estatísticas globais
  - Gestão de admins
✅ Autorização `[Authorize(Roles = "Admin")]`  
✅ JWT com claim "Nivel" (super_admin/admin)  
✅ Mockup para testes sem BD  

### Frontend (Vue)
✅ `authService`: Suporte login admin  
✅ `adminService`: API calls para gestão  
✅ `authStore`: `isAdmin` computed  
✅ TypeScript types para todas as interfaces  

### Documentação
✅ `ADMIN_SETUP.md`: Guia completo passo-a-passo  
✅ `ADMIN_RESUMO.md`: Quick reference  
✅ Comentários em código  

---

## 🎯 Próximos Passos (A fazer)

### Frontend
- [ ] Criar componente `AdminDashboard.vue`
- [ ] Criar componente `ClinicasList.vue`
- [ ] Criar formulário `ClinicaForm.vue`
- [ ] Adicionar rotas `/admin/*` no router
- [ ] Implementar router guard para rotas admin
- [ ] Página de login com seletor de tipo (admin/clinica/funcionario)

### Backend
- [ ] Remover mockups quando BD estiver pronta
- [ ] Adicionar validações de dados
- [ ] Implementar soft delete para clínicas
- [ ] Logs de auditoria (quem criou/editou o quê)
- [ ] Rate limiting no login
- [ ] Reset de password

### Segurança
- [ ] HTTPS em produção
- [ ] Política de passwords fortes
- [ ] 2FA para super_admin (opcional)
- [ ] Sessões/refresh tokens

---

## 🐛 Troubleshooting

### Problema: "401 Unauthorized"
**Causa**: Token expirado ou role errada  
**Solução**: Fazer login novamente ou verificar role no token

### Problema: Build warning no GenerateHash.cs
**Causa**: Arquivo tem Main() mas não é usado  
**Solução**: Ignorar warning ou mover para projeto separado

### Problema: "Email já em uso"
**Causa**: Email já existe na tabela  
**Solução**: Usar outro email ou eliminar o existente

---

## 📞 Comandos Úteis

```bash
# Build API
cd src/api && dotnet build

# Rodar API
cd src/api && dotnet run

# Gerar hash de password
cd src/api && dotnet run --project GenerateHash.cs "minhasenha"

# Conectar à BD
psql -U postgres -d cvsl_db

# Ver admins na BD
psql -U postgres -d cvsl_db -c "SELECT * FROM admins;"

# Rodar frontend
cd src/web && npm run dev
```

---

## 🎊 Conclusão

Sistema de admin **completamente funcional** e **separado** de clínicas/funcionários!

**O que tens agora:**
- ✅ 3 tipos de login independentes
- ✅ Backend com autorização por roles
- ✅ CRUD de clínicas via admin
- ✅ Frontend preparado para admin
- ✅ Documentação completa

**Só falta:**
- Criar as páginas Vue para o painel admin
- Popular a BD com dados reais
- Deploy em produção

---

**Pronto para criar clínicas e gerir a aplicação! 🚀**
