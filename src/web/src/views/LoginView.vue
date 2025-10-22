<script setup lang="ts">
import { ref } from 'vue';
import { useQuasar } from 'quasar'; // IMPORTANTE!

const $q = useQuasar(); // ACEDER AO CONTEXTO DO QUASAR

const tab = ref('login'); // Controle das tabs
const name = ref('');
const surename = ref('');
const password = ref('');
const birth_date = ref('');
const checkbox = ref(false);

function onRegister() {
    console.log("Registo")
}
</script>


<template>
    <div class="q-pa-md">
        <q-card flat bordered>
            <q-card-section>
                <div v-if="tab == 'login'" class="text-h5 text-center">Login</div>
                <div v-if="tab == 'register'" class="text-h5 text-center">Regista-te</div>
                <div class="text-subtitle2 text-center">by NoLife Dev Team</div>
            </q-card-section>

            <q-tabs v-model="tab" class="text-teal">
                <q-tab label="Login" class="text-grey-9" name="login" />
                <q-tab label="Register" class="text-grey-9" name="register" />
            </q-tabs>

            <q-separator />

            <q-tab-panels v-model="tab" animated>
                <q-tab-panel name="login" style="color: #357870;">
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" v-model="name" label="Email" />
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" class="q-mt-md" v-model="name" label="Password" />
                    <div class="q-mt-md q-mr-md" style="text-align: right;">
                        <q-btn push color="grey-9" label="Login" />
                    </div>
                </q-tab-panel>

                <q-tab-panel name="register" animated>
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" v-model="name" label="Nome" lazy-rules
                        :rules="[val => val && val.length > 0 || 'Tens que escrever algo']" />
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" v-model="surename"
                        label="Sobrenome" lazy-rules
                        :rules="[val => val && val.length > 0 || 'Tens que escrever algo']" />
                    <q-input rounded outlined  bg-color="grey-3" color="grey-10" v-model="name"
                        hint="Email escolar ou pessoal" label="Email" lazy-rules
                        :rules="[val => val && val.length > 0 || 'Tens que escrever algo']" />
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" class="q-mt-md" v-model="birth_date"
                        label="Data nascimento" />
                    <q-input rounded outlined bg-color="grey-3" color="grey-10" class="q-mt-lg" v-model="password"
                        label="Password" />

                    <div class="q-mt-md q-mr-md" style="text-align: right;">
                        <q-checkbox v-model="checkbox" color="grey-9">
                            <template v-slot:default>
                                <span>
                                    Eu li e aceito os
                                    <router-link to="/termos-e-condicoes"
                                        style="text-decoration: underline; color: #357870;" @click.stop>
                                        termos de utilização
                                    </router-link>
                                </span>
                            </template>
                        </q-checkbox>
                        <q-btn push class="q-ml-md" color="grey-9" @click="onRegister" label="Register" />
                    </div>
                </q-tab-panel>
            </q-tab-panels>
        </q-card>
    </div>
</template>

<style scoped></style>