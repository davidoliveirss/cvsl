<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAuthStore } from '@/stores/auth';

const $q = useQuasar();
const router = useRouter();
const authStore = useAuthStore();

const in1 = ref(null);
const in2 = ref(null);
const accept = ref(false);
const ratingModel = ref(2);

const onSubmit = () => {
    console.log("accept")
};

const onReset = () => {
    console.log("reset")
};

const handleLogout = () => {
    authStore.logout();
    $q.notify({
        type: 'positive',
        message: 'Logout efetuado com sucesso!'
    });
    router.push('/login');
};
</script>

<template>

    <title>Centro Veterinario S.Lourenço</title>

    <div class="q-pa-md">
        <q-card class="my-card" flat bordered>
            <q-card-section class="bg-grey-10 text-center">
                <div class="text-h5 text-white">Gerir Conta</div>
                <div class="text-subtitle1 text-grey-4 q-mt-sm">{{ authStore.user?.nome || 'Utilizador' }}</div>
                <div class="text-caption text-grey-5">{{ authStore.user?.email }}</div>
            </q-card-section>
            <div>
                <q-form @submit="onSubmit" @reset="onReset" class="q-gutter-md q-px-md">
                    <q-input filled v-model="in1" color="grey-10" class="q-mt-xl" label="Avaliação *"
                        hint="O que achas sobre a aplicação" lazy-rules
                        :rules="[val => val && val.length > 0 || 'Tens que escrever algo']" />
                    <q-input filled v-model="in2" label="Melhorias" color="grey-10"
                        hint="O que achas que podia ser melhorado" />

                    <div class="q-pb-md q-pr-sm" style="text-align: right;">

                        <q-btn label="Enviar" type="submit" color="grey-9 q-mr-sm">
                            <q-tooltip anchor="center right" self="center left" transition-show="scale"
                                transition-hide="fade" class="bg-grey-9 text-body2">
                                <q-icon name="warning" class="q-mr-xs" />
                                Esta alteração é defenitiva!
                            </q-tooltip>
                        </q-btn>
                        <q-btn label="Redefinir" type="reset" color="white" flat class="text-grey-10" />

                    </div>
                </q-form>

                <q-separator class="q-my-md" />

                <div class="q-pa-md text-center">
                    <q-btn 
                        push 
                        color="negative" 
                        label="Terminar Sessão" 
                        icon="logout"
                        @click="handleLogout"
                    />
                </div>
            </div>
        </q-card>
    </div>

</template>
