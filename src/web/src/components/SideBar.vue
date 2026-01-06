<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useQuasar } from 'quasar';

const sideBar = defineModel<boolean>('sideBar');
const router = useRouter();
const authStore = useAuthStore();
const $q = useQuasar();

const handleLogout = () => {
    authStore.logout();
    $q.notify({
        type: 'positive',
        message: 'Logout! Bom descanso'
    });
    router.push('/login');
};

</script>

<template>
  <q-drawer v-model="sideBar" side="left" bordered :width="280" style="background-color: #357870;" class="text-white">
    <q-list padding>
      <q-item class="q-mb-md">
        <q-item-section>
          <q-item-label class="text-h5 text-weight-bold">Centro Veterinario S.Lourenço</q-item-label>
        </q-item-section>
      </q-item>

      <!-- items do Menu -->
      <q-item clickable v-ripple @click="router.push('/')">
        <q-item-section avatar>
          <q-icon name="home" color="white" />
        </q-item-section>
        <q-item-section>
          <q-item-label>Dashboard</q-item-label>
        </q-item-section>
      </q-item>

      <!-- Menu para Clínica -->
      <template v-if="authStore.isClinica">
        <q-item clickable v-ripple @click="router.push('/clientes')">
          <q-item-section avatar>
            <q-icon name="people" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Clientes</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/animais')">
          <q-item-section avatar>
            <q-icon name="pets" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Animais</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/funcionarios')">
          <q-item-section avatar>
            <q-icon name="person" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Funcionarios</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/resumos')">
          <q-item-section avatar>
            <q-icon name="event" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Consultas</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/avaliar')">
          <q-item-section avatar>
            <q-icon name="receipt" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Faturas</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/relatorios')">
          <q-item-section avatar>
            <q-icon name="analytics" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Relatórios</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/configuracoes')">
          <q-item-section avatar>
            <q-icon name="settings" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Configurações</q-item-label>
          </q-item-section>
        </q-item>
      </template>

      <!-- Menu para Funcionário -->
      <template v-if="authStore.isFuncionario">
        <q-item clickable v-ripple @click="router.push('/clientes')">
          <q-item-section avatar>
            <q-icon name="people" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Clientes</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/animais')">
          <q-item-section avatar>
            <q-icon name="pets" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Animais</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/consultas')">
          <q-item-section avatar>
            <q-icon name="event" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Consultas</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/agenda')">
          <q-item-section avatar>
            <q-icon name="calendar_today" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Agenda</q-item-label>
          </q-item-section>
        </q-item>
      </template>

      <div class="fixed-bottom" style="margin-bottom: 0; padding-bottom: 0;">
        <q-separator dark class="q-mb-sm" />

        <!-- Info do utilizador -->
        <q-item v-if="authStore.user" class="q-mb-xs">
          <q-item-section color="red" avatar>
            <q-avatar color="white" text-color="primary" icon="account_circle" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-medium">{{ authStore.user.nome }}</q-item-label>
            <q-item-label caption class="text-white" style="opacity: 0.7;">
              {{ authStore.isClinica ? 'Clínica' : 'Funcionário' }}
            </q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable v-ripple @click="router.push('/status')">
          <q-item-section avatar>
            <q-icon name="info" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Status</q-item-label>
          </q-item-section>
        </q-item>
        
        <q-item clickable v-ripple @click="handleLogout">
          <q-item-section avatar>
            <q-icon name="logout" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Logout</q-item-label>
          </q-item-section>
        </q-item>
      </div>
    </q-list>
  </q-drawer>
</template>

<style scoped>
.q-drawer {
  background-color: #357870;
  border-right: 1px solid rgba(255, 255, 255, 0.1) !important;
}

.q-item:hover {
  background-color: rgba(255, 255, 255, 0.1) !important;
  border-radius: 8px;
}

.q-item__label {
  color: white !important;
}
</style>
