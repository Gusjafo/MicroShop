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

## Stop
```bash
scripts/dev.sh down
```

## Logs
```bash
scripts/dev.sh logs
```
