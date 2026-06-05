#!/bin/bash
# 01-create-user.sh
set -e

echo "[INFO] Criando usuário Oracle $APP_USER..."

sqlplus / as sysdba <<SQL
EXIT;
CREATE USER $APP_USER IDENTIFIED BY "$APP_USER_PASSWORD";
GRANT CONNECT, RESOURCE TO $APP_USER;
EXIT;
SQL

echo "[INFO] Usuário $APP_USER criado no Oracle"
