# ─── Ambiente ────────────────────────────────────────────────────────────────

up:           ## Sobe o container de dev com hot reload
	docker compose up -d --build
	@echo "✅ App rodando em http://localhost:5000"

down:         ## Para os containers
	docker compose down

logs:         ## Acompanha os logs
	docker compose logs -f app

shell:        ## Abre shell dentro do container
	docker exec -it dotnet-dev bash

# ─── .NET ─────────────────────────────────────────────────────────────────────

restore:      ## Restaura pacotes NuGet
	docker exec dotnet-dev dotnet restore /workspace/src/WebApi/WebApi.csproj

build:        ## Build sem rodar
	docker exec dotnet-dev dotnet build /workspace/src/WebApi/WebApi.csproj

# ─── EF Core Migrations ───────────────────────────────────────────────────────

migration:    ## Cria migration: make migration name=NomeDaMigration
	docker exec dotnet-dev dotnet ef migrations add $(name) \
		--project /workspace/src/WebApi/WebApi.csproj

migrate:      ## Aplica migrations pendentes
	docker exec dotnet-dev dotnet ef database update \
		--project /workspace/src/WebApi/WebApi.csproj

# ─── Ferramentas opcionais ────────────────────────────────────────────────────

tools-up:     ## Sobe SQLite browser em http://localhost:8080
	docker compose --profile tools up -d sqlite-web

.PHONY: up down logs shell restore build migration migrate tools-up
