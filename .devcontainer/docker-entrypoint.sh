#!/bin/bash
set -e

echo "Inicializando ambiente..."
ng config -g cli.completion false
# Corrige permissões dos volumes persistentes
sudo chown -R developer:developer /home/developer/.nuget || true
sudo chown -R developer:developer /home/developer/.dotnet || true
sudo chown -R developer:developer /home/developer/.aspnet || true

chmod -R u+rwX /home/developer/.nuget || true
chmod -R u+rwX /home/developer/.dotnet || true

# Gera certificado se não existir
if [ ! -f /workspace/src/WebApi/certificate/webapi-cert.pfx ]; then
    echo "Criando certificado HTTPS..."

    dotnet dev-certs https \
        -ep /workspace/src/WebApi/certificate/webapi-cert.pfx \
        -p "12345!"
fi


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

exec "$@"