#!/bin/bash
# entrypoint.sh

set -e
set -o pipefail

# Função de log
log() {
    echo "[INFO] $*"
}

log "Iniciando SSH em background..."
sudo /usr/sbin/sshd

tail -f /dev/null
