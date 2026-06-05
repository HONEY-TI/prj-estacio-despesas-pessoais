const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:42535';

const PROXY_CONFIG = [{
  target: target,
  "context": [
    "/acesso",
    "/categoria",
    "/despesa",
    "/receita",
    "/lancamento",
    "/graficos",
    "/saldo",
    "/usuario",
    "/dashboard"
  ],
  secure: false,
  changeOrigin: true,
  headers: {
    Connection: 'Keep-Alive',
    "Cross-Origin-Opener-Policy": "same-origin",
    "Cross-Origin-Embedder-Policy": "require-corp"
  }
}]

module.exports = PROXY_CONFIG;
