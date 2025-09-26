Conventions
===========

Naming & Structure
------------------

*   Root namespace: MicroShop.
    
*   Services: MicroShop.Services.{Service}.{Layer} with layers: Domain, Application, Infrastructure, API.
    
*   Building blocks packages: MicroShop.BuildingBlocks.{AbstractionName}.
    
*   Message contracts: PascalCase + dot-separated version suffix, e.g., Catalog.ProductPriceChanged.V1.
    

Clean Architecture (per service)
--------------------------------

*   Domain: Aggregates/entities, domain events, value objects, interfaces.
    
*   Application: Use cases (command/query handlers), DTOs, validators, unit of work abstractions.
    
*   Infrastructure: EF Core DbContext, repositories, outbox/inbox, message bus, external clients.
    
*   API: Minimal APIs/Controllers, DI, pipelines, health checks.
    

Messaging
---------

*   Broker: RabbitMQ, events published outbox-first.
    
*   Contracts live in a shared Contracts package (no domain logic).
    
*   Versioning via message name (.V1) and metadata headers (message-version).
    

Configuration
-------------

*   appsettings.json + appsettings.{Environment}.json + env vars.
    
*   DOTNET\_ENVIRONMENT controls behavior; secrets via dotnet user-secrets in dev.
    

Telemetry
---------

*   OpenTelemetry: resource attributes include service.name=microshop-{service}, service.version.
    
*   Traces for HTTP and messaging; logs are structured (Serilog), metrics via Prometheus.
    

Git Workflow
------------

*   Conventional Commits: feat:, fix:, refactor:, test:, docs:, chore:.
    
*   Branches: main (stable), dev (integration), feat/\*.
    

Testing Levels
--------------

*   Unit: pure domain/application.
    
*   Integration: Testcontainers (SQL Server, RabbitMQ).
    
*   Contract: Pact (provider/consumer).
    
*   Smoke: compose up + scripted HTTP.