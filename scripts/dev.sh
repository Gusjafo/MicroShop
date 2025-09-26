# scripts/dev.sh
#!/usr/bin/env bash
set -euo pipefail
COMPOSE="docker compose -f deploy/compose/docker-compose.yml --env-file .env"
case "${1:-}" in
  up)     $COMPOSE up -d ;;
  down)   $COMPOSE down -v ;;
  logs)   $COMPOSE logs -f ;;
  ps)     $COMPOSE ps ;;
  *) echo "usage: scripts/dev.sh [up|down|logs|ps]"; exit 1 ;;
esac
