import { authService } from './authService';

interface FuncionarioPerfil {
  id: number;
  nome: string;
  email: string;
  especialidade?: string;
  telefone: string;
  salario: number;
  ativo: boolean;
  clinica: {
    id: number;
    nome: string;
  };
}

interface UpdatePerfilData {
  telefone: string;
  password?: string;
}

export const funcionariosService = {
  async getPerfil(): Promise<FuncionarioPerfil> {
    const response = await authService.fetchWithAuth('/funcionarios/perfil');
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao obter perfil');
    }
    
    return response.json();
  },

  async updatePerfil(data: UpdatePerfilData): Promise<void> {
    const response = await authService.fetchWithAuth('/funcionarios/perfil', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar perfil');
    }
    
    return response.json();
  },
};
