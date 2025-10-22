<script setup lang = "ts" >
import { ref } from 'vue';
import { useMeta } from 'quasar';

useMeta({ title: 'Centro Veterinario S.Lourenço' });

</script>

<template>
  <title>Centro Veterinario S.Lourenço</title>

  <div class="q-pa-md">
    <q-card>
      <q-parallax src="https://cdn.quasar.dev/img/parallax1.jpg" :height="150" />

      <q-card-section>
        <div class="text-h6">Regras de utilização</div>
        <div class="text-subtitle2">by NoLife Dev Team</div>
      </q-card-section>
    </q-card>
  </div>

</template>
