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

export interface FuncionarioPerfil {
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

export interface UpdatePerfilData {
  telefone: string;
  password?: string;
}
