<script setup lang="ts">

import { ref } from "vue";
import { useQuasar } from "quasar";
import { useEventosStore } from '@/stores/eventos';
import BarraPesquisas from '@/components/BarraPesquisas.vue';

const $q = useQuasar();

</script>

<template>
    <title>Centro Veterinario S.Lourenço</title>

    <div class="q-pa-md">
        <q-card flat bordered class="q-mx-xl">
            <q-card-section class="bg-grey-10 text-center">
                <div class="text-h4 text-white q-mb-sm">Centro Veterinario S.Lourenço</div>
                <div class="text-h5 text-white">Calendário escolar</div>
                <div class="text-subtitle1 text-grey-2 q-mt-sm">Ano letivo 2024/25</div>
            </q-card-section>

            <q-separator />

            <q-card-section>
                
                <BarraPesquisas />

                <div class="terms-container q-pa-md q-mb-md">
                    <div class="q-pa-md">
                        <q-markup-table>
                            <thead>
                                <tr>
                                    <th class="text-left">Titulo</th>
                                    <th class="text-right">Data</th>
                                    <th class="text-right">Disciplina</th>
                                    <th class="text-right">Modulo</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="text-left">Frozen Yogurt</td>
                                    <td class="text-right">159</td>
                                    <td class="text-right">6</td>
                                    <td class="text-right">24</td>
                                </tr>
                                <tr>
                                    <td class="text-left">Ice cream sandwich</td>
                                    <td class="text-right">237</td>
                                    <td class="text-right">9</td>
                                    <td class="text-right">37</td>
                                </tr>
                                <tr>
                                    <td class="text-left">Eclair</td>
                                    <td class="text-right">262</td>
                                    <td class="text-right">16</td>
                                    <td class="text-right">23</td>
                                </tr>
                                <tr>
                                    <td class="text-left">Cupcake</td>
                                    <td class="text-right">305</td>
                                    <td class="text-right">3.7</td>
                                    <td class="text-right">67</td>
                                </tr>
                            </tbody>
                        </q-markup-table>
                    </div>
                </div>
            </q-card-section>
        </q-card>
    </div>
</template>