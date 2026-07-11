FROM alexfariakof/dotnet-sdk-with-node:latest

# Ferramentas do sistema
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        git \
        curl \
        wget \
        unzip \
        vim \
        nano \
        zsh \
        sudo \
        sqlite3 \
        netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

# eza
ARG EZA_VERSION=0.23.4
RUN curl -fsSL "https://github.com/eza-community/eza/releases/download/v${EZA_VERSION}/eza_x86_64-unknown-linux-musl.tar.gz" \
    | tar -xzf - -C /usr/local/bin \
    && chmod +x /usr/local/bin/eza

# Usuário dev (não root)
ARG USERNAME=developer
RUN groupadd ${USERNAME} \
    && useradd -m -s /bin/bash -g ${USERNAME} ${USERNAME} \
    && echo "${USERNAME} ALL=(ALL) NOPASSWD:ALL" >/etc/sudoers.d/${USERNAME} \
    && chmod 0440 /etc/sudoers.d/${USERNAME}

RUN chmod 1777 /tmp
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH="${PATH}:/home/${USERNAME}/.dotnet/tools"
ENV NUGET_PACKAGES=/home/${USERNAME}/.nuget/packages
ENV DOTNET_CLI_HOME=/home/${USERNAME}
# ── Bash personalizado 
RUN cat >> /home/${USERNAME}/.bashrc <<'EOF'
alias cls='clear'
alias ll='eza -l -h --group --icons'
alias la='eza -la -hs --group'
alias lt='eza --tree --level=2 --group'

function run() {
    case "$1" in
        swagger:dev)
            ASPNETCORE_ENVIRONMENT=Development \
            DOTNET_ENVIRONMENT=Development \
            dotnet run --launch-profile "Swagger Development"
            ;;

        swagger:prod)
            ASPNETCORE_ENVIRONMENT=Production \
            DOTNET_ENVIRONMENT=Production \
            dotnet run --launch-profile "Swagger Production"
            ;;

        dev)
            ASPNETCORE_ENVIRONMENT=Development \
            DOTNET_ENVIRONMENT=Development \
            dotnet run --launch-profile "Development"
            ;;

        prod)
            ASPNETCORE_ENVIRONMENT=Production \
            DOTNET_ENVIRONMENT=Production \
            dotnet run --launch-profile "Production"
            ;;

        *)
            echo "Uso: run {swagger:dev|swagger:prod|dev|prod}"
            return 1
            ;;
    esac
}

function ls() {
    command eza -la --group --group-directories-first "$@"
}

function parse_git_branch() {
    local branch
    branch=$(git branch --show-current 2>/dev/null)
    [ -n "$branch" ] && echo " ($branch)"
}

PS1='\[\e[0;32m\][\[\e[1;34m\]\u\[\e[0m\]:\[\e[1;36m\]\w\[\e[0m\]\[\e[1;33m\]$(parse_git_branch)\[\e[0m\]\[\e[0;32m\]]\[\e[0m\]\$'
EOF

RUN cat > /home/${USERNAME}/.bash_profile <<'EOF'
export PATH="$PATH:/home/developer/.dotnet/tools"

if [ -f ~/.bashrc ]; then
    source ~/.bashrc
fi
EOF

RUN chown ${USERNAME}:${USERNAME} \
    /home/${USERNAME}/.bashrc \
    /home/${USERNAME}/.bash_profile

RUN mkdir -p /home/${USERNAME}/.aspnet/DataProtection-Keys \
    && chown -R ${USERNAME}:${USERNAME} /home/${USERNAME}/.aspnet
RUN mkdir -p /home/${USERNAME}/.aspnet/https \
    && chown -R ${USERNAME}:${USERNAME} /home/${USERNAME}/.aspnet

COPY .devcontainer/docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

USER ${USERNAME}
WORKDIR /workspace
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
