<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import Sidebar from '@/components/SideBar.vue';
import NotificationCenter from '@/components/NotificationCenter.vue';
import packageJson from '../package.json';

const APP_VERSION = packageJson.version;

const router = useRouter();
const authStore = useAuthStore();
const sideBar = ref(false);
</script>

<template>
  <q-layout>
    <!-- Sidebar -->
    <Sidebar v-if="authStore.isAuthenticated" v-model:sideBar="sideBar" />

    <!-- Header -->
    <q-header elevated style="background-color: #357870;" class="text-white shadow-2">
      <q-toolbar>
        <!-- Botão para abrir/fechar a sidebar -->
        <q-btn v-if="authStore.isAuthenticated" flat dense icon="menu" class="q-mr-sm" aria-label="Menu"
          @click="sideBar = !sideBar" />

        <q-space />

        <q-toolbar-title class="text-weight-bold q-ml-xl q-pl-xl" style="text-align: center; margin-left: 150px;">
          Centro Veterinario S.Lourenço
          <q-badge outline align="top" color="white" :label="'v' + APP_VERSION" />
        </q-toolbar-title>

        <q-space />

        <template v-if="authStore.isAuthenticated">          
          <div class="row items-center">
            <q-icon name="network_wifi" color="green" class="cursor-pointer q-mr-sm">
              <q-popup-proxy :offset="[10, 10]">
                <q-banner class="bg-green text-white">
                  <template v-slot:avatar>
                    <q-icon name="network_wifi" />
                  </template>
                  Todos os modulos estão online. A app está a funcionar.
                </q-banner>
              </q-popup-proxy>
            </q-icon>
            <div class="q-mr-md">
              Status
            </div>
          </div>


          <notification-center />
          <q-btn round color="grey-10" class="q-ml-sm" icon="account_circle" @click="router.push('/conta')" />
        </template>

      </q-toolbar>
    </q-header>

    <!-- Conteúdo da página -->
    <q-page-container>
      <q-page padding>
        <RouterView />
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<!-- App.vue - ADICIONA ISTO NO FINAL -->
<style>
/* QUASAR LAYOUT SCROLL FIX - GLOBAL */
.q-layout__container {
  height: 100vh !important;
  overflow: hidden !important;
}

.q-layout-page-container {
  height: calc(100vh - 64px) !important;
  overflow: hidden !important;
  display: flex !important;
}

/* Sidebar NUNCA faz scroll */
.q-drawer {
  overflow: hidden !important;
  position: fixed !important;
  z-index: 1000 !important;
}

/* Conteúdo das páginas SEMPRE scroll independente */
.q-page {
  height: 100vh !important;
  overflow-y: auto !important;
  overflow-x: hidden !important;
  padding-bottom: 24px !important;
  contain: layout style !important;
}

/* Remove scroll da página container */
.q-page-container {
  overflow: hidden !important;
}

/* Scrollbar custom limpa */
.q-page::-webkit-scrollbar {
  width: 6px;
}

.q-page::-webkit-scrollbar-track {
  background: transparent !important;
}

.q-page::-webkit-scrollbar-thumb {
  background: rgba(53, 120, 112, 0.5);
  border-radius: 3px;
}

.q-page::-webkit-scrollbar-thumb:hover {
  background: rgba(53, 120, 112, 0.8);
}

/* StockView específico */
.stock-layout {
  height: 100vh;
  display: flex;
  overflow: hidden;
}

.stock-sidebar {
  flex: 0 0 380px;
  height: 100vh;
  position: sticky;
  top: 64px;
}

.stock-content {
  flex: 1;
  height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
}

/* Mobile */
@media (max-width: 1023px) {
  .q-layout-page-container {
    height: auto !important;
  }
  .stock-layout {
    flex-direction: column;
  }
}

/* ADICIONA estas linhas no teu CSS atual do App.vue */
.q-header {
  position: fixed !important;
  top: 0 !important;
  z-index: 1100 !important;
  height: 64px !important; /* altura exata do teu header */
}

.q-layout__header {
  position: fixed !important;
  top: 0 !important;
  height: 64px !important;
}

/* Ajusta altura do conteúdo (header 64px) */
.q-layout-page-container,
.q-page {
  height: calc(100vh - 64px) !important; /* CORRIGIDO: -64px */
}

</style>
