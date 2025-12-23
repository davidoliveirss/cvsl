import { authService } from './authService';

export interface Funcionario {
  id?: number;
  nome: string;
  email: string;
  password?: string;
  especialidade?: string;
  telefone: string;
  salario: number;
  ativo?: boolean;
  clinicaId: number;
}

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
  // ========== GESTÃO DE FUNCIONÁRIOS ==========
  
  // GET /api/Clinicas/funcionarios - Listar todos
  async getAll(incluirInativos: boolean = false): Promise<Funcionario[]> {
    const response = await authService.fetchWithAuth(`/Clinicas/funcionarios?incluirInativos=${incluirInativos}`);
    
    if (!response.ok) {
      throw new Error('Erro ao carregar funcionários');
    }
    
    return response.json();
  },

  // GET /api/Clinicas/funcionarios/:id - Buscar por ID
  async getById(id: number): Promise<Funcionario> {
    const response = await authService.fetchWithAuth(`/Clinicas/funcionarios/${id}`);
    
    if (!response.ok) {
      throw new Error('Funcionário não encontrado');
    }
    
    return response.json();
  },

  // POST /api/Clinicas/funcionarios - Criar novo
  async create(funcionario: Funcionario): Promise<Funcionario> {
    const response = await authService.fetchWithAuth('/Clinicas/funcionarios', {
      method: 'POST',
      body: JSON.stringify(funcionario)
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar funcionário');
    }
    
    return response.json();
  },

  // PUT /api/Clinicas/funcionarios/:id - Atualizar
  async update(id: number, funcionario: Funcionario): Promise<Funcionario> {
    const response = await authService.fetchWithAuth(`/Clinicas/funcionarios/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...funcionario, id })
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar funcionário');
    }
    
    return response.json();
  },

  // DELETE /api/Clinicas/funcionarios/:id - Desativar (soft delete)
  async delete(id: number): Promise<void> {
    const response = await authService.fetchWithAuth(`/Clinicas/funcionarios/${id}`, {
      method: 'DELETE'
    });
    
    if (!response.ok) {
      throw new Error('Erro ao desativar funcionário');
    }
  },

  // PATCH /api/Clinicas/funcionarios/:id/ativo - Alterar estado ativo
  async updateAtivo(id: number, ativo: boolean): Promise<void> {
    const response = await authService.fetchWithAuth(`/Clinicas/funcionarios/${id}/ativo`, {
      method: 'PATCH',
      body: JSON.stringify({ ativo })
    });
    
    if (!response.ok) {
      throw new Error('Erro ao alterar estado do funcionário');
    }
  },

  // Métodos de perfil
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

export type { FuncionarioPerfil, UpdatePerfilData };
