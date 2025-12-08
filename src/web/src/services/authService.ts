const API_URL = 'http://localhost:5000/api';

interface LoginCredentials {
  email: string;
  password: string;
}

interface AuthResponse {
  token: string;
  email: string;
  nome: string;
}

type UserType = 'clinica' | 'funcionario';

export const authService = {
  async login(email: string, password: string, userType: UserType = 'clinica'): Promise<AuthResponse> {
    // Escolhe endpoint baseado no tipo de utilizador
    const endpoint = userType === 'clinica' 
      ? `${API_URL}/auth/login/clinica`
      : `${API_URL}/auth/login/funcionario`;

    const response = await fetch(endpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Email ou password inválidos');
    }

    const data: AuthResponse = await response.json();
    
    // Guardar token e tipo de utilizador no localStorage
    if (data.token) {
      localStorage.setItem('token', data.token);
      localStorage.setItem('userType', userType);
      localStorage.setItem('user', JSON.stringify({ 
        email: data.email, 
        nome: data.nome,
        type: userType 
      }));
    }

    return data;
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('userType');
  },

  getToken(): string | null {
    return localStorage.getItem('token');
  },

  getUserType(): UserType | null {
    return localStorage.getItem('userType') as UserType | null;
  },

  getUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Verificar se token expirou
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000; // converter para milliseconds
      return Date.now() < exp;
    } catch {
      return false;
    }
  },

  isClinica(): boolean {
    return this.getUserType() === 'clinica';
  },

  isFuncionario(): boolean {
    return this.getUserType() === 'funcionario';
  },

  // Função helper para fazer requests autenticadas
  async fetchWithAuth(url: string, options: RequestInit = {}) {
    const token = this.getToken();
    
    const headers = {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
      ...options.headers,
    };

    const response = await fetch(`${API_URL}${url}`, {
      ...options,
      headers,
    });

    // Se retornar 401, fazer logout automático
    if (response.status === 401) {
      this.logout();
      window.location.href = '/login';
      throw new Error('Sessão expirada');
    }

    return response;
  }
};
