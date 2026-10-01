FROM mcr.microsoft.com/dotnet/sdk:10.0
# =============================================================================
# STEP 1 - Ambiente
# =============================================================================

ENV DEBIAN_FRONTEND=noninteractive
ENV HOME=/home/ubuntu
ENV UBUNTU_ROOT=/usr/share/ubuntu
ENV PATH="/home/ubuntu/.dotnet/tools:/home/ubuntu/.npm-global/bin:/home/ubuntu/.local/bin:/usr/local/bin:/usr/bin:/bin:${PATH}"

# =============================================================================
# STEP 2 - Pacotes do sistema
# =============================================================================

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        sudo \
        openssl \
        curl \
        ca-certificates \        
        iptables \
        ipset \
        iproute2 \
        iputils-ping \
        dnsutils \
        jq \
        aggregate \
        procps \
        git \
        gh \
        openssh-client \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# =============================================================================
# STEP 3 - Diretórios do usuário ubuntu
# =============================================================================

RUN mkdir -p \
        /home/ubuntu/.ubuntu/tools \
        /home/ubuntu/.npm-global \
        /home/ubuntu/.ssh \
        /home/ubuntu/.config \
        /home/ubuntu/.cache \
        /home/ubuntu/.local/bin 

# =============================================================================
# STEP 4 - .NET Tools
# =============================================================================
RUN dotnet tool install --global dotnet-reportgenerator-globaltool
RUN mkdir -p /tools && \
    ln -s /root/.dotnet/tools/reportgenerator /tools/reportgenerator

RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && \
    apt-get install -y nodejs

RUN npm config set prefix /home/ubuntu/.npm-global

RUN npm install -g \
    npm@12.0.2 \
    @angular/cli@21.2.13 \
    @nestjs/cli@11.0.2 \
    @openai/codex


# =============================================================================
# STEP 6 - Eza
# =============================================================================

ARG EZA_VERSION=0.23.4

RUN curl -fsSL \
    "https://github.com/eza-community/eza/releases/download/v${EZA_VERSION}/eza_x86_64-unknown-linux-musl.tar.gz" \
    | tar -xzf - -C /usr/local/bin \
    && chmod +x /usr/local/bin/eza

# =============================================================================
# STEP 7 - SSH
# =============================================================================

#RUN mkdir -p /home/ubuntu/.ssh \
#    && chown -R ubuntu:ubuntu /home/ubuntu/.ssh \
#    && chmod 700 /home/ubuntu/.ssh

# =============================================================================
# STEP 8 - Git
# =============================================================================

RUN printf '%s\n' \
    '[user]' \
    '    name = Alex Ribeiro de Faria' \
    '    email = 135660435+alexribeirofaria@github.com' \
    > /home/ubuntu/.gitconfig 

# =============================================================================
# STEP 9 - Scripts
# =============================================================================

# ── Firewall ────────────────────────────────────────────────────────────────
COPY --chown=root:root --chmod=0755 \
    init-firewall.sh setup-codex.sh docker-entrypoint.sh \
    /usr/local/bin/

COPY setup-codex.sh /usr/local/bin/setup-codex.sh
RUN chmod +x /usr/local/bin/setup-codex.sh

COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod 775 /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# =============================================================================
# STEP 10 - Sudo
# =============================================================================

RUN mkdir -p /home/ubuntu/.npm-global \
    && npm config set prefix /home/ubuntu/.npm-global

    RUN set -eux; \
    printf '%s\n' \
    'Defaults:ubuntu env_reset, !setenv, secure_path="/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"' \
    'ubuntu ALL=(root) NOPASSWD: \' \
    '    /usr/local/bin/init-firewall.sh "", \' \
    '    /usr/local/bin/setup-codex.sh "", \' \
    '    /usr/local/bin/docker-entrypoint.sh ""' \
    > /etc/sudoers.d/10-ubuntu-dev; \
    chown root:root /etc/sudoers.d/10-ubuntu-dev; \
    chmod 0440 /etc/sudoers.d/10-ubuntu-dev; \
    visudo -cf /etc/sudoers.d/10-ubuntu-dev

#RUN echo "ubuntu ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers \
#    && printf '%s\n' \
#    'Cmnd_Alias SHELLS = /bin/bash, /bin/sh, /usr/bin/zsh, /usr/bin/su, \' \
#    '                     /usr/bin/sudo -i, /usr/bin/sudo -s, \' \
#    '                     /usr/bin/newgrp, /usr/bin/newgrp *' \
#    '%sudo ALL=(ALL) ALL, !SHELLS' \
#    > /etc/sudoers.d/restrict-shells \
#    && chmod 0440 /etc/sudoers.d/restrict-shells \
#    && visudo -cf /etc/sudoers.d/restrict-shells

RUN printf '%s\n' \
    'ubuntu ALL=(root) NOPASSWD: /usr/local/bin/init-firewall.sh, /usr/local/bin/setup-codex.sh, /usr/local/bin/docker-entrypoint.sh' \
    > /etc/sudoers.d/ubuntu-dev \
    && chmod 0440 /etc/sudoers.d/ubuntu-dev \
    && visudo -cf /etc/sudoers.d/ubuntu-dev


# =============================================================================
# STEP 11 - Bash
# =============================================================================

RUN cat >> /etc/bash.bashrc <<'EOF'
export HOME="/home/ubuntu"
export PATH="/home/ubuntu/.dotnet/tools:/home/ubuntu/.npm-global/bin:/home/ubuntu/.local/bin:/usr/local/bin:/usr/bin:/bin:$PATH"

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
        --color=always \
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
        --color=always \
        "$dir"
}

# ==========================================

# GIT BRANCH NO PROMPT

# ==========================================

parse_git_branch() {
    local branch
    local git_dir

    branch=$(git branch --show-current 2>/dev/null)

    if [ -n "$branch" ]; then
        printf " (%s)" "$branch"
        return
    fi

    git_dir=$(git rev-parse --git-dir 2>/dev/null)

    if [ -n "$git_dir" ]; then
        branch=$(git -C "$(git rev-parse --show-toplevel 2>/dev/null)" \
            branch --show-current 2>/dev/null)

        [ -n "$branch" ] && printf " (%s)" "$branch"
    fi
}

set_git_prompt() {
    PS1='\[\e[1;32m\]┌──\[\e[0m\] '\
'\[\e[0;37m\](\[\e[1;34m\]\u\[\e[0;37m\]㉿\[\e[1;32m\]\h\[\e[0;37m\])\[\e[0m\]-'\
'\[\e[0;37m\][\w\[\e[1;33m\]$(parse_git_branch)\[\e[0;37m\]]\[\e[0m\]'\
'\n'\
'\[\e[1;32m\]└─\[\e[0m\]\$ '
}

set_git_prompt

EOF

RUN cp -r /etc/bash.bashrc /home/ubuntu/.bashrc \
    && chown ubuntu:ubuntu /home/ubuntu/.bashrc

# =============================================================================
# STEP 13 - GITHUB SET and AUTO SWITCH — ROOT 
# ============================================================================

ARG GITHUB_TOKEN
RUN if [ -n "$GITHUB_TOKEN" ]; then \
      echo "${GITHUB_TOKEN}" | gh auth login --with-token || echo "gh auth falhou, seguindo sem autenticar"; \
    else \
      echo "GITHUB_TOKEN não fornecido, pulando gh auth login"; \
    fi

# =============================================================================
# 📌 false = DESATIVADO | true = ATIVADO
# =============================================================================
ARG ENABLE_SWITCH_USER=true
RUN if [ "$ENABLE_SWITCH_USER" = "true" ]; then \
        echo "🔄 Configurando auto switch: root ➜ ubuntu"; \
        printf '%s\n' \
            'if [ "${AUTO_SWITCHED}" != "1" ]; then' \
            '    export AUTO_SWITCHED=1' \
            '    exec su - ubuntu' \
            'fi' \
            > /root/.switch_user; \
        echo "source /root/.switch_user" >> /root/.bashrc; \
        echo "source /root/.switch_user" >> /root/.bash_profile; \
        echo ". /root/.switch_user" >> /root/.profile; \
    else \
        echo "⏭️  Auto switch root ➜ ubuntu: DESATIVADO"; \
    fi

# =============================================================================
# STEP 14 - execução
# =============================================================================
WORKDIR /workspace
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
