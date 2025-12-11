# 🔐 Guia Passo-a-Passo: Autenticação no Swagger

## ⚠️ Erro Comum: "invalid_token"

Se você está recebendo `error="invalid_token"`, provavelmente:
- ❌ Colou o token com espaços extras
- ❌ Colou "Bearer" junto com o token
- ❌ Não copiou o token completo

---

## ✅ PASSO 1: Fazer Login

### 1.1 Abra o Swagger
```
http://localhost:5000/swagger
```

### 1.2 Encontre o endpoint de login
Procure por: **POST /api/Auth/login/admin**

### 1.3 Clique em "Try it out"
Um formulário vai aparecer

### 1.4 Cole o JSON no campo "Request body"
```json
{
  "email": "admin@cvsl.pt",
  "password": "admin123"
}
```

### 1.5 Clique em "Execute"

### 1.6 Copie o TOKEN da resposta
A resposta será assim:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhZG1pbkBjdnNsLnB0IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IkFkbWluIFByaW5jaXBhbCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzY1NDM4MDU3LCJpc3MiOiJDbGluaWNhQVBJIiwiYXVkIjoiQ2xpbmljYVdlYkFwcCJ9.NPhtCGa3VQsdQtRh6d9zypkfL0fgiAr7R5VKpKv7cbo",
  "email": "admin@cvsl.pt",
  "nome": "Admin Principal"
}
```

**⚠️ IMPORTANTE:** Copie APENAS o valor dentro das aspas do "token", desde `eyJ` até o final (sem as aspas!)

---

## ✅ PASSO 2: Configurar o Token no Swagger

### 2.1 Procure o botão "Authorize"
No **canto superior direito** da página, você verá um botão verde com um cadeado: 🔓 **Authorize**

### 2.2 Clique em "Authorize"
Um popup/modal vai abrir com o título "Available authorizations"

### 2.3 Você verá um campo chamado "Value"
```
┌─────────────────────────────────────────┐
│ Bearer (http, Bearer)                   │
│                                         │
│ Value: [____________campo vazio_______] │
│                                         │
│ Description: Insira o token JWT no      │
│ formato: Bearer {seu_token}             │
└─────────────────────────────────────────┘
```

### 2.4 Cole APENAS o token no campo "Value"
✅ **CORRETO:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhZG1pbkBjdnNsLnB0IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IkFkbWluIFByaW5jaXBhbCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzY1NDM4MDU3LCJpc3MiOiJDbGluaWNhQVBJIiwiYXVkIjoiQ2xpbmljYVdlYkFwcCJ9.NPhtCGa3VQsdQtRh6d9zypkfL0fgiAr7R5VKpKv7cbo
```

❌ **ERRADO (não coloque "Bearer"):**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

❌ **ERRADO (não coloque aspas):**
```
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### 2.5 Clique no botão "Authorize"
O botão está dentro do popup, embaixo do campo "Value"

### 2.6 O cadeado vai fechar 🔒
Você verá que o cadeado mudou de 🔓 para 🔒

### 2.7 Clique em "Close"
Feche o popup

---

## ✅ PASSO 3: Testar

### 3.1 Vá até o endpoint que quer testar
Exemplo: **POST /api/Admin/clinicas**

### 3.2 Clique em "Try it out"

### 3.3 Preencha os dados
```json
{
  "nome": "Clínica Teste",
  "email": "clinica@teste.pt",
  "password": "123",
  "cp": "1000-001",
  "nif": "123456789",
  "iban": "PT50000201231234567890154"
}
```

### 3.4 Clique em "Execute"

### 3.5 Verifique a resposta
✅ **Se funcionou (Status 200 ou 201):**
```json
{
  "id": 1,
  "nome": "Clínica Teste",
  "email": "clinica@teste.pt",
  ...
}
```

❌ **Se ainda dá erro 401:**
```
Error: Unauthorized
```
→ Volte ao PASSO 2 e verifique se colou o token corretamente

---

## 🔍 Checklist de Verificação

Antes de testar, confirme:

- [ ] Fiz login e obtive um token válido
- [ ] Copiei o token COMPLETO (começa com `eyJ` e tem centenas de caracteres)
- [ ] NÃO colei "Bearer" junto com o token
- [ ] NÃO colei aspas junto com o token
- [ ] Cliquei no botão "Authorize" dentro do popup
- [ ] O cadeado está fechado 🔒
- [ ] Fechei o popup clicando em "Close"

---

## 🐛 Troubleshooting

### ❌ Erro: "error='invalid_token'"

**Causa:** Token mal formatado ou incompleto

**Solução:**
1. Faça logout clicando no botão "Authorize" 🔒 e depois em "Logout"
2. Faça login novamente e obtenha um novo token
3. Copie o token com CTRL+A (selecionar tudo) no campo do token
4. Cole no Swagger sem "Bearer"
5. Verifique se não há espaços antes ou depois do token

### ❌ Erro: "401 Unauthorized" mesmo com token

**Causa:** Token não foi configurado corretamente

**Solução:**
1. Verifique se o cadeado está fechado 🔒
2. Se estiver aberto 🔓, o token não foi aceito
3. Clique no cadeado, depois em "Logout"
4. Tente novamente desde o PASSO 2

### ❌ Não vejo o botão "Authorize"

**Causa:** API precisa ser reiniciada

**Solução:**
```bash
pkill -f dotnet
cd /home/david/dev/cvsl/src/api
dotnet run
```
Aguarde a API iniciar e recarregue o Swagger

### ❌ Token expira muito rápido

**Causa:** Tokens expiram em 8 horas por segurança

**Solução:**
- Faça login novamente para obter um novo token
- Isso é comportamento normal de segurança

---

## 📝 Exemplo Completo

### 1. Login
```bash
POST /api/Auth/login/admin

Body:
{
  "email": "admin@cvsl.pt",
  "password": "admin123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...",
  "email": "admin@cvsl.pt",
  "nome": "Admin Principal"
}
```

### 2. Copiar Token
Selecione e copie tudo depois de `"token": "` e antes da última `"`

### 3. Authorize
- Clique em 🔓 Authorize (canto superior direito)
- Cole o token no campo "Value"
- Clique em "Authorize"
- Veja o cadeado fechar 🔒
- Clique em "Close"

### 4. Usar Endpoint
Agora qualquer endpoint protegido funcionará!

---

## 💡 Dica Pro

Para verificar se o token está válido, você pode testá-lo via curl:

```bash
# Substitua {seu_token} pelo token real
curl -X GET "http://localhost:5000/api/Admin/clinicas" \
  -H "Authorization: Bearer {seu_token}"
```

Se retornar dados (status 200), o token está válido! ✅
