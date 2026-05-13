FROM gvenzl/oracle-xe:21-full

EXPOSE 1521

# Recebe do build
ARG ORACLE_PASSWORD
ARG APP_USER
ARG APP_USER_PASSWORD

# Passa para ambiente da imagem
ENV ORACLE_PASSWORD=${ORACLE_PASSWORD}
ENV APP_USER=${APP_USER}
ENV APP_USER_PASSWORD=${APP_USER_PASSWORD}

# Scripts de inicialização
COPY ./init-scripts/*.sh /container-entrypoint-initdb.d/
RUN sed -i 's/\r$//' /container-entrypoint-initdb.d/*.sh && \
    sed -i '1s/^\xEF\xBB\xBF//' /container-entrypoint-initdb.d/*.sh && \
    chmod +x /container-entrypoint-initdb.d/*.sh

