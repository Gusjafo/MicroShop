// README.md
# MicroShop (.NET 9) — Order-to-Cash Microservices

A production-grade learning case: Identity, Catalog, Ordering, Payments, Shipping; YARP gateway; RabbitMQ; SQL Server; Observability (OpenTelemetry + Prometheus/Grafana + Jaeger).

## Prerequisites
- .NET SDK 9
- Docker Desktop (Linux containers)
- Git

## Quick Start
```bash
dotnet --info
dotnet build
```

## Run
```bash
scripts/dev.sh up
```

### Identity service configuration

The Identity API requires a SQL Server database, RabbitMQ, and an RSA key pair for signing JWTs. The provided `deploy/compose/docker-compose.yml` file spins up all dependencies together with the API. Before running the stack make sure the following environment variables are available (for example in a `.env` file beside the compose file):

| Variable | Description |
| --- | --- |
| `MSSQL_SA_PASSWORD` | Strong password for the SQL Server `sa` user (e.g. `Str0ngP@ssw0rd!`). |
| `IDENTITY_JWT_PRIVATE_KEY` | RSA private key in PEM format used to sign access tokens. |
| `IDENTITY_JWT_PUBLIC_KEY` | Matching RSA public key in PEM format used for validation. |
| `IDENTITY_JWT_ISSUER` | (Optional) JWT issuer claim. Defaults to `microshop-identity`. |
| `IDENTITY_JWT_AUDIENCE` | (Optional) JWT audience claim. Defaults to `microshop-clients`. |

You can generate a development RSA key pair with OpenSSL:

```bash
openssl genrsa -out identity-private.pem 2048
openssl rsa -in identity-private.pem -pubout -out identity-public.pem
```

Then copy the PEM contents into the environment variables (preserving line breaks).

Once the containers are running you can exercise the service with a simple smoke test:

```bash
curl -X POST http://localhost:8080/register \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","email":"demo@example.com","password":"P@ssw0rd!"}'

curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"demo@example.com","password":"P@ssw0rd!"}'
```

The `/users` endpoints require a valid bearer token.

## Stop
```bash
scripts/dev.sh down
```

## Logs
```bash
scripts/dev.sh logs
```

## Troubleshooting

- [Troubleshooting NuGet cache cleanup errors on Windows](docs/0003-troubleshooting-nuget-cache.md)
