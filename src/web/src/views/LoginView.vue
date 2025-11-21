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

async function onLogin() {
    if (!email.value || !password.value) {
        $q.notify({
            type: 'negative',
            message: 'Preencha email e password'
        });
        return;
    }

    const success = await authStore.login(email.value, password.value);

    if (success) {
        $q.notify({
            type: 'positive',
            message: 'Login efetuado com sucesso!'
        });
        router.push('/');
    } else {
        $q.notify({
            type: 'negative',
            message: authStore.error || 'Email ou password inválidos'
        });
    }
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
                    <q-input 
                        rounded 
                        outlined 
                        bg-color="grey-3" 
                        color="grey-10" 
                        v-model="email" 
                        label="Email"
                        type="email"
                        @keyup.enter="onLogin"
                    />
                    <q-input 
                        rounded 
                        outlined 
                        bg-color="grey-3" 
                        color="grey-10" 
                        class="q-mt-md" 
                        v-model="password" 
                        label="Password"
                        type="password"
                        @keyup.enter="onLogin"
                    />
                    <div class="q-mt-md q-mr-md" style="text-align: right;">
                        <q-btn 
                            push 
                            color="grey-9" 
                            label="Login" 
                            @click="onLogin"
                            :loading="authStore.isLoading"
                        />
                    </div>
                </q-tab-panel>

                <q-tab-panel name="register" animated>
                    <div class="text-center text-grey-7">
                        <p>Registo disponível apenas para administradores</p>
                    </div>
                </q-tab-panel>
            </q-tab-panels>
        </q-card>
    </div>
</template>

<style scoped></style>