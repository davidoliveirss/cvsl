import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import { authService } from '@/services/authService';

interface User {
  email: string;
  nome: string;
  type?: 'clinica' | 'funcionario' | 'admin';
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(authService.getToken());
  const user = ref<User | null>(authService.getUser());
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const isAuthenticated = computed(() => !!token.value);
  const isClinica = computed(() => user.value?.type === 'clinica');
  const isFuncionario = computed(() => user.value?.type === 'funcionario');
  const isAdmin = computed(() => user.value?.type === 'admin');

  async function login(email: string, password: string, userType: 'clinica' | 'funcionario' | 'admin' = 'clinica') {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await authService.login(email, password, userType);
      token.value = response.token;
      user.value = {
        email: response.email,
        nome: response.nome,
        type: userType
      };
      return true;
    } catch (err: any) {
      error.value = err.message || 'Erro ao fazer login';
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  function logout() {
    authService.logout();
    token.value = null;
    user.value = null;
  }

  return {
    token,
    user,
    isLoading,
    error,
    isAuthenticated,
    isClinica,
    isFuncionario,
    isAdmin,
    login,
    logout
  };
});
