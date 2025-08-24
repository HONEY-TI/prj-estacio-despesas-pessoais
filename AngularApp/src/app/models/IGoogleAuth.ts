
export interface IGoogleAuth {
  authenticated: boolean,
  created: string,
  expiration: string
  accessToken: string,
  refreshToken: string,
  externalId: string;
  externalProvider: string;
  email: string;
  nome: string;
  sobreNome: string;
  telefone: string;
}
