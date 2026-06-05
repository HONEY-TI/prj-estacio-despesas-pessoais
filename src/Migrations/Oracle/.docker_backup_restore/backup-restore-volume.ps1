param(
    [Parameter(Mandatory=$false)]
    [switch]$Help,

    [Parameter(Mandatory=$false)]
    [string]$VolName,

    [ValidateSet("backup","restore")]
    [string]$Action = "backup"
)

# 🔹 Mostrar ajuda se pedido explicitamente OU se nenhum parâmetro foi passado OU se não informou o VolName
if ($Help -or $PSBoundParameters.Count -eq 0 -or -not $VolName) {
@"
Script para backup ou restore de volumes Docker usando Alpine.

USO:
  .\Backup-Restore-Volume.ps1 -VolName <nome_do_volume> -Action <backup|restore> [-Help]

PARÂMETROS:
  -VolName     Nome do volume Docker que deseja fazer backup ou restaurar. (Obrigatório)
  -Action      'backup' (padrão) cria arquivo .tgz do volume.
               'restore' restaura o volume a partir do arquivo .tgz existente.
  -Help        Mostra esta ajuda.

EXEMPLOS:
  # Fazer backup do volume 'oracle-xe_oradata'
  .\Backup-Restore-Volume.ps1 -VolName "oracle-xe_oradata" -Action backup

  # Restaurar o volume 'oracle-xe_oradata' a partir do arquivo .tgz na pasta do projeto
  .\Backup-Restore-Volume.ps1 -VolName "oracle-xe_oradata" -Action restore

O script verifica se a imagem 'alpine' existe e baixa automaticamente se necessário.
"@ | Write-Host
    exit 0
}

# 🔹 Diretório raiz do projeto (onde o script está)
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# 🔹 Timestamp atual para o arquivo de backup
$TimeStamp = Get-Date -Format "yyyyMMddHHmm"

# 🔹 Caminho do arquivo de backup
$BackupFile = Join-Path $ProjectRoot "$VolName-backup-$TimeStamp.tgz"

# 🔹 Certificar que a imagem alpine existe
$null = docker image inspect alpine 2>$null
if (-not $?) {
    Write-Host "🔹 Imagem 'alpine' não encontrada. Baixando..."
    docker pull alpine | Out-Null
    Write-Host "✅ Imagem 'alpine' baixada."
}

# 🔹 Backup
if ($Action -eq "backup") {
    Write-Host "🔹 Fazendo backup do volume $VolName para $BackupFile ..."
    docker run --rm `
      -v "${VolName}:/dados" `
      -v "${ProjectRoot}:/backup" `
      alpine sh -c "tar czf /backup/$(Split-Path $BackupFile -Leaf) /dados"
    Write-Host "✅ Backup concluído."
}

# 🔹 Restore
if ($Action -eq "restore") {
    if (-not (Test-Path $BackupFile)) {
        Write-Host "❌ Arquivo de backup não encontrado: $BackupFile"
        exit 1
    }
    Write-Host "🔹 Restaurando $BackupFile para volume $VolName ..."
    docker run --rm `
      -v "${VolName}:/dados" `
      -v "${ProjectRoot}:/backup" `
      alpine sh -c "tar xzf /backup/$(Split-Path $BackupFile -Leaf) -C /"
    Write-Host "✅ Restore concluído."
}
