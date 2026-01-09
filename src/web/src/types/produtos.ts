export interface Produto {
  id?: number;
  nome: string;
  idCategoria: number;          
  preco: number;                
  unidadesPorCaixa: number;     
  quantidadeStock: number;      
  idClinica: number;            
  ativo: boolean;
}
