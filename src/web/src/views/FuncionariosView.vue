<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useQuasar } from 'quasar';
import { funcionariosService, type Funcionario } from '@/services/funcionariosService';
import { authService } from '@/services/authService';
import { fasBedPulse } from '@quasar/extras/fontawesome-v6';

const $q = useQuasar();

const funcionarios = ref<Funcionario[]>([]);
const loading = ref(false);
const dialog = ref(false);
const editMode = ref(false);
const incluirInativos = ref(false);

// Obtém o ID da clínica do token JWT
const clinicaId = computed(() => {
  const token = authService.getToken();
  if (!token) return 1; // Fallback
  
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    // O NameIdentifier contém o ID da clínica/funcionário/admin
    return parseInt(payload.nameid || payload.sub || '1');
  } catch {
    return 1; // Fallback
  }
});

const formData = ref<Funcionario>({
  nome: '',
  email: '',
  password: '',
  especialidade: '',
  telefone: '',
  salario: 0,
  ativo: true,
  clinicaId: clinicaId.value
});

const columns = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, sortable: true },
  { name: 'email', label: 'Email', field: 'email', align: 'left' as const },
  { name: 'especialidade', label: 'Especialidade', field: 'especialidade', align: 'left' as const },
  { name: 'telefone', label: 'Telefone', field: 'telefone', align: 'left' as const },
  { name: 'salario', label: 'Salário', field: 'salario', align: 'right' as const, sortable: true },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' as const },
  { name: 'actions', label: 'Ações', field: 'id', align: 'center' as const }
];

async function loadFuncionarios() {
  loading.value = true;
  try {
    funcionarios.value = await funcionariosService.getAll(incluirInativos.value);
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao carregar funcionários'
    });
  } finally {
    loading.value = false;
  }
}

function openNewDialog() {
  editMode.value = false;
  formData.value = {
    nome: '',
    email: '',
    password: '',
    especialidade: '',
    telefone: '',
    salario: 0,
    ativo: true,
    clinicaId: clinicaId.value
  };
  dialog.value = true;
}

function openEditDialog(funcionario: Funcionario) {
  editMode.value = true;
  formData.value = { ...funcionario };
  dialog.value = true;
}


async function saveFuncionario() {
  try {
    if (editMode.value && formData.value.id) {
      await funcionariosService.update(formData.value.id, formData.value);
      $q.notify({
        type: 'positive',
        message: 'Funcionário atualizado com sucesso!'
      });
    } else {
      await funcionariosService.create(formData.value);
      $q.notify({
        type: 'positive',
        message: 'Funcionário criado com sucesso!'
      });
    }
    
    dialog.value = false;
    await loadFuncionarios();
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao salvar funcionário'
    });
  }
}

async function toggleEstadoFuncionario(funcionario: Funcionario) {
  const novoEstado = !funcionario.ativo;
  const acao = novoEstado ? 'ativar' : 'desativar';
  
  $q.dialog({
    title: 'Confirmar alteração de estado',
    message: `Tem certeza que deseja ${acao} este funcionário? ${novoEstado ? 'O funcionário poderá fazer login e ter acesso ao sistema.' : 'O funcionário não poderá fazer login nem ter acesso ao sistema.'}`,
    cancel: {
      label: 'Cancelar',
      flat: true,
      color: 'grey-7'
    },
    ok: {
      label: 'Confirmar',
      color: novoEstado ? 'positive' : 'warning'
    },
    persistent: true
  }).onOk(async () => {
    try {
      if (funcionario.ativo) {
        await funcionariosService.updateStatus(funcionario.id!, false);
      } else {
        await funcionariosService.updateStatus(funcionario.id!, true);
      }
      
      $q.notify({
        type: 'positive',
        message: `Funcionário ${novoEstado ? 'ativado' : 'desativado'} com sucesso!`
      });
      await loadFuncionarios();
    } catch (error: any) {
      $q.notify({
        type: 'negative',
        message: error.message || 'Erro ao alterar estado do funcionário'
      });
    }
  });
}

onMounted(() => {
  loadFuncionarios();
});
</script>

<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-md">
        <div class="col">
          <div class="text-h5">Funcionários</div>
          <div class="text-subtitle2 text-grey-7">
            Gerir funcionários da clínica
          </div>
        </div>
        <div class="col-auto row items-center q-gutter-md">
          <q-toggle
            v-model="incluirInativos"
            label="Mostrar inativos"
            @update:model-value="loadFuncionarios"
          />
          <q-btn
            color="primary"
            label="Novo Funcionário"
            icon="add"
            @click="openNewDialog"
          />
        </div>
      </div>

      <q-table
        :rows="funcionarios"
        :columns="columns"
        row-key="id"
        :loading="loading"
        flat
        bordered
      >
        <template v-slot:body-cell-salario="props">
          <q-td :props="props">
            {{ props.row.salario.toFixed(2) }}€
          </q-td>
        </template>

        <template v-slot:body-cell-ativo="props">
          <q-td :props="props">
            <q-badge 
              :color="props.row.ativo ? 'positive' : 'negative'"
              :label="props.row.ativo ? 'Ativo' : 'Inativo'"
            />
          </q-td>
        </template>

        <template v-slot:body-cell-actions="props">
          <q-td :props="props">
            <q-btn
              flat
              round
              dense
              color="primary"
              icon="edit"
              @click="openEditDialog(props.row)"
            >
              <q-tooltip>Editar</q-tooltip>
            </q-btn>
            <q-btn
              flat
              round
              dense
              :color="props.row.ativo ? 'negative' : 'positive'"
              :icon="props.row.ativo ? 'block' : 'check_circle'"
              @click="toggleEstadoFuncionario(props.row)"
            >
              <q-tooltip>{{ props.row.ativo ? 'Desativar' : 'Ativar' }}</q-tooltip>
            </q-btn>
          </q-td>
        </template>

        <template v-slot:no-data>
          <div class="full-width row flex-center text-grey-7 q-gutter-sm q-pa-lg">
            <q-icon size="2em" name="sentiment_dissatisfied" />
            <span>Nenhum funcionário encontrado</span>
          </div>
        </template>
      </q-table>
    </div>

    <!-- Dialog Criar/Editar -->
    <q-dialog v-model="dialog" persistent>
      <q-card style="min-width: 450px">
        <q-card-section>
          <div class="text-h6">
            {{ editMode ? 'Editar Funcionário' : 'Novo Funcionário' }}
          </div>
        </q-card-section>

        <q-card-section class="q-pt-none">
          <q-input
            v-model="formData.nome"
            label="Nome *"
            outlined
            dense
            :rules="[val => !!val || 'Nome é obrigatório']"
          />

          <q-input
            v-model="formData.email"
            label="Email *"
            type="email"
            outlined
            dense
            class="q-mt-md"
            :rules="[val => !!val || 'Email é obrigatório']"
          />

          <q-input
            v-if="!editMode"
            v-model="formData.password"
            label="Password *"
            type="password"
            outlined
            dense
            class="q-mt-md"
            :rules="[val => !!val || 'Password é obrigatório']"
          />

          <q-input
            v-model="formData.especialidade"
            label="Especialidade"
            outlined
            dense
            class="q-mt-md"
          />

          <q-input
            v-model="formData.telefone"
            label="Telefone *"
            outlined
            dense
            class="q-mt-md"
            :rules="[val => !!val || 'Telefone é obrigatório']"
          />

          <q-input
            v-model.number="formData.salario"
            label="Salário *"
            type="number"
            outlined
            dense
            class="q-mt-md"
            suffix="€"
            :rules="[val => val >= 0 || 'Salário inválido']"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey-7" v-close-popup />
          <q-btn
            unelevated
            label="Salvar"
            color="primary"
            @click="saveFuncionario"
            :disable="!formData.nome || !formData.email || !formData.telefone || (!editMode && !formData.password)"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
    
  </q-page>
</template>

<style scoped>
</style>
