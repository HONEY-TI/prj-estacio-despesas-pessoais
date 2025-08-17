#!/bin/bash

arquivo_base="ssl_certificate"
chave_privada="${arquivo_base}.key"
certificado="${arquivo_base}.pem"

if [ ! -f "$chave_privada" ]; then
    echo "Gerando chave privada RSA..."
    openssl genpkey -algorithm RSA -out "$chave_privada" -config openssl.cnf
    echo "Chave privada gerada: $chave_privada"
fi

if [ ! -f "$certificado" ]; then
    echo "Gerando certificado autoassinado..."
    openssl req -x509 -new -key "$chave_privada" -out "$certificado" -days 365 -config openssl.cnf
    echo "Certificado autoassinado gerado: $certificado"
fi
