import { authService } from './authService';

const API_URL = 'http://localhost:5000/api';

export interface Cliente {
  id?: number;
  nome: string;
  nif?: string;
  morada?: string;
  telefone: string;
  email?: string;
  clinicaId: number;
  ativo?: boolean;
}

export const clientesService = {
  // GET /api/clientes - Listar todos
  async getAll(incluirInativos: boolean = false): Promise<Cliente[]> {

      const userType = authService.getUserType();

      if (!userType) {
        throw new Error('Usuário não autenticado');
      }

      const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/clientes',
      'funcionario': '/Funcionarios/clientes'
      };

      const endpoint = `${endpointMap[userType] || '/Gerencia/clientes'}?incluirInativos=${incluirInativos}`;

      const response = await authService.fetchWithAuth(endpoint);

      if (!response.ok) {
        throw new Error('Erro ao carregar clientes');
      }
      
      return response.json();
    },

  // GET /api/clientes/:id - Buscar por ID
  async getById(id: number): Promise<Cliente> {
    const response = await authService.fetchWithAuth(`/clientes/${id}`);
    
    if (!response.ok) {
      throw new Error('Cliente não encontrado');
    }
    
    return response.json();
  },

  async create(cliente: Cliente): Promise<Cliente> {
    const userType = authService.getUserType();
  
    if (!userType) {
      throw new Error('Usuário não autenticado');
    }

    const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/clientes',
      'funcionario': '/Funcionarios/clientes',
    };

    const endpoint = endpointMap[userType] || '/clientes';
  
    const response = await authService.fetchWithAuth(endpoint, {
      method: 'POST',
      body: JSON.stringify(cliente)
    });
  
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar cliente');
    }
  
    return response.json();
  },

  // PUT /api/clientes/:id - Atualizar
  async update(id: number, cliente: Cliente): Promise<Cliente> {
    const response = await authService.fetchWithAuth(`/clientes/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...cliente, id })
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar cliente');
    }
    
    return response.json();
  },

  // DELETE /api/clientes/:id - Remover
  async delete(id: number): Promise<void> {
    const response = await authService.fetchWithAuth(`/clientes/${id}`, {
      method: 'DELETE'
    });
    
    if (!response.ok) {
      throw new Error('Erro ao remover cliente');
    }
  }
};
