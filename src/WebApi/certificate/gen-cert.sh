#!/bin/bash

# Função para gerar o arquivo PFX sem senha se a chave privada e o certificado existirem
generate_pfx() {
    local chave_privada=$1
    local certificado=$2
    local arquivo_pfx=$3
    local certs_dir=$4

    if [ -f "$chave_privada" ] && [ -f "$certificado" ]; then
        echo "Gerando arquivo PFX sem senha..."
        openssl pkcs12 -export -out "$certs_dir/$arquivo_pfx" -inkey "$chave_privada" -in "$certificado" -password pass:
        echo "Arquivo PFX gerado: $certs_dir/$arquivo_pfx"
    else
        echo "Erro: Chave privada ou certificado não encontrados."
        exit 1
    fi
}

# Função para gerar a chave privada RSA (sem senha)
generate_private_key() {
    local chave_privada=$1
    if [ ! -f "$chave_privada" ]; then
        echo "Gerando chave privada RSA..."
        openssl genpkey -algorithm RSA -out "$chave_privada"
        echo "Chave privada gerada: $chave_privada"
    else
        echo "Chave privada já existe: $chave_privada"
    fi
}

# Função para gerar o certificado autoassinado diretamente sem a necessidade de CSR usando um arquivo .cnf
generate_self_signed_cert() {
    local chave_privada=$1
    local certificado=$2
    local config_file=$3
    if [ ! -f "$certificado" ]; then
        echo "Gerando certificado autoassinado com arquivo de configuração $config_file..."
        openssl req -x509 -new -key "$chave_privada" -out "$certificado" -days 36500 -config "$config_file"
        echo "Certificado autoassinado gerado: $certificado"
    else
        echo "Certificado já existe: $certificado"
    fi
}

# Função para limpar arquivos temporários (chave e certificado)
cleanup() {
    local chave_privada=$1
    local certificado=$2
    echo "Removendo arquivos temporários..."
    rm -f "$chave_privada"
    rm -f "$certificado"
    echo "Arquivos temporários removidos."
}

# Verifica se o número de argumentos passados é correto
if [ $# -ne 1 ]; then
    echo "Uso: $0 <nome_do_arquivo>"
    exit 1
fi

# Nome do arquivo base para os certificados gerados
arquivo_base=$1
# Caminho do arquivo de configuração
oem_config_file="oem.cnf"

# Definindo os caminhos dos arquivos
chave_privada="${arquivo_base}_dev.key"
certificado="${arquivo_base}_dev_cert.pem"
arquivo_pfx="${arquivo_base}"

# Verifica se o arquivo de configuração existe
if [ ! -f "$oem_config_file" ]; then
    echo "Erro: Arquivo de configuração $oem_config_file não encontrado."
    exit 1
fi

# Verifica o sistema operacional e define o diretório de certificados corretamente
if [[ "$OSTYPE" == "linux-gnu"* || "$OSTYPE" == "darwin"* ]]; then
    # Para sistemas Linux/Mac, define o diretório de certificados com barra normal
    CERTS_DIR="./"
    
    # Verifica se o diretório de certificados existe
    if [ ! -d "$CERTS_DIR" ]; then
        echo "Diretório de certificados não encontrado, criando diretório: $CERTS_DIR"
        mkdir -p "$CERTS_DIR"
    fi

elif [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" || "$OSTYPE" == "win32" ]]; then
    # Para Windows, usa a variável de ambiente $APPDATA
    CERTS_DIR="./"

    # Verifica se o diretório de certificados existe no Windows
    if [ ! -d "$CERTS_DIR" ]; then
        echo "Diretório de certificados não encontrado, criando diretório: $CERTS_DIR"
        mkdir -p "$CERTS_DIR"
    fi
else
    echo "Sistema operacional não suportado."
    exit 1
fi

# Gera a chave privada
generate_private_key "$chave_privada"

# Gera o certificado autoassinado com o arquivo de configuração
generate_self_signed_cert "$chave_privada" "$certificado" "$oem_config_file"

# Gera o arquivo PFX
generate_pfx "$chave_privada" "$certificado" "$arquivo_pfx" "$CERTS_DIR"

# Limpa os arquivos temporários
cleanup "$chave_privada" "$certificado"

echo "Processo concluído."