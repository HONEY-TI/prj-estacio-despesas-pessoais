#!/bin/bash

set -euo pipefail

# --- Limpa a tela ---
clear

# --- Paths configurados (mantidos) ---
baseDirectory=$(pwd)
projectTestPath="$baseDirectory/XunitTests"
reportPath="$projectTestPath/TestResults"
coverageXmlPath="$reportPath/coveragereport"
sourceDirs="$baseDirectory/Despesas.Application:$baseDirectory/Despesas.Domain:$baseDirectory/Despesas.Repository:$baseDirectory/Despesas.Backend"
filefilters="$baseDirectory/Migrations.DataSeeders/**:-$baseDirectory/Migrations.MySqlServer/**:-$baseDirectory/Migrations.MsSqlServer/**:-$baseDirectory/Despesas.CrossCutting/**:-$baseDirectory/Despesas.Infrastructure/**"

# --- Função para matar processos dotnet sem janela ---
stop_dotnet_processes() {
    pids=$(pgrep -f "dotnet" || true)
    if [ -n "$pids" ]; then
        echo "Encerrando processos dotnet em segundo plano..."
        kill -9 $pids || true
    fi
}

# --- Limpa a pasta TestResults ---
echo "Limpando diretório de TestResults..."
rm -rf "$reportPath"
mkdir -p "$reportPath"

# --- Encerra processos dotnet em segundo plano ---
stop_dotnet_processes

# --- Build do projeto backend ---
echo "Build do backend..."
dotnet build "$baseDirectory/Despesas.Backend/Despesas.Backend.csproj" --restore --nologo

# --- Executa testes e coleta cobertura ---
echo "Executando testes e coletando cobertura..."
dotnet test "$projectTestPath/XUnit.Tests.csproj" \
    --configuration Test \
    --results-directory "$reportPath" \
    -p:CollectCoverage=true \
    -p:CoverletOutput="$reportPath/coverage.cobertura.xml" \
    -p:CoverletOutputFormat=cobertura \
    --collect:"XPlat Code Coverage;Format=opencover" \
    --no-build \
    --nologo

# --- Aguarda até o arquivo de cobertura existir (timeout 60s) ---
timeout=60
elapsed=0
sleepInterval=2
coverageFile="$projectTestPath/TestResults/coverage.cobertura.xml"

while [ ! -f "$coverageFile" ]; do
    sleep $sleepInterval
    elapsed=$((elapsed + sleepInterval))
    if [ $elapsed -ge $timeout ]; then
        echo "Timeout: Arquivo de cobertura não encontrado em $timeout segundos"
        exit 1
    fi
done

# --- Gera relatório HTML e lcov ---
mkdir -p "$coverageXmlPath"
echo "Gerando relatório de cobertura..."
reportgenerator \
    -reports:"$coverageFile" \
    -targetdir:"$coverageXmlPath" \
    -reporttypes:"Html;lcov;" \
    -sourcedirs:"$sourceDirs" \
    -filefilters:"$filefilters"

# --- Abre o relatório no navegador padrão ---
echo "Abrindo relatório..."
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    xdg-open "$coverageXmlPath/index.html"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    open "$coverageXmlPath/index.html"
elif [[ "$OSTYPE" == "msys"* || "$OSTYPE" == "win32" ]]; then
    cmd /c start "" "$coverageXmlPath/index.html"
fi

echo "Relatório de cobertura gerado em: $coverageXmlPath/index.html"
