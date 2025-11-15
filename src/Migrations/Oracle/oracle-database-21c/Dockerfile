# docker run --env-file .env ole-database
# docker update --cpus=2 --memory=4g --memory-swap=8g ole-database

FROM container-registry.oracle.com/os/oraclelinux:8.10

ARG ROOT_PWD
ARG ORACLE_PWD
ARG MIGRATIONS_PWD
ARG TZ
ARG ORACLE_ACCEPT_LICENSE
ARG ORACLE_BASE
ARG HOST_NAME

ENV ORACLE_BASE=${ORACLE_BASE}
ENV ORACLE_HOME=${ORACLE_BASE}/product/21.3.0/dbhome_1
ENV ROOT_PWD=${ROOT_PWD}
ENV ORACLE_PWD=${ORACLE_PWD}
ENV MIGRATIONS_PWD=${MIGRATIONS_PWD}
ENV TZ=${TZ}
ENV HOST_NAME=${HOST_NAME}

EXPOSE 22
EXPOSE 1521
EXPOSE 5500

# Instala pacotes necessários e pré-requisitos do Oracle 21c
RUN dnf -y install \
    oracle-database-preinstall-21c \
    glibc-langpack-en \
    glibc-langpack-pt \
    nfs-utils \
    perf \
    "@Development Tools" \
    net-tools \
    iproute \
    procps-ng \
    gnome-system-monitor \
    openssh-server \
    openssh-clients \
    sudo \
    unzip \
    && dnf clean all

# Copia as partes do ZIP para o container
COPY ./softwares/LINUX.X64_213000_db_home.zip.part* $ORACLE_HOME/softwares/

# Junta as partes, descompacta e remove temporários
RUN cat $ORACLE_HOME/softwares/LINUX.X64_213000_db_home.zip.part* \
    > $ORACLE_HOME/softwares/LINUX.X64_213000_db_home.zip && \
    unzip $ORACLE_HOME/softwares/LINUX.X64_213000_db_home.zip -d $ORACLE_HOME/ > /dev/null 2>&1 && \
    rm -f $ORACLE_HOME/softwares/LINUX.X64_213000_db_home.zip \
          $ORACLE_HOME/softwares/LINUX.X64_213000_db_home.zip.part* && \
    rmdir $ORACLE_HOME/softwares || true

# Copia o init-scripts e corrige BOM + CRLF e Executa o script de configuração
COPY ./init-scripts/setup-oracle.sh /etc/init.d/setup-oracle.sh
RUN sed -i 's/\r$//' /etc/init.d/setup-oracle.sh && \
    sed -i '1s/^\xEF\xBB\xBF//' /etc/init.d/setup-oracle.sh && \
    chmod +x /etc/init.d/setup-oracle.sh
RUN /bin/bash /etc/init.d/setup-oracle.sh

COPY ./init-scripts/entrypoint.sh /etc/init.d/entrypoint.sh
RUN sed -i 's/\r$//' /etc/init.d/entrypoint.sh && \
    sed -i '1s/^\xEF\xBB\xBF//' /etc/init.d/entrypoint.sh && \
    chmod +x /etc/init.d/entrypoint.sh


USER migrations 
WORKDIR /home/migrations
ENTRYPOINT ["bash", "/etc/init.d/entrypoint.sh"]

