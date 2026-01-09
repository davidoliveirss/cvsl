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