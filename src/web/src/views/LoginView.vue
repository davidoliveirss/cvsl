<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAuthStore } from '@/stores/auth';

const $q = useQuasar();
const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const userType = ref<'clinica' | 'funcionario'>('clinica');

async function onLogin() {
    if (!email.value || !password.value) {
        $q.notify({
            type: 'negative',
            message: 'Preencha email e password',
            position: 'top'
        });
        return;
    }

    const success = await authStore.login(email.value, password.value, userType.value);

    if (success) {
        $q.notify({
            type: 'positive',
            message: `Bem-vindo(a), ${authStore.user?.nome}!`,
            icon: 'check_circle',
            position: 'top'
        });
        router.push('/');
    } else {
        $q.notify({
            type: 'negative',
            message: authStore.error || 'Email ou password inválidos',
            icon: 'error',
            position: 'top'
        });
    }
}
</script>

<template>
    <q-layout>
        <q-page-container>
            <q-page class="flex flex-center" style="background: linear-gradient(135deg, #357870 0%, #2a5f5a 100%);">
                <q-card style="width: 100%; max-width: 450px; border-radius: 16px;" class="shadow-10">
                    <!-- Header -->
                    <q-card-section class="text-center" style="background-color: #357870; color: white; padding: 32px;">
                        <div class="text-h4 text-weight-bold q-mb-sm">
                            🐾 Centro Veterinário
                        </div>
                        <div class="text-h6">
                            S. Lourenço
                        </div>
                        <div class="text-caption q-mt-sm" style="opacity: 0.9;">
                            Sistema de Gestão Veterinária
                        </div>
                    </q-card-section>

                    <q-separator />

                    <q-card-section style="padding: 32px;">
                        <!-- Seletor de tipo de utilizador -->
                        <div class="q-mb-lg">
                            <div class="text-subtitle2 text-grey-8 q-mb-sm text-weight-medium">Entrar como:</div>
                            <q-btn-toggle
                                v-model="userType"
                                spread
                                no-caps
                                rounded
                                unelevated
                                toggle-color="primary"
                                color="grey-3"
                                text-color="grey-9"
                                style="border: 2px solid #357870;"
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
                            class="q-mb-lg"
                            style="background-color: #e8f5f3; border-left: 4px solid #357870;"
                        >
                            <template v-slot:avatar>
                                <q-icon name="info" style="color: #357870;" />
                            </template>
                            <div class="text-caption" style="color: #2a5f5a;">
                                <strong>Teste:</strong> clinica@teste.pt / teste123
                            </div>
                        </q-banner>

                        <q-banner 
                            v-else 
                            dense 
                            rounded 
                            class="q-mb-lg"
                            style="background-color: #e8f5f3; border-left: 4px solid #357870;"
                        >
                            <template v-slot:avatar>
                                <q-icon name="info" style="color: #357870;" />
                            </template>
                            <div class="text-caption" style="color: #2a5f5a;">
                                <strong>Veterinário:</strong> vet@teste.pt / teste123<br>
                                <strong>Rececionista:</strong> rececionista@teste.pt / teste123
                            </div>
                        </q-banner>

                        <!-- Formulário -->
                        <q-input 
                            rounded 
                            outlined 
                            v-model="email" 
                            label="Email"
                            type="email"
                            bg-color="grey-1"
                            color="primary"
                            @keyup.enter="onLogin"
                            :rules="[val => !!val || 'Email é obrigatório']"
                            class="q-mb-md"
                        >
                            <template v-slot:prepend>
                                <q-icon name="email" style="color: #357870;" />
                            </template>
                        </q-input>

                        <q-input 
                            rounded 
                            outlined 
                            v-model="password" 
                            label="Password"
                            type="password"
                            bg-color="grey-1"
                            color="primary"
                            @keyup.enter="onLogin"
                            :rules="[val => !!val || 'Password é obrigatória']"
                            class="q-mb-lg"
                        >
                            <template v-slot:prepend>
                                <q-icon name="lock" style="color: #357870;" />
                            </template>
                        </q-input>

                        <!-- Botão de Login -->
                        <q-btn 
                            unelevated
                            rounded
                            label="Entrar" 
                            class="full-width"
                            size="lg"
                            style="background-color: #357870; color: white;"
                            @click="onLogin"
                            :loading="authStore.isLoading"
                            :disable="!email || !password"
                        />
                    </q-card-section>

                    <!-- Footer -->
                    <q-separator />
                    <q-card-section class="text-center" style="padding: 16px; background-color: #f5f5f5;">
                        <div class="text-caption text-grey-7">
                            by <strong>NoLife Dev Team</strong>
                        </div>
                    </q-card-section>
                </q-card>
            </q-page>
        </q-page-container>
    </q-layout>
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