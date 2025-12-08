<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAuthStore } from '@/stores/auth';

const $q = useQuasar();
const router = useRouter();
const authStore = useAuthStore();

const tab = ref('login');
const email = ref('');
const password = ref('');
const userType = ref<'clinica' | 'funcionario'>('clinica');

async function onLogin() {
    if (!email.value || !password.value) {
        $q.notify({
            type: 'negative',
            message: 'Preencha email e password'
        });
        return;
    }

    const success = await authStore.login(email.value, password.value, userType.value);

    if (success) {
        $q.notify({
            type: 'positive',
            message: `Bem-vindo(a), ${authStore.user?.nome}!`,
            icon: 'check_circle'
        });
        router.push('/');
    } else {
        $q.notify({
            type: 'negative',
            message: authStore.error || 'Email ou password inválidos',
            icon: 'error'
        });
    }
}
</script>


<template>
    <div class="q-pa-md flex flex-center" style="min-height: 100vh;">
        <q-card flat bordered style="max-width: 450px; width: 100%;">
            <q-card-section class="text-center q-pb-none">
                <div class="text-h4 text-weight-bold text-primary">🐾 CVSL</div>
                <div class="text-h6 q-mt-sm">Sistema Veterinário</div>
                <div class="text-subtitle2 text-grey-7">by NoLife Dev Team</div>
            </q-card-section>

            <q-tabs v-model="tab" class="text-primary" dense>
                <q-tab label="Login" name="login" />
                <q-tab label="Sobre" name="register" />
            </q-tabs>

            <q-separator />

            <q-tab-panels v-model="tab" animated>
                <q-tab-panel name="login">
                    <!-- Seletor de tipo de utilizador -->
                    <div class="q-mb-md">
                        <div class="text-subtitle2 text-grey-7 q-mb-xs">Entrar como:</div>
                        <q-btn-toggle
                            v-model="userType"
                            spread
                            no-caps
                            rounded
                            toggle-color="primary"
                            color="grey-3"
                            text-color="grey-9"
                            :options="[
                                { label: '🏥 Clínica', value: 'clinica' },
                                { label: '👨‍⚕️ Funcionário', value: 'funcionario' }
                            ]"
                        />
                    </div>

                    <!-- Credenciais de teste -->
                    <q-banner 
                        v-if="userType === 'clinica'" 
                        dense 
                        rounded 
                        class="bg-blue-1 text-blue-9 q-mb-md"
                    >
                        <template v-slot:avatar>
                            <q-icon name="info" color="blue" />
                        </template>
                        <div class="text-caption">
                            <strong>Teste:</strong> clinica@teste.pt / teste123
                        </div>
                    </q-banner>

                    <q-banner 
                        v-else 
                        dense 
                        rounded 
                        class="bg-green-1 text-green-9 q-mb-md"
                    >
                        <template v-slot:avatar>
                            <q-icon name="info" color="green" />
                        </template>
                        <div class="text-caption">
                            <strong>Veterinário:</strong> vet@teste.pt / teste123<br>
                            <strong>Rececionista:</strong> rececionista@teste.pt / teste123
                        </div>
                    </q-banner>

                    <q-input 
                        rounded 
                        outlined 
                        bg-color="grey-2" 
                        v-model="email" 
                        label="Email"
                        type="email"
                        @keyup.enter="onLogin"
                        :rules="[val => !!val || 'Email é obrigatório']"
                    >
                        <template v-slot:prepend>
                            <q-icon name="email" />
                        </template>
                    </q-input>

                    <q-input 
                        rounded 
                        outlined 
                        bg-color="grey-2" 
                        class="q-mt-md" 
                        v-model="password" 
                        label="Password"
                        type="password"
                        @keyup.enter="onLogin"
                        :rules="[val => !!val || 'Password é obrigatória']"
                    >
                        <template v-slot:prepend>
                            <q-icon name="lock" />
                        </template>
                    </q-input>

                    <div class="q-mt-lg">
                        <q-btn 
                            unelevated
                            rounded
                            color="primary" 
                            label="Entrar" 
                            class="full-width"
                            size="md"
                            @click="onLogin"
                            :loading="authStore.isLoading"
                            :disable="!email || !password"
                        />
                    </div>
                </q-tab-panel>

                <q-tab-panel name="register">
                    <div class="text-center">
                        <q-icon name="pets" size="64px" color="primary" class="q-mb-md" />
                        <p class="text-h6 text-weight-medium">Sistema de Gestão Veterinária</p>
                        <p class="text-body2 text-grey-7">
                            Este sistema permite gerir clínicas veterinárias, 
                            incluindo clientes, animais, consultas, produtos e faturas.
                        </p>
                        <q-separator class="q-my-md" />
                        <p class="text-caption text-grey-6">
                            Para obter acesso, contacte a administração.
                        </p>
                    </div>
                </q-tab-panel>
            </q-tab-panels>
        </q-card>
    </div>
</template>

<style scoped>
.flex {
    display: flex;
}

.flex-center {
    align-items: center;
    justify-content: center;
}
</style>