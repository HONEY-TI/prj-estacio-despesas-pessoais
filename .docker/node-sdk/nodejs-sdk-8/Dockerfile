FROM alexfariakof/node-24.18:latest AS node
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

# =============================================================================
# Variáveis globais: .NET, Node.js, Angular, NestJS e ferramentas
# =============================================================================
ENV DOTNET_ROOT=/usr/share/dotnet \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    NUGET_XMLDOC_MODE=skip \
    NPM_CONFIG_UPDATE_NOTIFIER=false \
    PATH="/usr/share/dotnet:/usr/local/bin:/usr/local/sbin:/tools:/home/app/.local/bin:${PATH}"

RUN apt-get update && \           
        apt-get install -y \ 
        sudo \
        curl \
        ca-certificates \
        zsh \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

ARG EZA_VERSION=0.23.4
RUN curl -fsSL \
    "https://github.com/eza-community/eza/releases/download/v${EZA_VERSION}/eza_x86_64-unknown-linux-musl.tar.gz" \
    | tar -xzf - -C /usr/local/bin \
    && chmod +x /usr/local/bin/eza

# Ferramentas .NET disponíveis para todos os usuários
RUN dotnet tool install --tool-path /usr/local/bin dotnet-ef \
    && dotnet tool install --tool-path /usr/local/bin dotnet-watch \
    && dotnet tool install --tool-path /usr/local/bin dotnet-outdated-tool \
    && dotnet tool install --tool-path /usr/local/bin dotnet-reportgenerator-globaltool

# =============================================================================
# Node.js, npm, npx, Angular CLI e NestJS CLI
# =============================================================================
COPY --from=node /usr/local/bin/node /usr/local/bin/node
COPY --from=node /usr/local/lib/node_modules /usr/local/lib/node_modules

RUN ln -sf /usr/local/lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm \
    && ln -sf /usr/local/lib/node_modules/npm/bin/npx-cli.js /usr/local/bin/npx \
    && if [ -f /usr/local/lib/node_modules/@angular/cli/bin/ng.js ]; then \
        ln -sf /usr/local/lib/node_modules/@angular/cli/bin/ng.js /usr/local/bin/ng; \
    fi \
    && if [ -f /usr/local/lib/node_modules/@nestjs/cli/bin/nest.js ]; then \
        ln -sf /usr/local/lib/node_modules/@nestjs/cli/bin/nest.js /usr/local/bin/nest; \
    fi

RUN mkdir -p /tools \
    && ln -s /usr/local/bin/reportgenerator /tools/reportgenerator

# =============================================================================
# Shell global: bash (/etc/bash.bashrc)
# =============================================================================
RUN cat >> /etc/bash.bashrc <<'EOF'

export HISTFILE="$HOME/.bash_history"
export PATH="/bin:/usr/local/bin:/usr/local/sbin:$HOME/.local/bin:$PATH"

# =========================================================
# ALIASES
# =========================================================
alias cls='clear'
alias ll='eza -l -h --group --group-directories-first'
alias la='eza -la -hs --group --group-directories-first'
alias lt='eza --tree --level=2 --group --group-directories-first'

function ls() {
    command eza \
        -la \
        -h \
        --group \
        --group-directories-first \
        --color=auto \
        "$@"
}

function lsh() {
    local dir="${1:-.}"

    command eza \
        -la \
        -h \
        --group \
        --group-directories-first \
        --sort=size \
        --color=auto \
        "$dir"
}


# ==========================================
# VENV + GIT BRANCH NO PROMPT
# ==========================================
MY_VENV="/opt/venv"

parse_git_branch() {
    local branch
    branch=$(git branch --show-current 2>/dev/null)

    [ -n "$branch" ] && printf " (%s)" "$branch"
}

set_venv_prompt() {
    local venv_prompt=""

    PS1='\[\e[1;32m\]┌──\[\e[0m\] '"$venv_prompt"\
'\[\e[0;37m\](\[\e[1;34m\]\u\[\e[0;37m\]㉿\[\e[1;32m\]\h\[\e[0;37m\])\[\e[0m\]-'\
'\[\e[0;37m\][\w\[\e[1;33m\]$(parse_git_branch)\[\e[0;37m\]]\[\e[0m\]'\
'\n'\
'\[\e[1;32m\]└─\[\e[0m\]\$ '
}


function activate() {
    if [ -f "$MY_VENV/bin/activate" ]; then
        source "$MY_VENV/bin/activate"
        set_venv_prompt
    else
        echo "Erro: virtualenv não encontrado em $MY_VENV"
        return 1
    fi
}

function desactive() {
    if [[ -n "$VIRTUAL_ENV" ]] && type deactivate >/dev/null 2>&1; then
        deactivate
        set_venv_prompt
    else
        echo "Nenhum virtualenv ativo."
    fi
}

set_venv_prompt
EOF

# =============================================================================
# Shell global: zsh (/etc/zsh/zshrc)
# =============================================================================
RUN cat >> /etc/zshrc <<'EOF'
export HISTFILE="$HOME/.zsh_history"
export PATH="/bin:/usr/local/bin:/usr/local/sbin:$HOME/.local/bin:$PATH"

HISTFILE="$HOME/.zsh_history"
HISTSIZE=10000
SAVEHIST=20000

setopt APPEND_HISTORY
setopt INC_APPEND_HISTORY
setopt SHARE_HISTORY
setopt HIST_IGNORE_DUPS
setopt HIST_IGNORE_SPACE
setopt HIST_REDUCE_BLANKS
setopt PROMPT_SUBST

alias cls='clear'
alias py='python3'
alias ll='eza -lh --group --group-directories-first'
alias la='eza -lah --group --group-directories-first'
alias lt='eza --tree --level=2 --group'


function ls() {
    command eza \
        -la \
        -h \
        --group \
        --group-directories-first \
        --color=auto \
        "$@"
}

function lsh() {
    local dir="${1:-.}"

    command eza \
        -la \
        -h \
        --group \
        --group-directories-first \
        --sort=size \
        --color=auto \
        "$dir"
}

MY_VENV="/opt/venv"

activate() {
    if [[ -f "$MY_VENV/bin/activate" ]]; then
        source "$MY_VENV/bin/activate"
    else
        echo "Erro: virtualenv não encontrado em $MY_VENV"
        return 1
    fi
}

desactive() {
    if [[ -n "$VIRTUAL_ENV" ]] && type deactivate >/dev/null 2>&1; then
        deactivate
    else
        echo "Nenhum virtualenv ativo."
    fi
}

set_prompt() {
    local git_branch
    git_branch=$(git branch --show-current 2>/dev/null)

    PROMPT=""
    PROMPT+="%F{%(#.blue.green)}┌── "

    PROMPT+="(%B%F{%(#.red.blue)}%n㉿%m%b%F{%(#.blue.green)})"
    PROMPT+="-[%B%F{reset}%~%b%F{%(#.blue.green)}"

    if [[ -n "$git_branch" ]]; then
        PROMPT+=" %F{yellow}%B(${git_branch})%b%F{%(#.blue.green)}"
    fi

    PROMPT+="]"
    PROMPT+=$'\n'
    PROMPT+="└─%B%(#.%F{red}#.%F{blue}\$)%b%F{reset} "
}

autoload -Uz add-zsh-hook
add-zsh-hook precmd set_prompt
EOF

# Copia o Bash global para o usuário app
RUN cp /etc/bash.bashrc /home/app/.bashrc \
    && chown app:app /home/app/.bashrc

# Copia o zsh global para o usuário app
RUN cp /etc/zshrc /home/app/.zshrc \
    && chown app:app /home/app/.zshrc

# =============================================================================
# Shell history global: 
# =============================================================================
RUN mkdir -p /home/app/shell_history \
    && touch /home/app/shell_history/.bash_history \
             /home/app/shell_history/.zsh_history \
    && rm -rf /home/app/.bash_history \
               /home/app/.zsh_history \
    && ln -s /home/app/shell_history/.bash_history \
             /home/app/.bash_history \
    && ln -s /home/app/shell_history/.zsh_history \
             /home/app/.zsh_history \
    && chown -R app:app /home/app/shell_history

# =============================================================================
# Configuração do usuário app e do shell padrão
# =============================================================================
RUN set -eux; \
    id app >/dev/null 2>&1; \
    APP_HOME="$(getent passwd app | cut -d: -f6)"; \
    mkdir -p "$APP_HOME/shell_history"; \
    touch "$APP_HOME/shell_history/.bash_history" \
          "$APP_HOME/shell_history/.zsh_history"; \
    rm -f "$APP_HOME/.bash_history" "$APP_HOME/.zsh_history"; \
    ln -s "$APP_HOME/shell_history/.bash_history" "$APP_HOME/.bash_history"; \
    ln -s "$APP_HOME/shell_history/.zsh_history" "$APP_HOME/.zsh_history"; \
    cp /etc/bash.bashrc "$APP_HOME/.bashrc"; \
    cp /etc/zshrc "$APP_HOME/.zshrc"; \
    chown -R app:app "$APP_HOME"; \
    chsh -s /bin/zsh app

# =============================================================================
# 🔄 AUTO SWITCH — ROOT ➜ app
# =============================================================================
#  📌 false = DESATIVADO | true = ATIVADO
#=============================================================================
ARG ENABLE_SWITCH_USER=true
RUN if [ "$ENABLE_SWITCH_USER" = "true" ]; then \
        echo "🔄 Configurando auto switch: root ➜ app"; \
        printf '%s\n' \
            'if [ "${AUTO_SWITCHED}" != "1" ]; then' \
            '    export AUTO_SWITCHED=1' \
            '    exec su - app' \
            'fi' \
            > /root/.switch_user; \
        echo "source /root/.switch_user" >> /root/.bashrc; \
        echo "source /root/.switch_user" >> /root/.bash_profile; \
        echo ". /root/.switch_user" >> /root/.profile; \
    else \
        echo "⏭️  Auto switch root ➜ app: DESATIVADO"; \
    fi
