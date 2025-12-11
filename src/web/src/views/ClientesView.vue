<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useQuasar } from 'quasar';
import { clientesService, type Cliente } from '@/services/clientesService';

const $q = useQuasar();

const clientes = ref<Cliente[]>([]);
const loading = ref(false);
const dialog = ref(false);
const editMode = ref(false);

const formData = ref<Cliente>({
  nome: '',
  telefone: '',
  email: '',
  nif: '',
  morada: '',
  clinicaId: 1 // TODO: obter da clínica logada
});

const columns = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, sortable: true },
  { name: 'telefone', label: 'Telefone', field: 'telefone', align: 'left' as const },
  { name: 'email', label: 'Email', field: 'email', align: 'left' as const },
  { name: 'nif', label: 'NIF', field: 'nif', align: 'left' as const },
  { name: 'actions', label: 'Ações', field: 'id', align: 'center' as const }
];

async function loadClientes() {
  loading.value = true;
  try {
    clientes.value = await clientesService.getAll();
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
    clinicaId: 1
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

async function deleteCliente(id: number) {
  $q.dialog({
    title: 'Confirmar',
    message: 'Tem certeza que deseja remover este cliente?',
    cancel: true,
    persistent: true
  }).onOk(async () => {
    try {
      await clientesService.delete(id);
      $q.notify({
        type: 'positive',
        message: 'Cliente removido com sucesso!'
      });
      await loadClientes();
    } catch (error: any) {
      $q.notify({
        type: 'negative',
        message: error.message || 'Erro ao remover cliente'
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
        <div class="col-auto">
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
              color="negative"
              icon="delete"
              @click="deleteCliente(props.row.id)"
            >
              <q-tooltip>Remover</q-tooltip>
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

    <!-- Dialog Criar/Editar -->
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
