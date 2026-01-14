<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useQuasar } from 'quasar';
import { animaisService } from '@/services/animaisService';
import { clientesService } from '@/services/clientesService';
import { authService } from '@/services/authService';
import { fasBedPulse } from '@quasar/extras/fontawesome-v6';
import type { Animal } from '@/types/animais';

const $q = useQuasar();

const animais = ref<Animal[]>([]);
const loading = ref(false);
const dialog = ref(false);
const editMode = ref(false);
const incluirInativos = ref(false);
const clientes = ref<any[]>([]);

const isFuncionario = computed(() => authService.isFuncionario());

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

const formData = ref<Animal>({
  transponder: '',
  nome: '',
  especie: '',
  raca: '',
  data_nascimento: undefined,
  sexo: '',
  id_cliente: null,
  id_clinica: clinicaId.value,
  ativo: true
});

const columns = [
  { name: 'transponder', label: 'Transponder', field: 'transponder', align: 'left' as const },
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, sortable: true },
  { name: 'especie', label: 'Espécie', field: 'especie', align: 'left' as const, sortable: true },
  { name: 'raca', label: 'Raça', field: 'raca', align: 'left' as const },
  { name: 'data_nascimento', label: 'Data Nascimento', field: 'data_nascimento', align: 'left' as const },
  { name: 'sexo', label: 'Sexo', field: 'sexo', align: 'center' as const },
  { name: 'nomeCliente', label: 'Cliente', field: 'nomeCliente', align: 'left' as const },
  { name: 'actions', label: 'Ações', field: 'id', align: 'center' as const }
];


async function loadAnimais() {
  loading.value = true;
  try {
    animais.value = await animaisService.getAll(incluirInativos.value);
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao carregar animais'
    });
  } finally {
    loading.value = false;
  }
}

async function loadClientes() {
  clientes.value = await clientesService.getAll(false); // só ativos
}

function openNewDialog() {
  editMode.value = false;
  formData.value = {
    transponder: '',
    nome: '',
    especie: '',
    raca: '',
    data_nascimento: undefined,
    sexo: '',
    id_cliente: 0,
    id_clinica: clinicaId.value,
    ativo: true
  };
  dialog.value = true;
}

function openEditDialog(animal: Animal) {
  editMode.value = true;
  formData.value = { ...animal };
  dialog.value = true;
}

async function saveCliente() {
  try {
    if (editMode.value && formData.value.id) {
      await animaisService.update(formData.value.id, formData.value);
      $q.notify({
        type: 'positive',
        message: 'Cliente atualizado com sucesso!'
      });
    } else {
      await animaisService.create(formData.value);
      $q.notify({
        type: 'positive',
        message: 'Cliente criado com sucesso!'
      });
    }

    dialog.value = false;
    await loadAnimais();
  } catch (error: any) {
    $q.notify({
      type: 'negative',
      message: error.message || 'Erro ao salvar cliente'
    });
  }
}

async function toggleEstadoCliente(animal: Animal) {
  const novoEstado = !animal.ativo;
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
      if (animal.ativo) {
        await animaisService.updateStatus(animal.id!, false);
      } else {
        await animaisService.updateStatus(animal.id!, true);
      }

      $q.notify({
        type: 'positive',
        message: `Cliente ${novoEstado ? 'ativado' : 'desativado'} com sucesso!`
      });
      await loadAnimais();
    } catch (error: any) {
      $q.notify({
        type: 'negative',
        message: error.message || 'Erro ao alterar estado do cliente'
      });
    }
  });
}

onMounted(() => {
  loadAnimais();
  loadClientes();
});
</script>

<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-md">
        <div class="col">
          <div class="text-h5">Animais</div>
          <div class="text-subtitle2 text-grey-7">
            Gerir animais da clínica
          </div>
        </div>
        <div class="col-auto row items-center q-gutter-md">
          <q-toggle v-model="incluirInativos" label="Mostrar inativos" @update:model-value="loadAnimais" />
          <q-btn v-if="isFuncionario" color="primary" label="Novo Animal" icon="add" @click="openNewDialog" />
        </div>
      </div>

      <q-table :rows="animais" :columns="columns" row-key="id" :loading="loading" flat bordered>
        <template v-slot:body-cell-actions="props">
          <q-td :props="props">
            <q-btn v-if="isFuncionario" flat round dense color="primary" icon="edit" @click="openEditDialog(props.row)">
              <q-tooltip>Editar</q-tooltip>
            </q-btn>
            <q-btn flat round dense :color="props.row.ativo ? 'negative' : 'positive'"
              :icon="props.row.ativo ? 'block' : 'check_circle'" @click="toggleEstadoCliente(props.row)">
              <q-tooltip>{{ props.row.ativo ? 'Desativar' : 'Ativar' }}</q-tooltip>
            </q-btn>
          </q-td>
        </template>

        <template v-slot:no-data>
          <div class="full-width row flex-center text-grey-7 q-gutter-sm q-pa-lg">
            <q-icon size="2em" name="sentiment_dissatisfied" />
            <span>Nenhum animal encontrado</span>
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

          <q-input v-model="formData.transponder" label="Transponder" outlined dense maxlength="15" class="q-mt-md" />

          <q-input v-model="formData.nome" label="Nome *" outlined dense class="q-mt-md"
            :rules="[val => !!val || 'Nome é obrigatório']" />

          <q-input v-model="formData.especie" label="Espécie" outlined dense class="q-mt-md" />

          <q-input v-model="formData.raca" label="Raça" outlined dense class="q-mt-md" />

          <q-input v-model="formData.data_nascimento" label="Data Nascimento" outlined dense type="date"
            class="q-mt-md" />

          <q-select v-model="formData.sexo" label="Sexo" outlined dense class="q-mt-md" :options="[
            { label: 'Macho', value: 'M' },
            { label: 'Fêmea', value: 'F' }
          ]" option-value="value" option-label="label" />

          <q-select v-model="formData.id_cliente" label="Cliente *" outlined dense class="q-mt-md" :options="clientes"
            option-value="id" option-label="nome" use-input  
            fill-input 
            input-debounce="0" 
            emit-value
            map-options
            :rules="[val => val !== null && val !== 0 || 'Cliente é obrigatório']"
            clearable
            />



        </q-card-section>


        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey-7" v-close-popup />
          <q-btn unelevated label="Salvar" color="primary" @click="saveCliente"
            :disable="!formData.nome || !formData.transponder" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped></style>
