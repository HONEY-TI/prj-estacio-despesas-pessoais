export interface DespesaDataSet {
  id: number | null;
  data: string;
  descricao: string;
  valor: string;
  dataVencimento: string | null;
  categoria: string;
}
