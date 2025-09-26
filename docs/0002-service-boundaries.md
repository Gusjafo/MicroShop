ADR 0002: Service Boundaries & Data Ownership
=============================================

Context
-------

We must prevent tight coupling and define ownership to enable independent deployability.

Decision
--------

*   **Identity** owns users/roles/claims; issues JWTs. No PII outside tokens.
    
*   **Catalog** owns products, prices, and inventory snapshots; publishes stock/price events.
    
*   **Ordering** owns carts/orders and orchestrates payments & inventory via saga; inbox/outbox for reliability.
    
*   **Payments** owns PSP simulation (authorize/capture/refund).
    
*   **Shipping** owns shipment lifecycle; reacts to paid orders.
    

Each service has its own database schema and publishes events through RabbitMQ. Cross-service queries are forbidden; use events to build local read models where needed.

Consequences
------------

*   Loose coupling and autonomy.
    
*   Requires eventual consistency thinking and clear event versioning.
    
*   Schema evolution handled per service with migrations and contracts evolution.