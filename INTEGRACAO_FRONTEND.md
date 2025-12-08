# 🚀 GUIA DE INTEGRAÇÃO - Frontend Vue.js + API C#

## ✅ O QUE JÁ ESTÁ PRONTO:

### Backend (API C#):
- ✅ Autenticação JWT com 2 endpoints:
  - `/api/auth/login/clinica`
  - `/api/auth/login/funcionario`
- ✅ CORS configurado para `localhost:5173`
- ✅ Models: Cliente, Clinica, Funcionario
- ✅ DbContext configurado
- ✅ API rodando em `http://localhost:5000`

### Frontend (Vue.js):
- ✅ `authService.ts` - Comunicação com API de autenticação
- ✅ `auth.ts` (Pinia Store) - Gestão de estado de autenticação
- ✅ `LoginView.vue` - Página de login com seletor Clínica/Funcionário
- ✅ Router com guards de autenticação
- ✅ `clientesService.ts` - Exemplo de serviço CRUD
- ✅ `ClientesView.vue` - Exemplo de página com tabela e CRUD

---

## 🎯 COMO TESTAR:

### 1. Iniciar a API:
```bash
cd /home/david/dev/cvsl/src/api
dotnet run --urls="http://localhost:5000"
```

### 2. Iniciar o Frontend:
```bash
cd /home/david/dev/cvsl/src/web
npm run dev
```

### 3. Acessar:
```
http://localhost:5173/login
```

### 4. Credenciais de Teste (Mockup):

**Clínica:**
- Email: `clinica@teste.pt`
- Password: `teste123`

**Funcionários:**
- Veterinário: `vet@teste.pt` / `teste123`
- Rececionista: `rececionista@teste.pt` / `teste123`

---

## 📂 ESTRUTURA DOS FICHEIROS:

```
src/web/src/
├── services/
│   ├── authService.ts          ← Autenticação (login, logout, token)
│   └── clientesService.ts      ← Exemplo de serviço CRUD
│
├── stores/
│   └── auth.ts                 ← Pinia store (estado global)
│
├── views/
│   ├── LoginView.vue           ← Página de login
│   └── ClientesView.vue        ← Exemplo de página CRUD
│
└── router/
    └── index.ts                ← Rotas + guards de autenticação
```

---

## 🔑 COMO FUNCIONA A AUTENTICAÇÃO:

### 1. Login:
```typescript
// No componente Vue
const authStore = useAuthStore();
await authStore.login('email@teste.pt', 'senha123', 'clinica');
```

### 2. O que acontece por trás:
```
LoginView.vue
    ↓ chama
authStore.login()
    ↓ chama
authService.login()
    ↓ faz
POST http://localhost:5000/api/auth/login/clinica
    ↓ recebe
{ token: "eyJhbGc...", email: "...", nome: "..." }
    ↓ guarda em
localStorage + Pinia Store
    ↓
Redireciona para página inicial
```

### 3. Usar token em requests:
```typescript
// authService.fetchWithAuth() adiciona automaticamente:
// Authorization: Bearer eyJhbGc...

const clientes = await clientesService.getAll();
// Faz GET /api/clientes com token no header
```

### 4. Logout:
```typescript
authStore.logout();
// Remove token do localStorage
// Limpa store
// Redireciona para /login
```

---

## 🛠️ CRIAR NOVOS SERVIÇOS (CRUD):

### Template para novo serviço:

```typescript
// services/animaisService.ts
import { authService } from './authService';

export interface Animal {
  id?: number;
  transponder?: string;
  nome: string;
  especie?: string;
  raca?: string;
  dataNascimento?: string;
  sexo?: string;
  idCliente: number;
  idClinica: number;
}

export const animaisService = {
  async getAll(): Promise<Animal[]> {
    const response = await authService.fetchWithAuth('/animais');
    if (!response.ok) throw new Error('Erro ao carregar animais');
    return response.json();
  },

  async getById(id: number): Promise<Animal> {
    const response = await authService.fetchWithAuth(`/animais/${id}`);
    if (!response.ok) throw new Error('Animal não encontrado');
    return response.json();
  },

  async create(animal: Animal): Promise<Animal> {
    const response = await authService.fetchWithAuth('/animais', {
      method: 'POST',
      body: JSON.stringify(animal)
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar animal');
    }
    return response.json();
  },

  async update(id: number, animal: Animal): Promise<Animal> {
    const response = await authService.fetchWithAuth(`/animais/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...animal, id })
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar animal');
    }
    return response.json();
  },

  async delete(id: number): Promise<void> {
    const response = await authService.fetchWithAuth(`/animais/${id}`, {
      method: 'DELETE'
    });
    if (!response.ok) throw new Error('Erro ao remover animal');
  }
};
```

---

## 📄 TEMPLATE DE VIEW (CRUD):

```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useQuasar } from 'quasar';
import { animaisService, type Animal } from '@/services/animaisService';

const $q = useQuasar();
const animais = ref<Animal[]>([]);
const loading = ref(false);

async function loadAnimais() {
  loading.value = true;
  try {
    animais.value = await animaisService.getAll();
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message
    });
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadAnimais();
});
</script>

<template>
  <q-page padding>
    <div class="text-h5">Animais</div>
    
    <q-table
      :rows="animais"
      :loading="loading"
      row-key="id"
    />
  </q-page>
</template>
```

---

## 🔒 PROTEGER ROTAS:

```typescript
// router/index.ts
{
  path: '/clientes',
  name: 'clientes',
  component: () => import('../views/ClientesView.vue'),
  meta: { requiresAuth: true }  // ← Requer autenticação
}

// O guard automático verifica:
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login');  // Redireciona para login
  } else {
    next();
  }
})
```

---

## 🎨 USAR DADOS DO UTILIZADOR:

```vue
<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
</script>

<template>
  <!-- Mostrar nome do utilizador -->
  <div>Olá, {{ authStore.user?.nome }}</div>
  
  <!-- Mostrar baseado no tipo -->
  <div v-if="authStore.isClinica">
    Você é uma Clínica
  </div>
  
  <div v-if="authStore.isFuncionario">
    Você é um Funcionário
  </div>
  
  <!-- Botão de logout -->
  <q-btn @click="authStore.logout()" label="Sair" />
</template>
```

---

## 🚨 TRATAMENTO DE ERROS:

### Erros automáticos:

```typescript
// authService.fetchWithAuth() já trata:
// - Token inválido → Logout automático + Redireciona para /login
// - Erro 401 → Logout + Redireciona

// Você só precisa fazer:
try {
  const data = await clientesService.getAll();
} catch (error: any) {
  $q.notify({
    type: 'negative',
    message: error.message
  });
}
```

---

## 📋 CHECKLIST DE INTEGRAÇÃO:

### Para cada entidade (Cliente, Animal, Consulta, etc.):

- [ ] 1. Criar Model na API (C#)
- [ ] 2. Adicionar DbSet no DbContext
- [ ] 3. Criar Controller na API
- [ ] 4. Criar interface TypeScript no frontend
- [ ] 5. Criar serviço (ex: `animaisService.ts`)
- [ ] 6. Criar view (ex: `AnimaisView.vue`)
- [ ] 7. Adicionar rota no router
- [ ] 8. Adicionar link no menu/sidebar

---

## 🎯 EXEMPLO COMPLETO - Adicionar Animais:

### 1. Backend (já tens o Model, falta Controller):

```csharp
// Controllers/AnimaisController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnimaisController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public AnimaisController(ClinicaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var animais = await _context.Animais.ToListAsync();
        return Ok(animais);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var animal = await _context.Animais.FindAsync(id);
        if (animal == null) return NotFound();
        return Ok(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Animal animal)
    {
        _context.Animais.Add(animal);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Animal animal)
    {
        if (id != animal.Id) return BadRequest();
        var existente = await _context.Animais.FindAsync(id);
        if (existente == null) return NotFound();

        existente.Nome = animal.Nome;
        existente.Especie = animal.Especie;
        // ... atualizar outros campos

        await _context.SaveChangesAsync();
        return Ok(existente);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _context.Animais.FindAsync(id);
        if (animal == null) return NotFound();
        
        _context.Animais.Remove(animal);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

### 2. Frontend - Criar serviço (copiar de `clientesService.ts` e adaptar)

### 3. Frontend - Criar view (copiar de `ClientesView.vue` e adaptar)

### 4. Adicionar rota:
```typescript
{
  path: '/animais',
  name: 'animais',
  component: () => import('../views/AnimaisView.vue'),
  meta: { requiresAuth: true }
}
```

---

## 🔧 DEBUGGING:

### Ver requests no console do browser:
```
F12 → Network → Fetch/XHR
```

### Ver token JWT:
```
F12 → Application → Local Storage → http://localhost:5173
```

### Ver erros da API:
```
Terminal onde a API está rodando
```

---

## 📚 PRÓXIMOS PASSOS:

1. ✅ **Login está funcional** - Testa primeiro!
2. 📝 **Criar Models** para outras tabelas (Animal, Consulta, etc.)
3. 🎛️ **Criar Controllers** para cada entidade
4. 🌐 **Criar serviços** no frontend
5. 🎨 **Criar views** para cada funcionalidade
6. 🔐 **Implementar BCrypt** para passwords
7. 🚀 **Deploy**

---

## ❓ TROUBLESHOOTING:

### Erro CORS:
```
Access to fetch at 'http://localhost:5000/api/...' from origin 
'http://localhost:5173' has been blocked by CORS policy
```

**Solução:** Verifica `Program.cs` - CORS deve estar configurado para `localhost:5173`

### Token não enviado:
Verifica se estás usando `authService.fetchWithAuth()` em vez de `fetch()` direto

### 401 Unauthorized:
- Token expirou (duração: 8 horas)
- Token inválido
- Faça login novamente

### API não responde:
- Verifica se API está rodando: `http://localhost:5000/swagger`
- Verifica porta correta (5000, não 5098)

---

Está tudo pronto para começares a usar! 🎉

Testa primeiro o login, depois experimenta a página de Clientes (`/clientes`).
