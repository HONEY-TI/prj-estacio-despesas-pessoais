FROM gvenzl/oracle-xe:21-slim

# Recebe do build
ARG ORACLE_PASSWORD
ARG APP_USER
ARG APP_USER_PASSWORD

# Passa para ambiente da imagem
ENV ORACLE_PASSWORD=${ORACLE_PASSWORD}
ENV APP_USER=${APP_USER}
ENV APP_USER_PASSWORD=${APP_USER_PASSWORD}

COPY ./init-scripts/entrypoint.sh /opt/oracle/entrypoint.sh
RUN sed -i 's/\r$//' /opt/oracle/entrypoint.sh && \
    sed -i '1s/^\xEF\xBB\xBF//' /opt/oracle/entrypoint.sh && \
    chmod +x /opt/oracle/entrypoint.sh

EXPOSE 1525

CMD ["bash", "-c", "/opt/oracle/entrypoint.sh"]
