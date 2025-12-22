<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAuthStore } from '@/stores/auth';
import { authService } from '@/services/authService';
import { funcionariosService } from '@/services/funcionariosService';

const $q = useQuasar();
const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const profileData = ref<any>(null);

// Dados do formulário
const telefone = ref('');
const password = ref('');
const confirmPassword = ref('');

// Dados da clínica (apenas para clínicas)
const cp = ref('');
const nif = ref('');
const iban = ref('');

const loadProfile = async () => {
    loading.value = true;
    try {
        if (authStore.isClinica) {
            const response = await authService.fetchWithAuth('/clinicas/perfil');
            const data = await response.json();
            profileData.value = data;
            
            // Preencher campos
            cp.value = data.cp || '';
            nif.value = data.nif || '';
            iban.value = data.iban || '';
        } else if (authStore.isFuncionario) {
            const data = await funcionariosService.getPerfil();
            profileData.value = data;
            
            // Preencher campos
            telefone.value = data.telefone || '';
        }
    } catch (error: any) {
        $q.notify({
            type: 'negative',
            message: error.message || 'Erro ao carregar perfil'
        });
    } finally {
        loading.value = false;
    }
};

const onSubmit = async () => {
    if (password.value && password.value !== confirmPassword.value) {
        $q.notify({
            type: 'warning',
            message: 'As passwords não coincidem'
        });
        return;
    }

    loading.value = true;
    try {
        if (authStore.isClinica) {
            const response = await authService.fetchWithAuth('/clinicas/perfil', {
                method: 'PUT',
                body: JSON.stringify({
                    cp: cp.value,
                    nif: nif.value,
                    iban: iban.value
                })
            });
            
            if (!response.ok) {
                throw new Error('Erro ao atualizar perfil');
            }
        } else if (authStore.isFuncionario) {
            await funcionariosService.updatePerfil({
                telefone: telefone.value,
                password: password.value || undefined
            });
        }

        $q.notify({
            type: 'positive',
            message: 'Perfil atualizado com sucesso!'
        });
        
        // Limpar passwords
        password.value = '';
        confirmPassword.value = '';
        
        // Recarregar perfil
        await loadProfile();
    } catch (error: any) {
        $q.notify({
            type: 'negative',
            message: error.message || 'Erro ao atualizar perfil'
        });
    } finally {
        loading.value = false;
    }
};

const onReset = () => {
    loadProfile();
    password.value = '';
    confirmPassword.value = '';
};

const handleLogout = () => {
    authStore.logout();
    $q.notify({
        type: 'positive',
        message: 'Logout efetuado com sucesso!'
    });
    router.push('/login');
};

onMounted(() => {
    loadProfile();
});
</script>

<template>
    <title>Centro Veterinario S.Lourenço</title>

    <div class="q-pa-md">
        <q-card class="my-card" flat bordered>
            <q-card-section class="bg-grey-10 text-center">
                <div class="text-h5 text-white">Gerir Conta</div>
                <div class="text-subtitle1 text-grey-4 q-mt-sm">{{ authStore.user?.nome || 'Utilizador' }}</div>
                <div class="text-caption text-grey-5">{{ authStore.user?.email }}</div>
                <q-badge v-if="authStore.isClinica" color="primary" class="q-mt-sm">🏥 Clínica</q-badge>
                <q-badge v-else-if="authStore.isFuncionario" color="secondary" class="q-mt-sm">👨‍⚕️ Funcionário</q-badge>
            </q-card-section>

            <q-inner-loading :showing="loading">
                <q-spinner-gears size="50px" color="primary" />
            </q-inner-loading>

            <div v-if="!loading && profileData">
                <q-form @submit="onSubmit" @reset="onReset" class="q-gutter-md q-px-md">
                    
                    <!-- Informações apenas visíveis -->
                    <div class="q-mt-lg">
                        <div class="text-h6 text-grey-8 q-mb-md">Informações da Conta</div>
                        
                        <q-input 
                            filled 
                            readonly
                            v-model="profileData.nome" 
                            label="Nome" 
                            color="grey-10"
                        />
                        
                        <q-input 
                            filled 
                            readonly
                            v-model="profileData.email" 
                            label="Email" 
                            color="grey-10"
                            class="q-mt-md"
                        />

                        <!-- Campos específicos do funcionário -->
                        <template v-if="authStore.isFuncionario">
                            <q-input 
                                filled 
                                readonly
                                v-model="profileData.especialidade" 
                                label="Especialidade" 
                                color="grey-10"
                                class="q-mt-md"
                            />
                            
                            <q-input 
                                filled 
                                readonly
                                v-model="profileData.clinica.nome" 
                                label="Clínica" 
                                color="grey-10"
                                class="q-mt-md"
                            />
                        </template>
                    </div>

                    <q-separator class="q-my-lg" />

                    <!-- Campos editáveis -->
                    <div>
                        <div class="text-h6 text-grey-8 q-mb-md">Editar Informações</div>
                        
                        <!-- Campos para clínica -->
                        <template v-if="authStore.isClinica">
                            <q-input 
                                filled 
                                v-model="cp" 
                                label="Código Postal" 
                                color="grey-10"
                                hint="Ex: 1000-001"
                            />
                            
                            <q-input 
                                filled 
                                v-model="nif" 
                                label="NIF" 
                                color="grey-10"
                                class="q-mt-md"
                            />
                            
                            <q-input 
                                filled 
                                v-model="iban" 
                                label="IBAN" 
                                color="grey-10"
                                class="q-mt-md"
                            />
                        </template>

                        <!-- Campos para funcionário -->
                        <template v-if="authStore.isFuncionario">
                            <q-input 
                                filled 
                                v-model="telefone" 
                                label="Telefone" 
                                color="grey-10"
                                hint="Número de contacto"
                                :rules="[val => val && val.length > 0 || 'Campo obrigatório']"
                            />
                            
                            <q-input 
                                filled 
                                v-model="password" 
                                label="Nova Password (opcional)" 
                                type="password"
                                color="grey-10"
                                class="q-mt-md"
                                hint="Deixe em branco para não alterar"
                            />
                            
                            <q-input 
                                v-if="password"
                                filled 
                                v-model="confirmPassword" 
                                label="Confirmar Nova Password" 
                                type="password"
                                color="grey-10"
                                class="q-mt-md"
                                :rules="[val => val === password || 'As passwords não coincidem']"
                            />
                        </template>
                    </div>

                    <div class="q-pb-md q-pr-sm" style="text-align: right;">
                        <q-btn label="Guardar" type="submit" color="grey-9" class="q-mr-sm" :loading="loading" />
                        <q-btn label="Cancelar" type="reset" color="white" flat class="text-grey-10" />
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
