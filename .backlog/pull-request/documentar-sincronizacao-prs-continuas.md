---
name: documentar-sincronizacao-prs-continuas
pr: 21
title: "PR(#21)-Documentar Sincronização Contínua das PRs do Fork"
branch: feature/documentar-sincronizacao-prs-continuas
base: develop
extends: feature-01-documentar-sincronizacao-prs-continuas
status: open
---

## 📋 Descrição

Documentação das PRs abertas que acompanham as branches do fork HONEY-TI nos
destinos `dev` e `main` do repositório de origem, preservando o histórico das
PRs encerradas.

Feature relacionada: `.backlog/features/feature-01-documentar-sincronizacao-prs-continuas.md`

## 📊 Estatísticas

| Métrica | Valor |
| --- | ---: |
| 🌿 Branch de origem | `feature/documentar-sincronizacao-prs-continuas` |
| 🎯 Branch de destino | `develop` |
| 📝 Total de commits | 10 |
| 📁 Arquivos alterados | 12 |
| 📈 Linhas | 213 adicionadas / 2 removidas |

## PRs documentadas

- [PR #192](https://github.com/alexfariakof/app-despesas-pessoais/pull/192) — aberta, destino `dev`.
- [PR #193](https://github.com/alexfariakof/app-despesas-pessoais/pull/193) — aberta, destino `main`.
- PRs #191 e #194 — encerradas, preservadas como histórico.

## Checklist

- [x] Commits separados por arquivo
- [x] Referência da PR incluída nos commits de conteúdo
- [x] Alterações revisadas e enviadas para a branch
- [ ] Revisão funcional
- [ ] Validação em ambiente Linux/jail

## 📝 Commits de conteúdo

- `docs(readme): documentar sincronizacao das PRs`
- `docs(backlog): registrar feature de sincronizacao`
- `docs(backlog): registrar PR de sincronizacao`
- `chore(ci): atualizar permissões dos workflows`
- `security(config): atualizar proteções de arquivos sensíveis`
- `feat(ci): adicionar bootstrap no pós-checkout`
