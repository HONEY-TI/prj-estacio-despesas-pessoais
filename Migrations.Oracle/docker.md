1️⃣ “Congelar” um container atual em uma imagem (docker commit)

O docker commit pega o filesystem do container no estado atual e cria uma imagem nova:

# Descubra o nome ou ID do container
docker ps



# Exemplo: Criar imagem apartir de um container 
  > docker commit 5be7795e618e alexfariakof/ole-database-xe:backup
    Isso cria a imagem alexfariakof/ole-database21c:custom.

# Baixar alpine para fazer backup and restotr dos volumes
  > docker pull alpine

# Exemplo:Criar backup .tar.gz do volume
  ## Ambientes Linux 
    > docker run --rm -v oracle-xe_oradata:/dados -v $(pwd):/backup  alpine sh -c "tar czf /backup/oracle-xe_oradata-backup.tgz /dados"
 
  ## Ambientes Window Windows Usando PowerShell
    > docker run --rm  -v oracle-xe_oradata:/dados -v "${PWD}:/backup" alpine sh -c "tar czf /backup/oracle-xe_oradata-backup.tgz /dados"


# Exemplo : Restuaração de backup .tar.gz do volume
  ## Ambientes Linux 
    > docker run --rm -v oracle-xe_oradata:/dados -v $(pwd):/backup  alpine sh -c "tar xzf /backup/oracle_data-backup.tgz -C /"
 
  ## Ambientes Window Windows Usando PowerShell
    > docker run --rm  -v oracle-xe_oradata:/dados -v "${PWD}:/backup" alpine sh -c "tar czf /backup/oracle-xe_oradata-backup.tgz /dados"

docker run -d --name oracle-xe-restaurado --env-file .env -p 1521:1521 -p 22:22 -v oradata:/opt/oracle/oradata \
alexfariakof/ole-database21c:backup



O estado de disco está lá (arquivos, configs).

Limitação: não grava variáveis, CMD, ENTRYPOINT que você configurou com docker run (eles podem ser passados com docker commit --change).