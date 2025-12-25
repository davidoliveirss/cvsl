import { authService } from './authService';

const API_URL = 'http://localhost:5000/api';

export interface Animal {
  id?: number;
  transponder?: string;
  nome: string;
  especie?: string;
  raca?: string;
  data_nascimento?: string;
  sexo?: string;
  id_cliente: number | null;
  id_clinica: number;
  ativo?: boolean;
}

export const animaisService = {
  // GET /api/clientes - Listar todos
  async getAll(incluirInativos: boolean = false): Promise<Animal[]> {

      const userType = authService.getUserType();

      if (!userType) {
        throw new Error('Usuário não autenticado');
      }

      const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/animais',
      'funcionario': '/Funcionarios/animais'
      };

      const endpoint = `${endpointMap[userType]}?incluirInativos=${incluirInativos}`;

      const response = await authService.fetchWithAuth(endpoint);

      if (!response.ok) {
        throw new Error('Erro ao carregar animais');
      }
      
      return response.json();
    },

  // GET /api/clientes/:id - Buscar por ID
  async getById(id: number): Promise<Animal> {
    const response = await authService.fetchWithAuth(`/clientes/${id}`);
    
    if (!response.ok) {
      throw new Error('Cliente não encontrado');
    }
    
    return response.json();
  },

  async create(animal: Animal): Promise<Animal> {
    const userType = authService.getUserType();
  
    if (!userType) {
      throw new Error('Usuário não autenticado');
    }

    const endpointMap: Record<string, string> = {
      'clinica': '/Clinicas/animais',
      'funcionario': '/Funcionarios/animais',
    };

    const endpoint = endpointMap[userType] || '/animais';
  
    const response = await authService.fetchWithAuth(endpoint, {
      method: 'POST',
      body: JSON.stringify(animal)
    });
  
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao registar o animal');
    }
  
    return response.json();
  },

  // PUT /api/clientes/:id - Atualizar
  async update(id: number, cliente: Animal): Promise<Animal> {
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
