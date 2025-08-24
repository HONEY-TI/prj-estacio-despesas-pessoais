#!/bin/bash

arquivo_base="ssl_certificate"
chave_privada="${arquivo_base}.key"
certificado="${arquivo_base}.pem"
certificado_crt="${arquivo_base}.crt"

# Gera a chave privada se não existir
if [ ! -f "$chave_privada" ]; then
    echo "Gerando chave privada RSA..."
    openssl genpkey -algorithm RSA -out "$chave_privada" -config openssl.cnf
    echo "Chave privada gerada: $chave_privada"
fi

# Gera o certificado autoassinado se não existir
if [ ! -f "$certificado" ]; then
    echo "Gerando certificado autoassinado..."
    openssl req -x509 -new -key "$chave_privada" -out "$certificado" -days 365 -config openssl.cnf
    echo "Certificado autoassinado gerado: $certificado"
fi

# Gera o arquivo CRT se não existir
if [ ! -f "$certificado_crt" ]; then
    echo "Gerando arquivo CRT..."
    cp "$certificado" "$certificado_crt"
    echo "Arquivo CRT gerado: $certificado_crt"
fi
