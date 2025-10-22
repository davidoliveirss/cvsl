<script setup lang="ts">
import { ref } from 'vue';
import { useMeta } from 'quasar';

const tab = ref('disciplina1');
const disciplinas = ref([
  {
    abreviatura: "TLP",
    name: 'disciplina1',
    descricao: "Disciplina de programação, modulos 1,2,3,4 ",
    label: 'Programação',
    icon: 'terminal',
    image: 'https://pactoparaoclima.portodigital.pt/wp-content/uploads/2023/02/AE-Fontes-Pereira-de-Melo-5.jpg'
  },
  {
    abreviatura: "MAT",
    name: 'disciplina2',
    descricao: "Disciplina de Matmatica B, modulos 6,7,8,9",
    label: 'Matmatica',
    icon: 'calculate',
    image: 'https://pactoparaoclima.portodigital.pt/wp-content/uploads/2023/02/AE-Fontes-Pereira-de-Melo-5.jpg'
  },
  {
    abreviatura: "ASO",
    name: 'disciplina3',
    descricao: "Disciplina de arquitetura, modulos 12,2,34",
    label: 'Arquitetura de sistemas operativos',
    icon: 'dns',
    image: 'https://pactoparaoclima.portodigital.pt/wp-content/uploads/2023/02/AE-Fontes-Pereira-de-Melo-5.jpg'
  },

]);
</script>

<template>

  <title>Centro Veterinario S.Lourenço</title>

  <div class="q-pa-md">
    <q-card class="my-card" flat bordered>
      <q-card-section class="bg-grey-10 text-center">
        <div class="text-h5 text-white">Sistema resumos P2PR</div>
        <div class="text-subtitle1 text-grey-4 q-mt-sm">Ano letivo 2024/25</div>
      </q-card-section>

      <!-- Navegação Estilo Dashboard -->
      <q-scroll-area horizontal :visible="true" class="q-pa-sm bg-grey-2" style="height: 150px">
        <div class="row no-wrap q-gutter-md">
          <q-card v-for="disciplina in disciplinas" :key="disciplina.name" class="cursor-pointer disciplina-card q-mt-lg"
            :class="{ 'active-card': tab === disciplina.name }" @click="tab = disciplina.name" style="min-width: 200px">
            <q-img :src="disciplina.image" style="height: 100px">
              <div class="absolute-bottom text-subtitle2 text-center">
                {{ disciplina.abreviatura }}
              </div>
            </q-img>
          </q-card>
        </div>
      </q-scroll-area>

      <!-- Conteúdo -->
      <q-tab-panels v-model="tab" animated transition-prev="scale" transition-next="scale">
        <q-tab-panel v-for="disciplina in disciplinas" :name="disciplina.name" :key="disciplina.name" class="q-pa-none">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-md-3">
              <q-card class="full-height">
                <q-card-section class="text-white" style="background-color: #357870;">
                  <div class="text-h6">{{ disciplina.label }}</div>
                  <q-icon :name="disciplina.icon" size="lg" class="q-mt-md" />
                </q-card-section>

                <q-card-section>
                  <div class="text-caption text-grey-8">
                    {{ disciplina.descricao }}
                  </div>
                </q-card-section>
              </q-card>
            </div>

            <div class="col-12 col-md-9">
              <q-markup-table flat bordered class="shadow-1">
                <thead class="bg-grey-10 text-white">
                  <tr>
                    <th class="text-left">Evento</th>
                    <th class="text-right">Data</th>
                    <th class="text-right">Responsável</th>
                    <th class="text-right">Local</th>
                    <th class="text-right">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <!-- Seus dados aqui -->
                </tbody>
              </q-markup-table>
            </div>
          </div>
        </q-tab-panel>
      </q-tab-panels>
    </q-card>
  </div>
</template>

<style lang="sass">
.disciplina-card
  transition: transform 0.3s, box-shadow 0.3s
  &:hover
    transform: translateY(-5px)
    box-shadow: $shadow-5

.active-card
  border: 2px solid $primary
  box-shadow: $shadow-3
</style>