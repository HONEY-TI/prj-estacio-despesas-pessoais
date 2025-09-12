param(
    [switch]$w,
    [switch]$d
)

# Path onde é gerado o relatório coverage
$reportPath = ".\coverage\lcov-report"

# Função para matar processos com base no nome do processo
function Stop-ProcessesByName {
  $processes = Get-Process | Where-Object { $_.ProcessName -like 'npm*' -or $_.ProcessName -like '*job*' } | Where-Object { $_.MainWindowTitle -eq '' }
  if ($processes.Count -gt 0) {
      $processes | ForEach-Object { Stop-Process -Id $_.Id -Force }
  }
}

function Open-Report-Coverage {
  $latestDir = Get-ChildItem -Directory -Path $reportPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1

  if ($null -ne $latestDir) {
    Invoke-Item "$reportPath\index.html"
  }
  else {
    Write-Host "Nenhum diretório de resultados encontrado."
  }
}

# Executa os testes unitários e gera o relatório
npm run test:coverage

# Encerra qualquer processo em segundo plano relacionado ao comando npm run test:watch
Stop-ProcessesByName

if ($w) {
  Start-Job -ScriptBlock { npm run test:watch }
  Open-Report-Coverage
}
elseif ($d) {
  Start-Job -ScriptBlock { npm run test:watch }
  Open-Report-Coverage
  npm run test:debug
}
else {
  Open-Report-Coverage
}

Get-Process | Where-Object { $_.ProcessName -like 'npm*' -or $_.ProcessName -like '*job*' } | Where-Object { $_.MainWindowTitle -eq '' }
