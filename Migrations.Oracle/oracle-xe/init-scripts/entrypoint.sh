#!/bin/bash
# Inicializa o banco e o listener do Oracle XE
echo "Iniciando Oracle XE..."

# Inicializa o listener
lsnrctl start

# Inicia a instância XE
sqlplus / as sysdba <<EOF
startup;
exit;
EOF

# Mantém o container rodando em foreground
tail -f /dev/null
