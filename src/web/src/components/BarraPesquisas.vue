<script setup lang="ts">
import { QBtn, QInput } from 'quasar';
import { ref } from 'vue';

const pesquisa = ref(null);

</script>

<template>
    <div class="q-px-lg text-h9">
        <q-input v-model="pesquisa" color="grey-9" label="Pesquisa" />
        <q-btn color="grey-9" @click="$emit('onsearch', pesquisa)">Pesquisar</q-btn>
    </div>
</template>

<style scoped></style>