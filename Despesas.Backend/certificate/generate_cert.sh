#!/bin/bash

arquivo_base="webapi-cert"
chave_privada="${arquivo_base}.key"
certificado="${arquivo_base}.pem"
certificado_crt="${arquivo_base}.crt"
arquivo_pfx="${arquivo_base}.pfx"

# Gera a chave privada se não existir
if [ ! -f "$chave_privada" ]; then
  echo "Gerando chave privada RSA..."
  openssl genpkey -algorithm RSA -out "$chave_privada" -aes256 -config openssl.cnf
  echo "Chave privada gerada: $chave_privada"
fi

# Gera o certificado autoassinado se não existir
if [ ! -f "$certificado" ]; then
  echo "Gerando certificado autoassinado..."
  openssl req -x509 -new -key "$chave_privada" -out "$certificado" -days 730 -config openssl.cnf
  echo "Certificado autoassinado gerado: $certificado"
fi

# Gera o arquivo .crt se não existir
if [ ! -f "$certificado_crt" ]; then
  echo "Gerando arquivo CRT..."
  cp "$certificado" "$certificado_crt"
  echo "Arquivo CRT gerado: $certificado_crt"
fi

# Gera o arquivo PFX se a chave privada e o certificado existirem
if [ -f "$chave_privada" ] && [ -f "$certificado" ]; then
  echo "Gerando arquivo PFX..."
  openssl pkcs12 -export -out "$arquivo_pfx" -inkey "$chave_privada" -in "$certificado"
  echo "Arquivo PFX gerado: $arquivo_pfx"
else
  echo "Erro: Chave privada ou certificado não encontrados."
  exit 1
fi
