#!/usr/bin/env bash

set -euo pipefail

# ==========================================================
# RE-EXECUÇÃO A PARTIR DE CÓPIA TEMPORÁRIA
# ==========================================================
# Este script vive dentro de um submodule (.ai/tools/bootstrap.sh) e, ao
# rodar, move o checkout do próprio .ai. O Bash lê o arquivo aos poucos,
# então se o conteúdo mudar no meio da execução o comportamento fica
# imprevisível. Por isso rodamos a partir de uma cópia em /tmp.
if [ -z "${BOOTSTRAP_REEXEC:-}" ]; then
  BOOTSTRAP_ORIGIN_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
  BOOTSTRAP_TMP="$(mktemp "${TMPDIR:-/tmp}/bootstrap.XXXXXX")"
  cp -- "${BASH_SOURCE[0]}" "$BOOTSTRAP_TMP"
  export BOOTSTRAP_ORIGIN_DIR BOOTSTRAP_TMP
  BOOTSTRAP_REEXEC=1 exec bash "$BOOTSTRAP_TMP" "$@"
fi

trap 'rm -f -- "${BOOTSTRAP_TMP:-}"' EXIT

echo "🚀 [Insights] Starting full repository bootstrap..."

# Pasta ORIGINAL do script (não a da cópia temporária)
SCRIPT_DIR="${BOOTSTRAP_ORIGIN_DIR:-$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)}"

# ==========================================================
# RESOLUÇÃO DA RAIZ DO PROJETO (REPOSITÓRIO MAIS EXTERNO)
# ==========================================================
# Função compartilhada (ver lib/git-root.sh). Override: PROJECT_ROOT=/caminho
# shellcheck source=lib/git-root.sh
# >>> git-root-lib (GERADO por embed-lib.sh a partir de lib/git-root.sh — não edite aqui)
# resolve_outer_repo <dir-do-repo>
#
# Sobe pelos SUPERPROJETOS (submodules registrados) até o mais
# externo. Funciona com submodules aninhados em vários níveis.
resolve_outer_repo() {
  local TOP="$1"
  local SUPER

  while true; do
    SUPER="$(git -C "$TOP" rev-parse --show-superproject-working-tree 2>/dev/null || true)"
    [ -z "$SUPER" ] && break
    TOP="$SUPER"
  done

  printf '%s\n' "$TOP"
}

# resolve_project_root <dir-inicial>
#
# O .ai e SEMPRE um submodule: a raiz do projeto e o superprojeto
# dele (o repositorio PAI mais externo). Imprime esse caminho.
# Retorna 1 se <dir-inicial> nao estiver dentro de um repositorio.
resolve_project_root() {
  local TOP

  TOP="$(git -C "$1" rev-parse --show-toplevel 2>/dev/null)" || return 1

  resolve_outer_repo "$TOP"
}

# init_project_root <dir-inicial>
#
# Define PROJECT_ROOT e entra nele (cd).
# Override manual: PROJECT_ROOT=/caminho ./script.sh
#
# Se o script NAO estiver dentro de um submodule (sem superprojeto),
# avisa e usa o proprio repositorio como raiz.
init_project_root() {
  local START="$1"
  local OWN_TOP

  if [ -n "${PROJECT_ROOT:-}" ]; then
    PROJECT_ROOT="$(git -C "$PROJECT_ROOT" rev-parse --show-toplevel 2>/dev/null)" || {
      echo "❌ PROJECT_ROOT informado não é um repositório Git." >&2
      return 1
    }
  else
    OWN_TOP="$(git -C "$START" rev-parse --show-toplevel 2>/dev/null)" || {
      echo "❌ Não encontrei um repositório Git a partir de: $START" >&2
      return 1
    }

    PROJECT_ROOT="$(resolve_outer_repo "$OWN_TOP")"

    if [ "$PROJECT_ROOT" = "$OWN_TOP" ]; then
      echo "⚠️  Este script não está dentro de um submodule (o .ai deveria ser um submodule)." >&2
      echo "   Usando como raiz: $PROJECT_ROOT" >&2
      echo "   Para outro projeto: PROJECT_ROOT=/caminho ./script.sh" >&2
    fi
  fi

  cd "$PROJECT_ROOT" || return 1
}

# ============================================================
# Ordem dos submodules: o .ai SEMPRE vem primeiro
# ============================================================
#
# Caminho do submodule prioritário (relativo ao repositório).
# Override: AI_SUBMODULE_PATH=outro/caminho
#
# submodule_entries <repo>
#   Imprime as linhas "submodule.<nome>.path <caminho>" do .gitmodules
#   de <repo>, com o submodule prioritário (.ai) na PRIMEIRA posição.
#   A ordem relativa dos demais é preservada.
submodule_entries() {
  local REPO="$1"
  local FIRST="${AI_SUBMODULE_PATH:-.ai}"
  local ALL

  [ -f "$REPO/.gitmodules" ] || return 0

  ALL="$(
    git -C "$REPO" config -f .gitmodules \
      --get-regexp '^submodule\..*\.path$' 2>/dev/null || true
  )"

  [ -z "$ALL" ] && return 0

  printf '%s\n' "$ALL" | awk -v first="$FIRST" '
    {
      p = $0
      sub(/^[^ ]+ /, "", p)
      if (p == first) { print; next }
      rest[++n] = $0
    }
    END { for (i = 1; i <= n; i++) print rest[i] }
  '
}

# submodule_paths <repo> — igual a submodule_entries, só os caminhos.
submodule_paths() {
  submodule_entries "$1" | sed 's/^[^ ]* //'
}
# <<< git-root-lib

init_project_root "$SCRIPT_DIR" || exit 1

# Ordem de prioridade da branch de cada submodule (primeira que existir no ORIGIN)
BRANCH_PRIORITY=(develop master main)

DO_PUSH="${PUSH:-0}"
ASSUME_YES="${ASSUME_YES:-0}"

for ARG in "$@"; do
  case "$ARG" in
    --push) DO_PUSH=1 ;;
    --yes|-y) ASSUME_YES=1 ;;
    *) echo "❌ Unknown option: $ARG"; exit 1 ;;
  esac
done

# ==========================================================
# ESTADO GLOBAL
# ==========================================================

declare -a CHANGED_PATHS=()
declare -a CHANGE_NOTES=()

# Submodules ignorados (chave = caminho absoluto do submodule)
declare -A SKIPPED=()

# Preenchidas por resolve_remote_branch
REMOTE_BRANCH=""
REMOTE_HASH=""

# ==========================================================
# PROTEÇÃO DO REPOSITÓRIO RAIZ (SOMENTE LEITURA)
# ==========================================================

# Fotografia do estado do raiz: HEAD + índice (inclui ponteiros dos submodules)
root_snapshot() {
  {
    git -C "$PROJECT_ROOT" rev-parse HEAD 2>/dev/null || echo "no-head"
    git -C "$PROJECT_ROOT" ls-files -s
  } | sha1sum | cut -d' ' -f1
}

ROOT_BEFORE="$(root_snapshot)"

# Aborta se algum comando de escrita (update-index, commit, push)
# for direcionado ao repositório raiz
assert_not_root() {
  local TARGET_REPO="$1"
  local ACTION="$2"

  if [ "$TARGET_REPO" = "$PROJECT_ROOT" ]; then
    echo "❌ Refusing to run '$ACTION' on the root repository (read-only)"
    exit 1
  fi
}

echo "📁 Script dir:   $SCRIPT_DIR"
echo "📁 Project root: $PROJECT_ROOT"
echo "🔒 Root repository is read-only: only submodules will be changed"
echo "🌿 Branch rule (from origin): ${BRANCH_PRIORITY[*]}"

# ==========================================================
# FUNÇÕES AUXILIARES
# ==========================================================

# Pergunta Y/N. Padrão: N.
# Sem terminal interativo (CI, pipe) → responde N automaticamente,
# a menos que --yes / ASSUME_YES=1 tenha sido informado.
ask_continue() {
  local PROMPT="$1"
  local ANSWER=""

  if [ "$ASSUME_YES" = "1" ]; then
    echo "   ⚙️  --yes enabled: continuing without asking"
    return 0
  fi

  if [ ! -t 0 ]; then
    echo "   ⚙️  Non-interactive shell: defaulting to N (skip)"
    return 1
  fi

  read -r -p "$PROMPT [y/N]: " ANSWER

  case "${ANSWER,,}" in
    y|yes|s|sim) return 0 ;;
    *)           return 1 ;;
  esac
}

# Descobre a branch alvo consultando DIRETAMENTE o origin (ls-remote).
# Regra: develop → master → main (primeira que existir no origin).
# Nunca usa branches ou refs locais.
#
# Retorno: 0 = encontrou (REMOTE_BRANCH e REMOTE_HASH preenchidos)
#          1 = origin acessível, mas nenhuma branch da regra existe
#          2 = origin inacessível
resolve_remote_branch() {
  local REPO_PATH="$1"
  local HEADS
  local BRANCH_NAME
  local FOUND

  REMOTE_BRANCH=""
  REMOTE_HASH=""

  if ! HEADS="$(
    git -C "$REPO_PATH" ls-remote --heads origin "${BRANCH_PRIORITY[@]}" 2>/dev/null
  )"; then
    return 2
  fi

  for BRANCH_NAME in "${BRANCH_PRIORITY[@]}"; do

    FOUND="$(
      printf '%s\n' "$HEADS" |
      awk -v ref="refs/heads/$BRANCH_NAME" '$2 == ref { print $1 }'
    )"

    if [ -n "$FOUND" ]; then
      REMOTE_BRANCH="$BRANCH_NAME"
      REMOTE_HASH="$FOUND"
      return 0
    fi

  done

  return 1
}

# Retorna 0 = seguro para atualizar
# Retorna 1 = tem alterações locais e o usuário NÃO quer continuar
check_local_changes() {
  local REPO_ROOT="$1"
  local SUB_PATH="$2"
  local ABS_SUB_PATH="$REPO_ROOT/$SUB_PATH"
  local STATUS

  # Submodule ainda não inicializado → nada a proteger
  if [ ! -e "$ABS_SUB_PATH/.git" ]; then
    return 0
  fi

  STATUS="$(git -C "$ABS_SUB_PATH" status --porcelain 2>/dev/null || true)"

  if [ -z "$STATUS" ]; then
    return 0
  fi

  echo ""
  echo "⚠️  [$SUB_PATH] Local changes detected (possibly made externally):"
  printf '%s\n' "$STATUS" | head -n 15 | sed 's/^/     /'

  if [ "$(printf '%s\n' "$STATUS" | wc -l)" -gt 15 ]; then
    echo "     ... (more files not shown)"
  fi

  echo "   Updating may overwrite or conflict with these changes."

  if ask_continue "❓ [$SUB_PATH] Continue updating this submodule?"; then
    echo "   ▶️  Continuing with [$SUB_PATH]"
    return 0
  fi

  echo "⏭️  [$SUB_PATH] Skipped (local changes preserved)"
  SKIPPED["$ABS_SUB_PATH"]=1
  return 1
}

# ==========================================================
# PROCESSA UM SUBMODULE
# ==========================================================

process_submodule() {
  local REPO_ROOT="$1"
  local NAME="$2"
  local SUB_PATH="$3"

  local ABS_SUB_PATH="$REPO_ROOT/$SUB_PATH"
  local RC=0
  local BRANCH
  local TARGET_HASH
  local SHORT_HASH
  local CURRENT_HASH
  local LOCAL_AHEAD

  echo ""
  echo "🤖 [$SUB_PATH] Checking origin..."
  echo "   → repository: $REPO_ROOT"

  # --------------------------------------------------------
  # Branch + hash vêm SEMPRE do origin do submodule
  # --------------------------------------------------------
  resolve_remote_branch "$ABS_SUB_PATH" || RC=$?

  if [ "$RC" -eq 2 ]; then
    echo "❌ [$SUB_PATH] Cannot reach origin. Skipped."
    SKIPPED["$ABS_SUB_PATH"]=1
    return 0
  fi

  if [ "$RC" -ne 0 ]; then
    echo "⚠️ [$SUB_PATH] None of (${BRANCH_PRIORITY[*]}) exists on origin. Skipped."
    SKIPPED["$ABS_SUB_PATH"]=1
    return 0
  fi

  BRANCH="$REMOTE_BRANCH"
  TARGET_HASH="$REMOTE_HASH"
  SHORT_HASH="${TARGET_HASH:0:7}"

  # Baixa os objetos da branch escolhida (apenas do origin)
  git -C "$ABS_SUB_PATH" \
    fetch --quiet origin "+refs/heads/$BRANCH:refs/remotes/origin/$BRANCH"

  if ! git -C "$ABS_SUB_PATH" cat-file -e "${TARGET_HASH}^{commit}" 2>/dev/null; then
    echo "❌ [$SUB_PATH] Commit $SHORT_HASH from origin/$BRANCH not available. Skipped."
    SKIPPED["$ABS_SUB_PATH"]=1
    return 0
  fi

  echo "   → branch chosen: $BRANCH"
  echo "   → origin/$BRANCH hash: $SHORT_HASH"

  # --------------------------------------------------------
  # Informativo: commits locais NUNCA são usados nem enviados
  # --------------------------------------------------------
  if git -C "$ABS_SUB_PATH" show-ref --verify --quiet "refs/heads/$BRANCH"; then

    LOCAL_AHEAD="$(
      git -C "$ABS_SUB_PATH" \
        rev-list --count "refs/remotes/origin/$BRANCH..refs/heads/$BRANCH" 2>/dev/null || echo 0
    )"

    if [ "$LOCAL_AHEAD" -gt 0 ]; then
      echo "ℹ️ [$SUB_PATH] Local '$BRANCH' has $LOCAL_AHEAD commit(s) not on origin."
      echo "   They are ignored (never used as hash, never pushed)."
    fi
  fi

  # --------------------------------------------------------
  # Submodule direto da RAIZ: a raiz NUNCA é alterada.
  # Apenas o checkout dentro do submodule é movido (sem force).
  # --------------------------------------------------------
  if [ "$REPO_ROOT" = "$PROJECT_ROOT" ]; then

    CURRENT_HASH="$(git -C "$ABS_SUB_PATH" rev-parse HEAD)"

    if [ "$CURRENT_HASH" = "$TARGET_HASH" ]; then
      echo "ℹ️ [$SUB_PATH] already at origin/$BRANCH"
      return 0
    fi

    echo "🔄 [$SUB_PATH] Moving submodule checkout: ${CURRENT_HASH:0:7} → $SHORT_HASH"
    echo "   🔒 Root repository untouched (pointer is not staged or committed)"

    if ! git -C "$ABS_SUB_PATH" \
      checkout --quiet --detach "$TARGET_HASH"; then

      echo "⚠️ [$SUB_PATH] Checkout failed (conflict with local files). Left as is."
      SKIPPED["$ABS_SUB_PATH"]=1
    fi

    return 0
  fi

  # --------------------------------------------------------
  # Submodule aninhado: atualiza o ponteiro no submodule PAI
  # --------------------------------------------------------
  CURRENT_HASH="$(
    git -C "$REPO_ROOT" ls-files -s -- "$SUB_PATH" | awk '{ print $2 }'
  )"

  if [ "$TARGET_HASH" = "$CURRENT_HASH" ]; then
    echo "ℹ️ [$SUB_PATH] pointer already matches origin/$BRANCH"
    return 0
  fi

  echo "🔄 [$SUB_PATH] Hash changed:"
  echo "   ${CURRENT_HASH:0:7} → $SHORT_HASH (origin/$BRANCH)"

  assert_not_root "$REPO_ROOT" "update-index"

  git -C "$REPO_ROOT" \
    update-index \
    --add \
    --cacheinfo "160000,$TARGET_HASH,$SUB_PATH"

  # Move o checkout já agora, para a recursão enxergar o conteúdo novo
  git -C "$REPO_ROOT" submodule update --init -- "$SUB_PATH"

  CHANGED_PATHS+=("$REPO_ROOT:$SUB_PATH")
  CHANGE_NOTES+=(
    "- $SUB_PATH: ${CURRENT_HASH:0:7} -> $SHORT_HASH (origin/$BRANCH)"
  )
}

# ==========================================================
# FASE 1 + 2: DESCOBERTA RECURSIVA DOS SUBMODULES
# ==========================================================

process_repository() {
  local REPO_ROOT="$1"

  local GITMODULES_FILE
  local ENTRY
  local KEY
  local SUB_PATH
  local NAME
  local CHILD_REPO
  local -a SUBMODULE_ENTRIES=()

  GITMODULES_FILE="$REPO_ROOT/.gitmodules"

  if [ ! -f "$GITMODULES_FILE" ]; then
    return 0
  fi

  echo ""
  echo "=========================================================="
  echo "📦 Repository: $REPO_ROOT"
  echo "📄 .gitmodules: $GITMODULES_FILE"
  echo "=========================================================="

  echo "📦 Syncing git submodule URLs..."
  git -C "$REPO_ROOT" submodule sync

  # .ai SEMPRE primeiro (ver submodule_entries em lib/git-root.sh)
  mapfile -t SUBMODULE_ENTRIES < <(submodule_entries "$REPO_ROOT")

  echo "🔎 Found ${#SUBMODULE_ENTRIES[@]} submodule(s)"

  for ENTRY in "${SUBMODULE_ENTRIES[@]}"; do

    KEY="${ENTRY%% *}"
    SUB_PATH="${ENTRY#* }"

    NAME="${KEY#submodule.}"
    NAME="${NAME%.path}"

    # ------------------------------------------------------
    # PROTEÇÃO: alterações locais → pergunta antes de tocar
    # ------------------------------------------------------
    if ! check_local_changes "$REPO_ROOT" "$SUB_PATH"; then
      continue
    fi

    # ------------------------------------------------------
    # Inicializa SOMENTE este submodule (não o repo inteiro)
    # ------------------------------------------------------
    git -C "$REPO_ROOT" submodule update --init -- "$SUB_PATH"

    process_submodule \
      "$REPO_ROOT" \
      "$NAME" \
      "$SUB_PATH"

    CHILD_REPO="$REPO_ROOT/$SUB_PATH"

    # Não desce em submodules ignorados
    if [ -n "${SKIPPED[$CHILD_REPO]:-}" ]; then
      continue
    fi

    if [ -f "$CHILD_REPO/.gitmodules" ]; then
      process_repository "$CHILD_REPO"
    fi

  done
}

# ==========================================================
# FASE 4: ATUALIZAÇÃO RECURSIVA (respeitando os ignorados)
# ==========================================================

update_working_trees() {
  local REPO_ROOT="$1"
  local ENTRY
  local SUB_PATH
  local CHILD_REPO
  local -a SUBMODULE_ENTRIES=()

  if [ ! -f "$REPO_ROOT/.gitmodules" ]; then
    return 0
  fi

  # .ai SEMPRE primeiro (ver submodule_entries em lib/git-root.sh)
  mapfile -t SUBMODULE_ENTRIES < <(submodule_entries "$REPO_ROOT")

  for ENTRY in "${SUBMODULE_ENTRIES[@]}"; do

    SUB_PATH="${ENTRY#* }"
    CHILD_REPO="$REPO_ROOT/$SUB_PATH"

    if [ -n "${SKIPPED[$CHILD_REPO]:-}" ]; then
      echo "⏭️  [$SUB_PATH] Working tree untouched"
      continue
    fi

    # Submodules diretos da raiz já foram posicionados em process_submodule
    if [ "$REPO_ROOT" != "$PROJECT_ROOT" ]; then
      git -C "$REPO_ROOT" submodule update --init -- "$SUB_PATH"
    fi

    update_working_trees "$CHILD_REPO"

  done
}

# ==========================================================
# INICIAR A PARTIR DO PROJETO RAIZ
# ==========================================================

echo ""
echo "🔎 Searching submodules from project root only..."

if [ ! -f "$PROJECT_ROOT/.gitmodules" ]; then
  echo "ℹ️ No submodules found in project root"
  echo "✅ Bootstrap completed successfully"
  exit 0
fi

process_repository "$PROJECT_ROOT"

# ==========================================================
# FASE 3: COMMIT + PUSH DOS SUBMODULES PAIS ALTERADOS
# ==========================================================

if [ "${#CHANGED_PATHS[@]}" -eq 0 ]; then

  echo ""
  echo "ℹ️ No nested submodule pointer needs to be updated"

else

  if [ "$DO_PUSH" = "1" ]; then

    echo ""
    echo "📝 Updating parent submodules..."

    declare -A REPOSITORIES=()

    for ENTRY in "${CHANGED_PATHS[@]}"; do
      REPO_ROOT="${ENTRY%%:*}"
      REPOSITORIES["$REPO_ROOT"]=1
    done

    for REPO_ROOT in "${!REPOSITORIES[@]}"; do

      assert_not_root "$REPO_ROOT" "commit/push"

      echo ""
      echo "📦 Processing parent: $REPO_ROOT"

      REPO_CHANGES=()
      for ENTRY in "${CHANGED_PATHS[@]}"; do
        if [ "${ENTRY%%:*}" = "$REPO_ROOT" ]; then
          REPO_CHANGES+=("${ENTRY#*:}")
        fi
      done

      # Branch de destino do push também segue a regra, a partir do origin
      RC=0
      resolve_remote_branch "$REPO_ROOT" || RC=$?

      if [ "$RC" -ne 0 ]; then
        echo "⚠️ Could not resolve branch on origin. Skipping push for $REPO_ROOT"
        continue
      fi

      PARENT_BRANCH="$REMOTE_BRANCH"

      if [ "${#REPO_CHANGES[@]}" -eq 1 ]; then
        TITLE="chore(deps): update submodule ${REPO_CHANGES[0]}"
      else
        TITLE="chore(deps): update ${#REPO_CHANGES[@]} submodules"
      fi

      if git -C "$REPO_ROOT" diff --cached --quiet -- \
        "${REPO_CHANGES[@]}"; then

        echo "ℹ️ Local HEAD already has these references"

      else

        BODY="$(
          printf '%s\n' "${CHANGE_NOTES[@]}" |
          grep -E '^- ' || true
        )"

        git -C "$REPO_ROOT" \
          commit \
          -m "$TITLE" \
          -m "$BODY" \
          -- "${REPO_CHANGES[@]}"

      fi

      echo "⬆️  Pushing $REPO_ROOT to origin/$PARENT_BRANCH..."

      git -C "$REPO_ROOT" \
        push origin "HEAD:refs/heads/$PARENT_BRANCH"

    done

  else

    echo ""
    echo "ℹ️ Nested pointers staged locally in their parent submodules."
    echo "   Run with --push to commit and push."

  fi

fi

# ==========================================================
# FASE 4: ATUALIZAÇÃO DAS WORKING TREES
# ==========================================================

echo ""
echo "🔁 Updating working trees recursively..."

update_working_trees "$PROJECT_ROOT"

# ==========================================================
# VERIFICAÇÃO: O REPOSITÓRIO RAIZ NÃO PODE TER MUDADO
# ==========================================================

ROOT_AFTER="$(root_snapshot)"

if [ "$ROOT_BEFORE" != "$ROOT_AFTER" ]; then
  echo ""
  echo "❌ The root repository state changed unexpectedly (HEAD or index)."
  echo "   Check with: git -C \"$PROJECT_ROOT\" status"
  exit 1
fi

echo ""
echo "🔒 Root repository verified: HEAD and index unchanged"

# ==========================================================
# RESUMO
# ==========================================================

if [ "${#SKIPPED[@]}" -gt 0 ]; then
  echo ""
  echo "⏭️  Skipped submodules (not updated):"
  for ABS in "${!SKIPPED[@]}"; do
    echo "   - ${ABS#"$PROJECT_ROOT"/}"
  done
fi

echo ""
echo "✅ Bootstrap completed successfully"