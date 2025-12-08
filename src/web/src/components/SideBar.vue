<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const sideBar = defineModel<boolean>('sideBar'); // usa model para controlar estado do drawer
const router = useRouter();
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

      <q-item clickable v-ripple @click="router.push('/clientes')">
        <q-item-section avatar>
          <q-icon name="event" color="white" />
        </q-item-section>
        <q-item-section>
          <q-item-label>Clientes</q-item-label>
        </q-item-section>
      </q-item>

      <q-item clickable v-ripple @click="router.push('/resumos')">
        <q-item-section avatar>
          <q-icon name="school" color="white" />
        </q-item-section>
        <q-item-section>
          <q-item-label>Resumos</q-item-label>
        </q-item-section>
      </q-item>

      <q-item clickable v-ripple @click="router.push('/resumos')">
        <q-item-section avatar>
          <q-icon name="dashboard" color="white" />
        </q-item-section>
        <q-item-section>
          <q-item-label>Gerir resumos</q-item-label>
        </q-item-section>
      </q-item>

      <q-item clickable v-ripple @click="router.push('/avaliar')">
        <q-item-section avatar>
          <q-icon name="rate_review" color="white" />
        </q-item-section>
        <q-item-section>
          <q-item-label>Feedback</q-item-label>
        </q-item-section>
      </q-item>

      <div class="fixed-bottom" style="margin-bottom: 0; padding-bottom: 0;">
        <q-item clickable v-ripple @click="router.push('/status')" style="margin-bottom: 0;">
          <q-item-section avatar>
            <q-icon name="running_with_errors" color="white" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Status</q-item-label>
          </q-item-section>
        </q-item>
        
        <q-item clickable v-ripple @click="router.push('/conta')">
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
