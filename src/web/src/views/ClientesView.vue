<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useQuasar } from 'quasar';
import { clientesService, type Cliente } from '@/services/clientesService';
import { authService } from '@/services/authService';
import { fasBedPulse } from '@quasar/extras/fontawesome-v6';

const $q = useQuasar();

const clientes = ref<Cliente[]>([]);
const loading = ref(false);
const dialog = ref(false);
const editMode = ref(false);
const incluirInativos = ref(false);

// Obtém o ID da clínica do token JWT (funcionário pertence a uma clínica)
const clinicaId = computed(() => {
  const token = authService.getToken();
  if (!token) return 1; // Fallback
  
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    // Para funcionários, pode vir de uma claim específica da clínica
    return parseInt(payload.clinicaid || payload.nameid || payload.sub || '1');
  } catch {
    return 1; // Fallback
  }
});

const formData = ref<Cliente>({
  nome: '',
  telefone: '',
  email: '',
  nif: '',
  morada: '',
  ativo: true,
  clinicaId: clinicaId.value
});

const columns = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, sortable: true },
  { name: 'telefone', label: 'Telefone', field: 'telefone', align: 'left' as const },
  { name: 'email', label: 'Email', field: 'email', align: 'left' as const },
  { name: 'nif', label: 'NIF', field: 'nif', align: 'left' as const },
  { name: 'morada', label: 'Morada', field: 'morada', align: 'left' as const },
  { name: 'actions', label: 'Ações', field: 'id', align: 'center' as const }
];

async function loadClientes() {
  loading.value = true;
  try {
    clientes.value = await clientesService.getAll(incluirInativos.value);
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao carregar clientes'
    });
  } finally {
    loading.value = false;
  }
}

function openNewDialog() {
  editMode.value = false;
  formData.value = {
    nome: '',
    telefone: '',
    email: '',
    nif: '',
    morada: '',
    ativo: true,
    clinicaId: clinicaId.value
  };
  dialog.value = true;
}

function openEditDialog(cliente: Cliente) {
  editMode.value = true;
  formData.value = { ...cliente };
  dialog.value = true;
}

async function saveCliente() {
  try {
    if (editMode.value && formData.value.id) {
      await clientesService.update(formData.value.id, formData.value);
      $q.notify({
        type: 'positive',
        message: 'Cliente atualizado com sucesso!'
      });
    } else {
      await clientesService.create(formData.value);
      $q.notify({
        type: 'positive',
        message: 'Cliente criado com sucesso!'
      });
    }
    
    dialog.value = false;
    await loadClientes();
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao salvar cliente'
    });
  }
}

async function toggleEstadoCliente(cliente: Cliente) {
  const novoEstado = !cliente.ativo;
  const acao = novoEstado ? 'ativar' : 'desativar';
  
  $q.dialog({
    title: 'Confirmar alteração de estado',
    message: `Tem certeza que deseja ${acao} este cliente? ${novoEstado ? 'O cliente poderá fazer login e ter acesso ao sistema.' : 'O funcionário não poderá fazer login nem ter acesso ao sistema.'}`, //mudar o texto
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
      if (cliente.ativo) {
        await clientesService.updateStatus(cliente.id!, false);
      } else {
        await clientesService.updateStatus(cliente.id!, true);
      }
      
      $q.notify({
        type: 'positive',
        message: `Cliente ${novoEstado ? 'ativado' : 'desativado'} com sucesso!`
      });
      await loadClientes();
    } catch (error: any) {
      $q.notify({
        type: 'negative',
        message: error.message || 'Erro ao alterar estado do cliente'
      });
    }
  });
}

onMounted(() => {
  loadClientes();
});
</script>

<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-md">
        <div class="col">
          <div class="text-h5">Clientes</div>
          <div class="text-subtitle2 text-grey-7">
            Gerir clientes da clínica
          </div>
        </div>
        <div class="col-auto row items-center q-gutter-md">
          <q-toggle
            v-model="incluirInativos"
            label="Mostrar inativos"
            @update:model-value="loadClientes"
          />
          <q-btn
            color="primary"
            label="Novo Cliente"
            icon="add"
            @click="openNewDialog"
          />
        </div>
      </div>

      <q-table
        :rows="clientes"
        :columns="columns"
        row-key="id"
        :loading="loading"
        flat
        bordered
      >
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
              @click="toggleEstadoCliente(props.row)"
            >
              <q-tooltip>{{ props.row.ativo ? 'Desativar' : 'Ativar' }}</q-tooltip>
            </q-btn>
          </q-td>
        </template>

        <template v-slot:no-data>
          <div class="full-width row flex-center text-grey-7 q-gutter-sm q-pa-lg">
            <q-icon size="2em" name="sentiment_dissatisfied" />
            <span>Nenhum cliente encontrado</span>
          </div>
        </template>
      </q-table>
    </div>

    <q-dialog v-model="dialog" persistent>
      <q-card style="min-width: 450px">
        <q-card-section>
          <div class="text-h6">
            {{ editMode ? 'Editar Cliente' : 'Novo Cliente' }}
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
            v-model="formData.telefone"
            label="Telefone *"
            outlined
            dense
            class="q-mt-md"
            :rules="[val => !!val || 'Telefone é obrigatório']"
          />

          <q-input
            v-model="formData.email"
            label="Email"
            type="email"
            outlined
            dense
            class="q-mt-md"
          />

          <q-input
            v-model="formData.nif"
            label="NIF"
            outlined
            dense
            class="q-mt-md"
            maxlength="9"
          />

          <q-input
            v-model="formData.morada"
            label="Morada"
            outlined
            dense
            class="q-mt-md"
            type="textarea"
            rows="2"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey-7" v-close-popup />
          <q-btn
            unelevated
            label="Salvar"
            color="primary"
            @click="saveCliente"
            :disable="!formData.nome || !formData.telefone"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
</style>
