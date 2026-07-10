#!/bin/bash
set -e

echo "Inicializando ambiente..."

# Gera certificado se não existir
if [ ! -f /workspace/src/WebApi/certificate/webapi-cert.pfx ]; then
    echo "Criando certificado HTTPS..."

    dotnet dev-certs https \
        -ep /workspace/src/WebApi/certificate/webapi-cert.pfx \
        -p "12345!"
fi


# Converte certificado para Angular
mkdir -p /home/developer/.aspnet/https

if [ ! -f /home/developer/.aspnet/https/WebApi.pem ]; then

    openssl pkcs12 \
        -in /workspace/src/WebApi/certificate/webapi-cert.pfx \
        -clcerts \
        -nokeys \
        -out /home/developer/.aspnet/https/WebApi.pem \
        -passin pass:12345!


    openssl pkcs12 \
        -in /workspace/src/WebApi/certificate/webapi-cert.pfx \
        -nocerts \
        -nodes \
        -out /home/developer/.aspnet/https/WebApi.key \
        -passin pass:12345!

fi


echo "Ambiente pronto."


# IMPORTANTE:
# mantém o container vivo para o VS Code
exec "$@"