<script setup lang="ts">
import { ref } from 'vue';
import { useQuasar } from 'quasar';
import { useMeta } from 'quasar';

const $q = useQuasar();

const avaliacao = ref(null);
const melhorias = ref(null);
const accept = ref(false);
const ratingModel = ref(2); // Movido para cá e declarado corretamente

const onSubmit = () => {
  if (accept.value !== true) {
    $q.notify({
      color: 'red-5',
      textColor: 'white',
      icon: 'warning',
      message: 'You need to accept the license and terms first'
    });
  }
  else {
    $q.notify({
      color: 'green-4',
      textColor: 'white',
      icon: 'cloud_done',
      message: 'Submitted'
    });
  }
};

const onReset = () => {
  avaliacao.value = null;
  melhorias.value = null;
};
</script>

<template>

  <title>Centro Veterinario S.Lourenço</title>

  <div class="q-pa-md">
    <q-card class="my-card" flat bordered>
      <q-card-section class="bg-grey-10 text-center">
        <div class="text-h5 text-white">Sistema de avaliações</div>
        <div class="text-subtitle1 text-grey-4 q-mt-sm">Ano letivo 2024/25</div>
      </q-card-section>
      <div style="text-align: center;">
        <q-form @submit="onSubmit" @reset="onReset" class="q-gutter-md q-px-md">
          <q-input filled v-model="avaliacao" color="grey-10" class="q-mt-xl" label="Avaliação *"
            hint="O que achas sobre a aplicação" lazy-rules
            :rules="[val => val && val.length > 0 || 'Tens que escrever algo']" />
          <q-input filled v-model="melhorias" label="Melhorias" color="grey-10" hint="O que achas que podia ser melhorado" />

          <div>
            <q-rating v-model="ratingModel" size="2em" :max="4" color="grey-10">
              <template v-slot:tip-1>
                <q-tooltip>Terrivel</q-tooltip>
              </template>
              <template v-slot:tip-2>
                <q-tooltip>Nada mal</q-tooltip>
              </template>
              <template v-slot:tip-3>
                <q-tooltip>Bom</q-tooltip>
              </template>
              <template v-slot:tip-4>
                <q-tooltip>Incrivel!</q-tooltip>
              </template>
            </q-rating>
          </div>

          <div>
            <q-toggle v-model="accept" color="grey-9" class="q-mt-md">
              <template v-slot:default>
                <span>
                  Eu li e aceito os
                  <router-link to="/termos-e-condicoes" style="text-decoration: underline; color: #357870;" @click.stop>
                    termos de utilização
                  </router-link>
                </span>
              </template>
            </q-toggle>
          </div>

          <div class="q-pb-md">
            <q-btn label="Redefinir" type="reset" color="white" flat class="text-grey-10 q-mr-md" @click="onReset" />
            <q-btn label="Enviar" type="submit" color="grey-9" :disable="!accept">
              <q-tooltip v-if="!accept" anchor="center right" self="center left" transition-show="scale"
                transition-hide="fade" class="bg-grey-9 text-body1">
                <div class="row items-center no-wrap">
                  <q-icon name="warning" class="q-mr-xs" />
                  Por favor, aceite os termos e condições para continuar
                </div>
              </q-tooltip>
            </q-btn>
          </div>
        </q-form>
      </div>
    </q-card>
  </div>

</template>
