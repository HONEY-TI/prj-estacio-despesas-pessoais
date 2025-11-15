#!/bin/bash

# ----------------------
# Script para backup ou restore de volumes Docker usando Alpine
# ----------------------

show_help() {
cat << EOF
USO:
  $0 -v <nome_do_volume> -a <backup|restore> [-h]

PARÂMETROS:
  -v    Nome do volume Docker que deseja fazer backup ou restaurar (obrigatório)
  -a    Ação: 'backup' (padrão: backup) ou 'restore'
  -h    Mostra esta ajuda

EXEMPLOS:
  # Fazer backup do volume 'oracle-xe_oradata'
  $0 -v oracle-xe_oradata -a backup

  # Restaurar o volume 'oracle-xe_oradata'
  $0 -v oracle-xe_oradata -a restore
EOF
}

# ----------------------
# Diretório raiz do projeto (onde o script está)
# ----------------------
ProjectRoot="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# ----------------------
# Valores padrão
# ----------------------
Action="backup"
VolName=""

# ----------------------
# Parse de parâmetros
# ----------------------
while getopts "v:a:h" opt; do
    case $opt in
        v) VolName="$OPTARG" ;;
        a) Action="$OPTARG" ;;
        h) show_help; exit 0 ;;
        *) show_help; exit 1 ;;
    esac
done

# ----------------------
# Mostrar help se nenhum parâmetro foi passado ou VolName não informado
# ----------------------
if [[ -z "$VolName" ]]; then
    show_help
    exit 0
fi

# ----------------------
# Timestamp atual para o arquivo de backup
# ----------------------
TimeStamp=$(date +"%Y%m%d%H%M")

# ----------------------
# Caminho do arquivo de backup
# ----------------------
BackupFile="${ProjectRoot}/${VolName}-backup-${TimeStamp}.tgz"

# ----------------------
# Certificar que a imagem alpine existe
# ----------------------
if ! docker image inspect alpine >/dev/null 2>&1; then
    echo "🔹 Imagem 'alpine' não encontrada. Baixando..."
    docker pull alpine
    echo "✅ Imagem 'alpine' baixada."
fi

# ----------------------
# Backup
# ----------------------
if [[ "$Action" == "backup" ]]; then
    echo "🔹 Fazendo backup do volume $VolName para $BackupFile ..."
    docker run --rm \
        -v "${VolName}:/dados" \
        -v "${ProjectRoot}:/backup" \
        alpine sh -c "tar czf /backup/$(basename $BackupFile) /dados"
    echo "✅ Backup concluído."
fi

# ----------------------
# Restore
# ----------------------
if [[ "$Action" == "restore" ]]; then
    if [[ ! -f "$BackupFile" ]]; then
        echo "❌ Arquivo de backup não encontrado: $BackupFile"
        exit 1
    fi
    echo "🔹 Restaurando $BackupFile para volume $VolName ..."
    docker run --rm \
        -v "${VolName}:/dados" \
        -v "${ProjectRoot}:/backup" \
        alpine sh -c "tar xzf /backup/$(basename $BackupFile) -C /"
    echo "✅ Restore concluído."
fi
