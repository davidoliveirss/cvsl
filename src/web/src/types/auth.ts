export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  nome: string;
}