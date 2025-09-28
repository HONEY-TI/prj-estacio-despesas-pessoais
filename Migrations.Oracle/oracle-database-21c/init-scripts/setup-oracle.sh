#!/bin/bash
# steup-oracle.sh

set -e
set -o pipefail

# -------------------------------
# Funcoes utilitarias
# -------------------------------
log() {
    echo "[INFO] $*"
}

run_as_oracle() {
    sudo -u oracle bash -c "$1"
}

configure_timezone(){
    ln -sf /usr/share/zoneinfo/${TZ} /etc/localtime
    log "Timezone configurado ${TZ}"
}

configure_alias_cls(){
    grep -qxF "alias cls='clear'" /etc/bashrc || echo "alias cls='clear'" >> /etc/bashrc    
    log "Alias cls para clear configurado"
}

configure_hostname() {
    echo ${HOST_NAME} > /etc/hostname
    log "Hostname configurado como ${HOST_NAME}"
}

configure_linux_premissive(){
    log "Configurando SELinux para permissive"

    CONFIG_FILE="/etc/selinux/config"

    if [ -f "$CONFIG_FILE" ]; then
        if grep -q "^SELINUX=" "$CONFIG_FILE"; then
            log "Alterando configuração SELINUX para 'permissive' no arquivo existente"
            sed -i 's/^SELINUX=.*/SELINUX=permissive/' "$CONFIG_FILE"
        else
            log "Adicionando configuração SELINUX=permissive ao arquivo existente"
            echo "SELINUX=permissive" >> "$CONFIG_FILE"
        fi
    else
        log "Arquivo $CONFIG_FILE não encontrado. Criando arquivo com configuração SELINUX=permissive"
        cat << EOF > "$CONFIG_FILE"
# This file controls the state of SELinux on the system.
# SELINUX= can take one of these three values:
#     enforcing - SELinux security policy is enforced.
#     permissive - SELinux prints warnings instead of enforcing.
#     disabled - No SELinux policy is loaded.
SELINUX=permissive
SELINUXTYPE=targeted
EOF
    fi
    setenforce Permissive || true
}

create_users() {
    log "Criando usuários se não existirem"

    # Cria usuário oracle se não existir
    if ! id "oracle" &>/dev/null; then
        log "Criando usuário oracle"
        useradd -m -s /bin/bash oracle
    fi

    # Cria usuário migrations se não existir
    if ! id "migrations" &>/dev/null; then
        log "Criando usuário migrations"
        useradd -m -s /bin/bash migrations
    fi

    # Configura sudo para migrations executar sshd como root sem senha
    mkdir -p /etc/sudoers.d
    chmod 750 /etc/sudoers.d
    echo "migrations ALL=(root) NOPASSWD: /usr/sbin/sshd" > /etc/sudoers.d/migrations-sshd
    chmod 440 /etc/sudoers.d/migrations-sshd
}

set_user_passwords() {
    log "Configurando senhas dos usuários"

    echo "root:$ROOT_PWD" | chpasswd
    log "Senha do root configurada"

    echo "oracle:$ORACLE_PWD" | chpasswd
    log "Senha do oracle configurada"

    echo "migrations:$MIGRATIONS_PWD" | chpasswd
    log "Senha do migrations configurada"
}

configure_ssh() {
    log "Gerando chaves de host do SSH (se não existirem)"
    # Gera todas as chaves de host (RSA, ECDSA, ED25519)
    ssh-keygen -A

    # Garante que o diretório de runtime exista
    mkdir -p /var/run/sshd

    log "Configurando sshd_config"
    # Habilita root login e autenticação por senha, sem duplicar linhas
    grep -qxF "PermitRootLogin yes" /etc/ssh/sshd_config || echo "PermitRootLogin yes" >> /etc/ssh/sshd_config
    grep -qxF "PasswordAuthentication yes" /etc/ssh/sshd_config || echo "PasswordAuthentication yes" >> /etc/ssh/sshd_config

    # Ajusta permissões
    chown root:root /etc/ssh/sshd_config
    chmod 644 /etc/ssh/sshd_config

    chown root:root /etc/ssh/ssh_host_*
    chmod 600 /etc/ssh/ssh_host_*

    chown root:root /etc/ssh
    chmod 750 /etc/ssh
    sudo systemctl enable sshd
    log "SSH configurado com sucesso"
}


# -------------------------------
# Configuracoes do sistema
# -------------------------------
configure_system() {
    configure_timezone
    configure_alias_cls
    configure_hostname    
    configure_linux_premissive
    create_users
    configure_ssh
    set_user_passwords
    
    log "Atualizando sistema"
    yum update -y > /dev/null 2>&1   
}

# -------------------------------
# Configuração usuário Oracle
# -------------------------------
configure_oracle_user() {

    log "Criando diretórios para Oracle"
    if [ ! -d /u01/app/oracle/product/21.3.0/dbhome_1 ]; then
        log "Criando diretório do Oracle Home"
        mkdir -p /u01/app/oracle/product/21.3.0/dbhome_1
    fi
    
    mkdir -p /u01/app/oracle/oradata
    mkdir -p /u02/app/oracle/oradata
    mkdir -p /u03/app/oracle/oradata

    log "Ajustando propriedade e permissões das pastas /u01, /u02 e /u03 para o usuário 'oracle' e grupo 'oinstall'"
    chown -R oracle:oinstall /u01 /u02 /u03
    chmod -R 750 /u01 /u02 /u03
}

# -------------------------------
# Configuração ambiente Oracle
# -------------------------------
configure_oracle_user_profile() {
    run_as_oracle
    log "Criando .bash_profile para oracle"
    local profile="/home/oracle/.bash_profile"
    cat << 'EOF' >> "$profile"
# Oracle Settings
export TMP=/tmp
export TMPDIR=$TMP

export ORACLE_HOSTNAME=ora-server
export ORACLE_BASE=/u01/app/oracle
export ORACLE_HOME=$ORACLE_BASE/product/21.3.0/dbhome_1
export ORA_INVENTORY=/u01/app/oraInventory

export PATH=/usr/sbin:/usr/local/bin:$PATH
export PATH=$ORACLE_HOME/bin:$PATH

export LD_LIBRARY_PATH=$ORACLE_HOME/lib:/lib:/usr/lib
export CLASSPATH=$ORACLE_HOME/jlib:$ORACLE_HOME/rdbms/jlib
export EDITOR=vi
EOF

    chown oracle:oinstall "$profile"
    chmod 644 "$profile"
    
}

# -------------------------------
#  Execucao principal
# -------------------------------
main() {
    log "Iniciando configuração do container Oracle Linux"
    configure_system
    configure_oracle_user
    configure_oracle_user_profile
    log "Configuração concluída!"
}

main "$@"
