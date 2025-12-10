# Implementação de Soft Delete

## Resumo
Implementado soft delete para **Clínicas** e **Funcionários**. Em vez de remover registos da base de dados, o campo `ativo` é alterado para `false`.

## Permissões

### Admins (AdminController)
- ✅ Podem desativar/ativar **Clínicas**
- ✅ Podem desativar/ativar **Funcionários**

### Clínicas (ClinicasController e FuncionariosController)
- ✅ Podem desativar/ativar **Funcionários** da sua clínica
- ⚠️ Podem desativar-se a si próprias (via ClinicasController)

## Endpoints Atualizados

### AdminController

#### Desativar Clínica
```http
DELETE /api/admin/clinicas/{id}
```
- Altera `ativo` para `false`
- Retorna: `{ "message": "Clínica desativada com sucesso" }`

#### Ativar/Desativar Clínica (novo)
```http
PATCH /api/admin/clinicas/{id}/ativo
Content-Type: application/json

{
  "ativo": true
}
```
- Permite ativar ou desativar uma clínica
- Retorna: `{ "message": "Clínica ativada com sucesso", "ativo": true }`

#### Desativar Funcionário
```http
DELETE /api/admin/funcionarios/{id}
```
- Altera `ativo` para `false`
- Retorna: `{ "message": "Funcionário desativado com sucesso" }`

#### Ativar/Desativar Funcionário (novo)
```http
PATCH /api/admin/funcionarios/{id}/ativo
Content-Type: application/json

{
  "ativo": true
}
```
- Permite ativar ou desativar um funcionário
- Retorna: `{ "message": "Funcionário ativado com sucesso", "ativo": true }`

#### Listar Clínicas (atualizado)
```http
GET /api/admin/clinicas?page=1&pageSize=10&incluirInativos=false
```
- Parâmetro `incluirInativos`: mostra clínicas inativas se `true`
- Por padrão, mostra apenas clínicas ativas
- Resposta inclui campo `ativo`

#### Obter Clínica (atualizado)
```http
GET /api/admin/clinicas/{id}
```
- Resposta inclui campo `ativo` para clínica e funcionários

#### Estatísticas (atualizado)
```http
GET /api/admin/dashboard/stats
```
- Conta apenas clínicas e funcionários ativos

---

### ClinicasController

#### Desativar Clínica
```http
DELETE /api/clinicas/{id}
```
- Altera `ativo` para `false`
- Retorna: `{ "message": "Clínica desativada com sucesso" }`

#### Ativar/Desativar Clínica (novo)
```http
PATCH /api/clinicas/{id}/ativo
Content-Type: application/json

{
  "ativo": true
}
```
- Permite ativar ou desativar a clínica
- Retorna: `{ "message": "Clínica ativada com sucesso", "ativo": true }`

#### Obter Clínica (atualizado)
```http
GET /api/clinicas/{id}
```
- Resposta inclui campo `ativo`

---

### FuncionariosController

#### Desativar Funcionário
```http
DELETE /api/funcionarios/{id}
```
- Altera `ativo` para `false`
- Retorna: `{ "message": "Funcionário desativado com sucesso" }`

#### Ativar/Desativar Funcionário (novo)
```http
PATCH /api/funcionarios/{id}/ativo
Content-Type: application/json

{
  "ativo": true
}
```
- Permite ativar ou desativar um funcionário
- Retorna: `{ "message": "Funcionário ativado com sucesso", "ativo": true }`

#### Obter Funcionário (atualizado)
```http
GET /api/funcionarios/{id}
```
- Resposta inclui campo `ativo`

#### Listar Funcionários por Clínica (atualizado)
```http
GET /api/funcionarios/clinica/{clinicaId}?incluirInativos=false
```
- Parâmetro `incluirInativos`: mostra funcionários inativos se `true`
- Por padrão, mostra apenas funcionários ativos
- Resposta inclui campo `ativo`

---

## Alterações na Base de Dados

### Tabelas Afetadas
O campo `ativo` já existia nas tabelas:
- ✅ `clinicas` - campo `ativo BOOLEAN DEFAULT TRUE`
- ✅ `funcionarios` - campo `ativo BOOLEAN DEFAULT TRUE`

**Nenhuma migração necessária** - o campo já estava no schema!

---

## Models Atualizados

### UpdateAtivoAdminModel (AdminController)
```csharp
public class UpdateAtivoAdminModel
{
    public bool Ativo { get; set; }
}
```

### UpdateAtivoClinicaModel (ClinicasController)
```csharp
public class UpdateAtivoClinicaModel
{
    public bool Ativo { get; set; }
}
```

### UpdateAtivoModel (FuncionariosController)
```csharp
public class UpdateAtivoModel
{
    public bool Ativo { get; set; }
}
```

---

## Comportamento das Queries

### Antes
```csharp
// Todas as clínicas
var clinicas = await _context.Clinicas.ToListAsync();
```

### Depois
```csharp
// Apenas clínicas ativas (padrão)
var clinicas = await _context.Clinicas
    .Where(c => c.Ativo)
    .ToListAsync();

// Todas as clínicas (incluindo inativas)
var clinicas = await _context.Clinicas.ToListAsync();
```

---

## Exemplos de Uso

### Admin desativa uma clínica
```bash
# Usando DELETE (soft delete)
curl -X DELETE http://localhost:5000/api/admin/clinicas/1 \
  -H "Authorization: Bearer {token}"

# Usando PATCH (mais explícito)
curl -X PATCH http://localhost:5000/api/admin/clinicas/1/ativo \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"ativo": false}'
```

### Admin reativa uma clínica
```bash
curl -X PATCH http://localhost:5000/api/admin/clinicas/1/ativo \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"ativo": true}'
```

### Clínica desativa um funcionário
```bash
# Usando DELETE
curl -X DELETE http://localhost:5000/api/funcionarios/5 \
  -H "Authorization: Bearer {token_clinica}"

# Usando PATCH
curl -X PATCH http://localhost:5000/api/funcionarios/5/ativo \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token_clinica}" \
  -d '{"ativo": false}'
```

### Listar apenas funcionários ativos
```bash
curl http://localhost:5000/api/funcionarios/clinica/1 \
  -H "Authorization: Bearer {token}"
```

### Listar todos os funcionários (incluindo inativos)
```bash
curl http://localhost:5000/api/funcionarios/clinica/1?incluirInativos=true \
  -H "Authorization: Bearer {token}"
```

---

## Notas Importantes

1. **Sem Cascade Delete**: O ON DELETE CASCADE nas foreign keys não é acionado porque não há DELETE real na base de dados

2. **Queries Existentes**: Atualize queries existentes para filtrar por `ativo = true` onde necessário

3. **Autenticação**: Adicione validação para garantir que:
   - Clínicas só possam desativar seus próprios funcionários
   - Clínicas não possam desativar outras clínicas

4. **Reativação**: Apenas Admins podem reativar clínicas através do endpoint PATCH

5. **Login**: Considere impedir login de usuários inativos no AuthController
