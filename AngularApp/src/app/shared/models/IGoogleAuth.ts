
export interface IGoogleAuth {
  id: number;
  authenticated: boolean,
  created: string,
  expiration: string
  accessToken: string,
  refreshToken: string,
  refreshTokenExpiry: string
  message: string;
  externalId: string;
  externalProvider: string;
  email: string;
  nome: string;
  sobreNome: string;
  telefone: string;
}
