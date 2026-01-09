import { authService } from './authService';
import type { Produto } 
  from '@/types/produtos';

const API_URL = 'http://localhost:5000/api';


export const produtosService = {
  // GET /api/clientes - Listar todos
  async getAll(incluirInativos: boolean = false): Promise<Produto[]> {

      const userType = authService.getUserType();

      if (!userType) {
        throw new Error('Usuário não autenticado');
      }

      const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/produtos',
      'funcionario': '/Funcionarios/produtos'
      };

      const endpoint = `${endpointMap[userType]}?incluirInativos=${incluirInativos}`;

      const response = await authService.fetchWithAuth(endpoint);

      if (!response.ok) {
        throw new Error('Erro ao carregar produtos');
      }
      
      return response.json();
    },

  // GET /api/clientes/:id - Buscar por ID
  async getById(id: number): Promise<Produto> {
    const response = await authService.fetchWithAuth(`/clientes/${id}`);
    
    if (!response.ok) {
      throw new Error('Produto não encontrado');
    }
    
    return response.json();
  },

  async create(produto: Produto): Promise<Produto> {
    const userType = authService.getUserType();
  
    if (!userType) {
      throw new Error('Usuário não autenticado');
    }

    const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/produtos',
      'funcionario': '/Funcionarios/produtos',
    };

    const endpoint = endpointMap[userType] || '/produto';
  
    const response = await authService.fetchWithAuth(endpoint, {
      method: 'POST',
      body: JSON.stringify(produto)
    });
  
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao registar o produto');
    }
  
    return response.json();
  },

  // PUT /api/clientes/:id - Atualizar
  async update(id: number, cliente: Produto): Promise<Produto> {
    const userType = authService.getUserType();
  
    if (!userType) {
      throw new Error('Usuário não autenticado');
    }

    const endpointMap: Record<string, string> = {
      'clinica': `/Clinicas/clientes/${id}`, 
      'funcionario': `/Funcionarios/clientes/${id}`,
    };

    const endpoint = endpointMap[userType] || `/clientes/${id}`;
    

    const response = await authService.fetchWithAuth(endpoint, {
      method: 'PUT',
      body: JSON.stringify({ ...cliente, id })
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar cliente');
    }
    
    return response.json();
  },

  async updateStatus(id: number, ativo: boolean): Promise<void> {
    const userType = authService.getUserType();
  
    if (!userType) {
      throw new Error('Usuário não autenticado');
    }

    const endpointMap: Record<string, string> = {
      'clinica': `/Clinicas/clientes/${id}`, 
      'funcionario': `/Funcionarios/clientes/${id}`,
    };

    const endpoint = endpointMap[userType] || `/clientes/${id}`;

    const response = await authService.fetchWithAuth(endpoint, {
      method: 'PATCH',
      body: JSON.stringify({ ativo })
    });
    
    if (!response.ok) {
      throw new Error('Erro ao alterar estado do cliente');
    }
  },
};
