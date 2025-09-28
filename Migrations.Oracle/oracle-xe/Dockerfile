# docker run --env-file .env ole-database-xe
FROM gvenzl/oracle-xe:21-slim

COPY ./init-scripts/entrypoint.sh //opt/oracle/entrypoint.sh
RUN sed -i 's/\r$//' //opt/oracle/entrypoint.sh && \
    sed -i '1s/^\xEF\xBB\xBF//' //opt/oracle/entrypoint.sh && \
    chmod +x //opt/oracle/entrypoint.sh

ENTRYPOINT ["bash", "//opt/oracle/entrypoint.sh"]
