1️⃣ “Congelar” um container atual em uma imagem (docker commit)

O docker commit pega o filesystem do container no estado atual e cria uma imagem nova:

# Descubra o nome ou ID do container
docker ps

# Exemplo: container chama-se oracle-xe
docker commit 5be7795e618e alexfariakof/ole-database21c:backup


Isso cria a imagem alexfariakof/ole-database21c:custom.

Depois você pode rodar:

docker run -d --name oracle-xe-restaurado --env-file .env -p 1521:1521 -p 22:22 -v oradata:/opt/oracle/oradata \
alexfariakof/ole-database21c:backup



O estado de disco está lá (arquivos, configs).

Limitação: não grava variáveis, CMD, ENTRYPOINT que você configurou com docker run (eles podem ser passados com docker commit --change).