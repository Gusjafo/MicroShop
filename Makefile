# Makefile (Linux/macOS; en Windows usar PowerShell script)
COMPOSE = docker compose -f deploy/compose/docker-compose.yml --env-file .env

infra-up:
	$(COMPOSE) up -d

infra-down:
	$(COMPOSE) down -v

infra-logs:
	$(COMPOSE) logs -f

infra-ps:
	$(COMPOSE) ps
