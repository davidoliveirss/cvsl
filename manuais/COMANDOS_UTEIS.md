# ⚡ COMANDOS ÚTEIS - CVSL

## 🚀 Iniciar Projeto

### Tudo de uma vez:
```bash
./start.sh
```

### Separado:
```bash
# Terminal 1 - API
cd src/api
dotnet run --urls="http://localhost:5000"

# Terminal 2 - Frontend
cd src/web
npm run dev
```

---

## 🛑 Parar Servidores

```bash
# Parar tudo
pkill -f 'dotnet.*api'
pkill -f 'vite'

# Ou se usaste ./start.sh, pressiona Ctrl+C
```

---

## 🧪 Testar API

```bash
# Testar todos endpoints
./test_api.sh

# Testar manualmente com curl
curl -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{"email":"clinica@teste.pt","password":"teste123"}'
```

---

## 📦 Dependências

### Backend (C#):
```bash
cd src/api

# Restaurar pacotes
dotnet restore

# Build
dotnet build

# Ver pacotes instalados
dotnet list package
```

### Frontend (Vue.js):
```bash
cd src/web

# Instalar dependências
npm install

# Atualizar dependências
npm update

# Ver dependências
npm list --depth=0
```

---

## 🔍 Debugging

### Ver logs em tempo real:
```bash
# API
tail -f /tmp/cvsl_api.log

# Frontend
tail -f /tmp/cvsl_frontend.log
```

### Ver processos rodando:
```bash
# Verificar porta 5000 (API)
lsof -i :5000

# Verificar porta 5173 (Frontend)
lsof -i :5173
```

### Matar processo específico:
```bash
kill -9 $(lsof -t -i:5000)  # API
kill -9 $(lsof -t -i:5173)  # Frontend
```

---

## 🗄️ Base de Dados

### Conectar ao PostgreSQL:
```bash
psql -U cvsl -d cvsl -h localhost
```

### Ver tabelas:
```sql
\dt
```

### Ver dados:
```sql
SELECT * FROM clinicas;
SELECT * FROM funcionarios;
SELECT * FROM clientes;
```

### Inserir dados de teste:
```sql
INSERT INTO clinicas (nome, email, password, cp, nif, iban)
VALUES ('Minha Clínica', 'teste@clinica.pt', 'senha123', 
        '1000-100', '123456789', 'PT50000000000000000000001');
```

---

## 🏗️ Criar Novos Componentes

### Backend - Novo Controller:
```bash
cd src/api/Controllers
# Criar AnimaisController.cs
# (copiar de ClientesController.cs como template)
```

### Backend - Novo Model:
```bash
cd src/api/Models
# Criar Animal.cs
# (copiar de Cliente.cs como template)
```

### Frontend - Novo Service:
```bash
cd src/web/src/services
# Criar animaisService.ts
# (copiar de clientesService.ts como template)
```

### Frontend - Nova View:
```bash
cd src/web/src/views
# Criar AnimaisView.vue
# (copiar de ClientesView.vue como template)
```

---

## 🔐 Segurança

### Ver token JWT:
```bash
# No browser: F12 → Application → Local Storage
# Ou via JavaScript console:
localStorage.getItem('token')
```

### Decodificar token:
1. Copia token
2. Vai para https://jwt.io
3. Cola no campo "Encoded"
4. Vê claims decodificados

---

## 📊 Swagger / API Explorer

### Aceder:
```
http://localhost:5000/swagger
```

### Testar endpoint protegido:
1. Faz login via `/api/auth/login/clinica`
2. Copia o token da resposta
3. Clica "Authorize" no Swagger
4. Insere: `Bearer <token>`
5. Testa endpoints protegidos

---

## 🔧 Troubleshooting

### API não inicia:
```bash
# Verificar erros
dotnet build

# Ver logs detalhados
dotnet run --urls="http://localhost:5000" --verbosity detailed
```

### Frontend não inicia:
```bash
cd src/web

# Limpar cache e reinstalar
rm -rf node_modules package-lock.json
npm install

# Build de produção (para testar)
npm run build
```

### Erro CORS:
Verifica `src/api/Program.cs` - linha com `WithOrigins`:
```csharp
policy.WithOrigins("http://localhost:5173")
```

### Erro 401 Unauthorized:
- Token expirou → Faz login novamente
- Token inválido → Limpa localStorage e faz login
- Endpoint requer auth → Adiciona `[Authorize]` no controller

---

## 📈 Performance

### Ver tamanho do bundle (Frontend):
```bash
cd src/web
npm run build
# Verifica dist/assets/*.js
```

### Optimizar build:
```bash
# Frontend
npm run build -- --minify

# Backend
dotnet publish -c Release
```

---

## 🚀 Deploy

### Frontend (Vercel):
```bash
cd src/web
npm run build
# Upload pasta dist/
```

### API (Azure):
```bash
cd src/api
dotnet publish -c Release -o ./publish
# Deploy pasta publish/
```

---

## 📝 Comandos Git

```bash
# Estado
git status

# Adicionar tudo
git add .

# Commit
git commit -m "feat: implementa login com JWT"

# Push
git push origin main

# Ver histórico
git log --oneline --graph
```

---

## 🧹 Limpeza

### Limpar builds:
```bash
# Backend
cd src/api
rm -rf bin/ obj/

# Frontend
cd src/web
rm -rf dist/ node_modules/
```

### Limpar logs temporários:
```bash
rm /tmp/cvsl_*.log
```

---

## 📚 Documentação Rápida

```bash
# Ver arquivo
cat EXPLICACAO_CODIGO.md
cat INTEGRACAO_FRONTEND.md
cat GUIA_TESTES.md

# Abrir no browser (se tiver markdown viewer)
open EXPLICACAO_CODIGO.md
```

---

## ⚙️ Configurações

### Mudar porta da API:
```bash
# src/api/Properties/launchSettings.json
# ou
dotnet run --urls="http://localhost:PORTA"
```

### Mudar porta do Frontend:
```bash
# src/web/vite.config.ts
server: {
  port: 5173
}
```

### Mudar URL da API no Frontend:
```typescript
// src/web/src/services/authService.ts
const API_URL = 'http://localhost:5000/api';
```

---

## 🎯 Atalhos Úteis

```bash
# Alias para facilitar (adiciona ao ~/.bashrc)
alias cvsl-start='cd ~/dev/cvsl && ./start.sh'
alias cvsl-api='cd ~/dev/cvsl/src/api && dotnet run'
alias cvsl-web='cd ~/dev/cvsl/src/web && npm run dev'
alias cvsl-test='cd ~/dev/cvsl && ./test_api.sh'
alias cvsl-logs='tail -f /tmp/cvsl_api.log'
```

Depois:
```bash
source ~/.bashrc
cvsl-start  # Inicia tudo!
```

---

Todos os comandos que vais precisar estão aqui! 🚀
