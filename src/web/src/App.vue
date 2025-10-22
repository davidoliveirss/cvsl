<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import Sidebar from '@/components/SideBar.vue';
import NotificationCenter from '@/components/NotificationCenter.vue';
import packageJson from '../package.json';

const APP_VERSION = packageJson.version;

const router = useRouter();
const sideBar = ref(false);
</script>

<template>
  <q-layout>
    <!-- Sidebar -->
    <Sidebar v-model:sideBar="sideBar" />

    <!-- Header -->
    <q-header elevated style="background-color: #357870;" class="text-white shadow-2">
      <q-toolbar>
        <!-- Botão para abrir/fechar a sidebar -->
        <q-btn flat dense icon="menu" class="q-mr-sm" aria-label="Menu" @click="sideBar = !sideBar" />

        <q-space />

        <q-toolbar-title class="text-weight-bold q-ml-xl q-pl-xl" style="text-align: center; margin-left: 150px;">
          Centro Veterinario S.Lourenço
          <q-badge outline align="top" color="white" :label="'v' + APP_VERSION" />
        </q-toolbar-title>

        <q-space />

        <q-badge color="green" rounded class="q-mr-sm" />
        <div class="q-mr-md">
          Status
        </div>


        <notification-center />
        <q-btn round color="grey-10" class="q-ml-sm" icon="account_circle" @click="router.push('/conta')" />
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
