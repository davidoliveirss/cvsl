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