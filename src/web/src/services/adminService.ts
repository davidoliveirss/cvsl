import { authService } from './authService';

const API_URL = 'http://localhost:5000/api/admin';

interface Clinica {
  id: number;
  nome: string;
  email: string;
  cp: string;
  nif?: string;
  iban: string;
  numeroFuncionarios?: number;
}

interface ClinicaCreate {
  nome: string;
  email: string;
  password: string;
  cp: string;
  nif?: string;
  iban: string;
}

interface ClinicaUpdate {
  nome: string;
  email: string;
  password?: string;
  cp: string;
  nif?: string;
  iban: string;
}

interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

interface GlobalStats {
  totalClinicas: number;
  totalFuncionarios: number;
  totalClientes: number;
}

export const adminService = {
  // ========== GESTÃO DE CLÍNICAS ==========

  async getAllClinicas(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<Clinica>> {
    const response = await authService.fetchWithAuth(
      `/admin/clinicas?page=${page}&pageSize=${pageSize}`
    );

    if (!response.ok) {
      throw new Error('Erro ao buscar clínicas');
    }

    return response.json();
  },

  async getClinica(id: number): Promise<Clinica> {
    const response = await authService.fetchWithAuth(`/admin/clinicas/${id}`);

    if (!response.ok) {
      throw new Error('Erro ao buscar clínica');
    }

    return response.json();
  },

  async createClinica(data: ClinicaCreate): Promise<Clinica> {
    const response = await authService.fetchWithAuth('/admin/clinicas', {
      method: 'POST',
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar clínica');
    }

    return response.json();
  },

  async updateClinica(id: number, data: ClinicaUpdate): Promise<Clinica> {
    const response = await authService.fetchWithAuth(`/admin/clinicas/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar clínica');
    }

    return response.json();
  },

  async deleteClinica(id: number): Promise<void> {
    const response = await authService.fetchWithAuth(`/admin/clinicas/${id}`, {
      method: 'DELETE',
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao eliminar clínica');
    }
  },

  // ========== ESTATÍSTICAS ==========

  async getGlobalStats(): Promise<GlobalStats> {
    const response = await authService.fetchWithAuth('/admin/dashboard/stats');

    if (!response.ok) {
      throw new Error('Erro ao buscar estatísticas');
    }

    return response.json();
  },

  // ========== GESTÃO DE ADMINS ==========

  async getAllAdmins() {
    const response = await authService.fetchWithAuth('/admin/admins');

    if (!response.ok) {
      throw new Error('Erro ao buscar administradores');
    }

    return response.json();
  },

  async createAdmin(data: { nome: string; email: string; password: string; nivel?: string }) {
    const response = await authService.fetchWithAuth('/admin/admins', {
      method: 'POST',
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar administrador');
    }

    return response.json();
  },
};

export type { Clinica, ClinicaCreate, ClinicaUpdate, PaginatedResponse, GlobalStats };
