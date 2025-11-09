#!/bin/bash

# --- Diretórios base ---
baseDirectory=$(realpath ..)
projectTestPath="$baseDirectory/XunitTests"
if [ ! -d "$projectTestPath" ]; then
    projectTestPath="$baseDirectory/XunitTests"
fi
projectAngular="$baseDirectory/AngularApp"
sourceDirs="$baseDirectory/Despesas.Application:$baseDirectory/Despesas.Domain:$baseDirectory/Despesas.Repository:$baseDirectory/Despesas.Backend:$baseDirectory/AngularApp"
filefilters="$baseDirectory/Despesas.DataSeeders/**;-$baseDirectory/Migrations.MySqlServer/**;-$baseDirectory/Migrations.MsSqlServer/**;-$baseDirectory/Despesas.CrossCutting/**;-$baseDirectory/Despesas.Application/HyperMedia/**"
reportPath="$projectTestPath/TestResults"
coveragePath="$reportPath/coveragereport"
coverageAngularPath="$projectAngular/coverage"

# --- Função para aguardar a pasta TestResults ---
wait_testresults() {
    local attempt=0
    while [ ! -d "$reportPath" ]; do
        echo "Aguardando TestResults..."
        sleep 10
        attempt=$((attempt + 1))
        if [ "$attempt" -ge 6 ]; then
            break
        fi
    done
}

# --- Executa testes backend e gera cobertura ---
dotnet test "$projectTestPath/XUnit.Tests.csproj" \
    --results-directory "$reportPath" \
    -p:CollectCoverage=true \
    -p:CoverletOutputFormat=cobertura \
    --collect:"XPlat Code Coverage;Format=opencover"

wait_testresults

# --- Encontra o arquivo coverage.cobertura.xml real ---
coverageXmlFile=$(find "$reportPath" -type f -name "coverage.cobertura.xml" | head -n 1)

if [ ! -f "$coverageXmlFile" ]; then
    echo "Arquivo de cobertura não encontrado!"
    exit 1
fi

# --- Gera relatório HTML/LCOV ---
reportgenerator \
    -reports:"$coverageXmlFile" \
    -targetdir:"$coveragePath" \
    -reporttypes:"Html;lcov;" \
    -sourcedirs:"$sourceDirs" \
    -filefilters:-"$filefilters"

# --- Copia os resultados do teste mais recente ---
latestDir=$(ls -td "$reportPath"/*/ | grep -v 'coveragereport' | head -n 1)
if [ -n "$latestDir" ]; then
    cp -r "$latestDir/"* "$reportPath/"
else
    echo "Nenhum diretório de resultados encontrado."
fi

# --- Verifica node_modules e instala se necessário ---
if [ ! -d "$projectAngular/node_modules" ]; then
    (cd "$projectAngular" && npm install)
fi

# --- Executa testes Angular com cobertura ---
(cd "$projectAngular" && npm run test:coverage)
