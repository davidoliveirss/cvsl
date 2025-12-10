# 🧪 GUIA DE TESTES - API CVSL

## ✅ Todos os testes passaram com sucesso!

### 📊 Resultados dos Testes Automáticos:
- ✅ Login Clínica: **SUCESSO**
- ✅ Login Veterinário: **SUCESSO**
- ✅ Login Rececionista: **SUCESSO**
- ✅ Rejeição de credenciais inválidas: **SUCESSO**

---

## 🚀 Como Executar os Testes

### Opção 1: Script Automático (Recomendado)
```bash
# Iniciar a API em um terminal
cd /home/david/dev/cvsl/src/api
dotnet run --urls="http://localhost:5000"

# Em outro terminal, executar os testes
cd /home/david/dev/cvsl
./test_api.sh
```

### Opção 2: Testar Manualmente com curl

#### 🏥 Login da Clínica
```bash
curl -X POST http://localhost:5000/api/auth/login/clinica \
  -H "Content-Type: application/json" \
  -d '{
    "email": "clinica@teste.pt",
    "password": "teste123"
  }'
```

#### 👨‍⚕️ Login do Veterinário
```bash
curl -X POST http://localhost:5000/api/auth/login/funcionario \
  -H "Content-Type: application/json" \
  -d '{
    "email": "vet@teste.pt",
    "password": "teste123"
  }'
```

#### 👩‍💼 Login da Rececionista
```bash
curl -X POST http://localhost:5000/api/auth/login/funcionario \
  -H "Content-Type: application/json" \
  -d '{
    "email": "rececionista@teste.pt",
    "password": "teste123"
  }'
```

---

## 🎯 Credenciais de Teste (Mockup)

### Clínica:
- **Email:** `clinica@teste.pt`
- **Password:** `teste123`
- **Role:** Clinica

### Funcionários:
| Email | Password | Nome | Role |
|-------|----------|------|------|
| `vet@teste.pt` | `teste123` | Dr. João Silva | Funcionario |
| `rececionista@teste.pt` | `teste123` | Maria Santos | Funcionario |

---

## 🔍 Como Verificar o Token JWT

### Online (jwt.io):
1. Copie o token da resposta
2. Acesse: https://jwt.io
3. Cole o token no campo "Encoded"
4. Veja os claims decodificados

### Usando jq no terminal:
```bash
# Exemplo de decodificação do payload (segunda parte do token)
TOKEN="eyJhbGci..."
echo $TOKEN | cut -d'.' -f2 | base64 -d 2>/dev/null | jq '.'
```

### O que esperar no Token da Clínica:
```json
{
  "nameid": "1",
  "email": "clinica@teste.pt",
  "name": "Clínica Veterinária Teste",
  "role": "Clinica",
  "exp": 1234567890,
  "iss": "ClinicaAPI",
  "aud": "ClinicaWebApp"
}
```

### O que esperar no Token do Funcionário:
```json
{
  "nameid": "1",
  "email": "vet@teste.pt",
  "name": "Dr. João Silva",
  "role": "Funcionario",
  "ClinicaId": "1",      // ⬅️ IMPORTANTE: ID da clínica do funcionário
  "exp": 1234567890,
  "iss": "ClinicaAPI",
  "aud": "ClinicaWebApp"
}
```

---

## 🌐 Testar com Swagger

A API também tem Swagger UI disponível:

```
http://localhost:5000/swagger
```

Lá você pode:
1. Ver todos os endpoints disponíveis
2. Testar diretamente pela interface
3. Ver a documentação gerada automaticamente

---

## 📝 Próximos Passos

### Para conectar com a Base de Dados Real:
1. No `AuthController.cs`, descomente as seções marcadas com `/* ... */`
2. Comente as seções de MOCKUP DATA
3. Insira dados reais na BD (clínicas e funcionários)
4. Reinicie a API

### Para usar no Frontend (Vue):
```javascript
// Login
const response = await fetch('http://localhost:5000/api/auth/login/clinica', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'clinica@teste.pt',
    password: 'teste123'
  })
});

const data = await response.json();
const token = data.token;

// Guardar token
localStorage.setItem('token', token);

// Usar token em requests autenticadas
const protectedResponse = await fetch('http://localhost:5000/api/alguma-rota', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

---

## ⚠️ Lembrete de Segurança

**ATENÇÃO:** Os dados mockup são APENAS PARA TESTES!

Quando conectar à BD real:
- ✅ Implemente hash de passwords (BCrypt)
- ✅ Valide inputs
- ✅ Use HTTPS em produção
- ✅ Não exponha secrets no código
- ✅ Configure CORS adequadamente

---

## 🆘 Troubleshooting

### API não inicia:
```bash
# Verificar se porta 5000 está ocupada
lsof -i :5000

# Matar processo se necessário
kill -9 <PID>
```

### Erro de conexão à BD:
- Verifique se PostgreSQL está rodando
- Confirme credenciais no `appsettings.json`
- Para testes mockup, ignore este erro (não usa BD)

### Token expirado:
- Tokens duram 8 horas
- Faça login novamente para obter novo token
