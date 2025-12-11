# Como Autenticar no Swagger

## ⚠️ Problema Resolvido!
A configuração do Swagger foi atualizada para suportar JWT. Agora você verá o botão **Authorize** 🔓

## Problema Original
Ao tentar criar uma clínica em `/api/Admin/clinicas`, você recebe erro **401 Unauthorized** porque o endpoint está protegido com autenticação JWT.

## Solução: Autenticar no Swagger

### Passo 1: Fazer Login como Admin

1. No Swagger, encontre o endpoint **POST /api/Auth/login/admin**
2. Clique em "Try it out"
3. Use as credenciais de teste:

```json
{
  "email": "admin@cvsl.pt",
  "password": "admin123"
}
```

4. Clique em **Execute**
5. Na resposta, copie o valor do campo `token`:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@cvsl.pt",
  "nome": "Admin Principal"
}
```

### Passo 2: Configurar o Token no Swagger

1. No **topo direito** da página do Swagger, procure o botão **"Authorize"** 🔓 (cadeado verde)
2. Clique nele - abrirá um modal/popup
3. No campo **"Value"**, cole **APENAS O TOKEN** (sem a palavra "Bearer"):
   - ❌ Errado: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - ✅ Correto: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - ⚠️ **Importante**: O Swagger adiciona "Bearer" automaticamente!
4. Clique em **Authorize**
5. O cadeado ficará fechado 🔒
6. Feche o modal clicando em **Close**

### Passo 3: Testar Endpoint Protegido

Agora você pode usar qualquer endpoint que requer autenticação:

```json
POST /api/Admin/clinicas
{
  "nome": "Clínica Teste",
  "email": "clinica@clinica.pt",
  "password": "123",
  "cp": "1000-001",
  "nif": "123456789",
  "iban": "PT50000201231234567890154"
}
```

## Credenciais Mockup Disponíveis

### Admin (acesso total)
```json
{
  "email": "admin@cvsl.pt",
  "password": "admin123"
}
```
**Endpoint**: `POST /api/Auth/login/admin`

---

### Clínica (mockup)
```json
{
  "email": "clinica@teste.pt",
  "password": "teste123"
}
```
**Endpoint**: `POST /api/Auth/login/clinica`

---

### Funcionário - Veterinário (mockup)
```json
{
  "email": "vet@teste.pt",
  "password": "teste123"
}
```
**Endpoint**: `POST /api/Auth/login/funcionario`

---

### Funcionário - Recepcionista (mockup)
```json
{
  "email": "rececionista@teste.pt",
  "password": "teste123"
}
```
**Endpoint**: `POST /api/Auth/login/funcionario`

---

## Testando Soft Delete

### 1. Criar uma Clínica (como Admin)
```bash
POST /api/Admin/clinicas
Authorization: Bearer {seu_token_admin}

{
  "nome": "Clínica Teste",
  "email": "clinica@teste.pt",
  "password": "123",
  "cp": "1000-001",
  "nif": "123456789",
  "iban": "PT50000201231234567890154"
}
```

### 2. Desativar a Clínica (Soft Delete)
```bash
DELETE /api/Admin/clinicas/1
Authorization: Bearer {seu_token_admin}
```

**Resposta:**
```json
{
  "message": "Clínica desativada com sucesso"
}
```

### 3. Verificar que a Clínica foi Desativada
```bash
GET /api/Admin/clinicas?incluirInativos=true
Authorization: Bearer {seu_token_admin}
```

**Resposta:**
```json
{
  "data": [
    {
      "id": 1,
      "nome": "Clínica Teste",
      "email": "clinica@teste.pt",
      "ativo": false,  // <-- Campo mudou para false!
      ...
    }
  ]
}
```

### 4. Reativar a Clínica
```bash
PATCH /api/Admin/clinicas/1/ativo
Authorization: Bearer {seu_token_admin}

{
  "ativo": true
}
```

**Resposta:**
```json
{
  "message": "Clínica ativada com sucesso",
  "ativo": true
}
```

---

## Fluxo Completo de Teste

### Como Admin:

```bash
# 1. Login
POST /api/Auth/login/admin
Body: {"email": "admin@cvsl.pt", "password": "admin123"}

# 2. Criar clínica
POST /api/Admin/clinicas
Body: {...}

# 3. Criar funcionário para a clínica
POST /api/Admin/funcionarios (se existir endpoint)

# 4. Desativar funcionário
DELETE /api/Admin/funcionarios/1

# 5. Reativar funcionário
PATCH /api/Admin/funcionarios/1/ativo
Body: {"ativo": true}

# 6. Desativar clínica
DELETE /api/Admin/clinicas/1

# 7. Reativar clínica
PATCH /api/Admin/clinicas/1/ativo
Body: {"ativo": true}
```

---

## Dicas Importantes

1. **Token expira em 8 horas**: Se receber 401 novamente, faça login novamente
2. **Não esqueça "Bearer"**: O formato correto é `Bearer {token}`
3. **Endpoints protegidos**: 
   - `/api/Admin/*` requer role "Admin"
   - `/api/Clinicas/*` requer role "Clinica"
   - `/api/Funcionarios/*` requer role "Funcionario"
4. **Soft Delete**: O campo `ativo` nunca é removido, apenas alterado para `false`
5. **Filtros**: Use `?incluirInativos=true` para ver entidades desativadas

---

## Troubleshooting

### Erro 401 mesmo com token
- ✅ Verifique se o token está no formato `Bearer {token}`
- ✅ Confirme que o token não expirou (válido por 8 horas)
- ✅ Certifique-se de que usou o endpoint de login correto

### Erro 403 Forbidden
- ✅ Você está autenticado, mas não tem a role necessária
- ✅ Use o login de Admin para endpoints `/api/Admin/*`

### Token não aparece na resposta de login
- ✅ Verifique se a API está rodando
- ✅ Confirme as credenciais de teste
- ✅ Verifique os logs da API
