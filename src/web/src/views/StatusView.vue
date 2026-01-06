<script setup lang="ts">
import { QBtn, QInput } from 'quasar';
import { ref } from 'vue';

const currentStatus = ref(true);
</script>

<template>

    <title>Centro Veterinario S.Lourenço</title>

    <q-card class="q-mx-xl q-mt-xl" flat bordered>
        <q-card-section class="text-center">
            <div class="q-mb-xl text-grey-9">
                <q-spinner-rings v-if="currentStatus" color="green" size="100px" />
                <q-spinner-gears v-else color="orange" size="100px" />
                <div v-if="currentStatus" class="text-h5" style="color: #357870;">Todos os sistemas operacionais</div>
                <div v-else class="text-h5" color="grey-10">Alguns sistemas em manuntenção</div>
            </div>
            <div>
                <div class="text-grey-8">
                    <q-spinner-rings color="green" size="sm" />
                    Online
                    <q-spinner-ios class="q-ml-md" color="blue" size="sm" />
                    Updating
                    <q-spinner-gears class="q-ml-md" color="orange" size="sm" />
                    Manuntenção
                    <q-spinner-oval class="q-ml-md" color="red" size="sm" />
                    Offline
                </div>

            </div>
        </q-card-section>

        <q-separator spaced inset />

        <q-card-section>
            <q-list class="bg-white q-pa-md q-rounded-md">
                <q-item>
                    <q-item-label class="q-mt-md q-ml-xs">Status dos servicos</q-item-label>
                </q-item>

                <q-separator spaced inset />

                <q-item>
                    <q-item-section avatar>
                        <q-spinner-gears class="q-ml-sm" color="orange" size="sm" />
                    </q-item-section>
                    <q-item-section>
                        <q-item-label class="q-mt-xs q-ml-xs">Admin</q-item-label>
                        <q-item-label class="q-mt-xs q-ml-xs" caption>Em manuntenção</q-item-label>
                    </q-item-section>
                </q-item>
                
                <q-separator spaced inset />

                <q-item>
                    <q-item-section avatar>
                        <q-spinner-oval class="q-ml-sm" color="red" size="sm" />
                    </q-item-section>
                    <q-item-section>
                        <q-item-label class="q-mt-xs q-ml-xs">Calendário</q-item-label>
                        <q-item-label class="q-mt-xs q-ml-xs" caption>Nenhum problema</q-item-label>
                    </q-item-section>
                </q-item>

                <q-separator spaced inset />

                <q-item>
                    <q-item-section avatar>
                        <q-spinner-rings class="q-ml-sm" color="green" size="sm" />
                    </q-item-section>
                    <q-item-section>
                        <q-item-label class="q-mt-xs q-ml-xs">Resumos</q-item-label>
                        <q-item-label class="q-mt-xs q-ml-xs" caption>Nenhum problema</q-item-label>
                    </q-item-section>
                </q-item>

                <q-separator spaced inset />

                <q-item>
                    <q-item-section avatar>
                        <q-spinner-ios class="q-ml-sm" color="blue" size="sm" />
                    </q-item-section>
                    <q-item-section>
                        <q-item-label class="q-mt-xs q-ml-xs">Animais</q-item-label>
                        <q-item-label class="q-mt-xs q-ml-xs" caption>Nenhum problema</q-item-label>
                    </q-item-section>
                </q-item>

                <q-separator spaced inset />

                <q-item>
                    <q-item-section avatar>
                        <q-spinner-rings class="q-ml-sm" color="green" size="sm" />
                    </q-item-section>
                    <q-item-section>
                        <q-item-label class="q-mt-xs q-ml-xs">Funcionarios</q-item-label>
                        <q-item-label class="q-mt-xs q-ml-xs" caption>Nenhum problema</q-item-label>
                    </q-item-section>
                </q-item>
            </q-list>
        </q-card-section>
    </q-card>
</template>