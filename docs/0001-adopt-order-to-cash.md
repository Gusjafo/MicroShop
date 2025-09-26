ADR 0001: Adopt Order-to-Cash Domain
====================================

Context
-------

We need a realistic domain to demonstrate microservice patterns: sagas, outbox/inbox, idempotency, retries, observability, and secure APIs.

Decision
--------

Adopt an e-commerce **Order-to-Cash** flow with services: Identity, Catalog, Ordering, Payments, Shipping, and a YARP API Gateway. Shared infra: SQL Server, RabbitMQ, OTel stack.

Consequences
------------

*   Clear saga orchestration (Ordering ↔ Payments/Inventory/Shipping).
    
*   Event-driven integration and eventual consistency.
    
*   Broad coverage of reliability and security patterns.
    
*   Familiar terminology for readers and contributors.