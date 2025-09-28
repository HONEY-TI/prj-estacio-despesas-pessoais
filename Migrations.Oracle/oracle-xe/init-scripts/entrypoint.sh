#!/bin/bash
# entrypoint.sh

set -e
set -o pipefail

# Função de log
log() {
    echo "[INFO] $*"
}

tail -f /dev/null
