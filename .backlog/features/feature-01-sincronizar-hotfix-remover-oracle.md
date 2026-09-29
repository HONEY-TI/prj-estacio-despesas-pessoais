---
name: feature-01-sincronizar-hotfix-remover-oracle
file: feature-01-sincronizar-hotfix-remover-oracle.md
description: >
  Sincronizar o hotfix do fork com o upstream e remover artefatos Oracle
  desnecessários do histórico versionado.
---

## Contexto / Problema

O fork precisa publicar uma correção sincronizada com o upstream, eliminando
artefatos grandes e desnecessários do Oracle e organizando os ajustes locais.

## Objetivo

Disponibilizar uma branch hotfix no fork para revisão no upstream, apontando
para a branch `dev`.

## Critérios de aceite

- [ ] PR aberta no upstream com a branch do fork como origem.
- [ ] Alterações locais revisadas e publicadas com referência à PR.
- [ ] PR atualizada com estatísticas reais e pronta para revisão.
