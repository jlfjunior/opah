# Architecture Decision Records


# ADR-001: Choice of Database

## Context
To store the application's data, we need to choose a relational database that offers:
- High reliability and data integrity.
- Support for complex transactions and concurrency.
- Good scalability and compatibility with analysis and replication tools.

## Decision
We chose **PostgreSQL** as the main database due to the following reasons:
- Advanced support for SQL and extensions such as JSONB and PostGIS.
- Excellent compliance with ACID standards and high performance in complex queries.
- Active community, extensive documentation, and robust support.
- Ease of integration with monitoring and data analysis tools.

## Consequences
- **Positive**: Security, reliability, and support for application growth.
- **Negative**: May require more configuration and tuning for high performance with large data volumes.

## Status
Accepted

## References
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)


# ADR-002: Use of Redis Streams for Message Processing

## Context
To efficiently handle event-driven and real-time processing in our system, we need a robust message queue mechanism. The main requirements are:
- High throughput and low latency for message consumption.
- Ability to process messages in a distributed and scalable manner.
- Support for persistent message storage and replay capabilities.

## Decision
We decided to use **Redis Streams** for message processing due to the following reasons:
- It provides an efficient, in-memory log structure for handling streams of messages.
- It supports consumer groups, enabling parallel and distributed message processing.
- It allows message persistence and replay, ensuring fault tolerance and reliability.
- It integrates well with existing Redis-based infrastructure, reducing operational overhead.

## Consequences
- **Positive**: High-performance message handling, scalability, and fault tolerance.
- **Negative**: Requires careful management of stream retention policies to avoid excessive memory usage.

## Status
Accepted

## References
- [Redis Streams Documentation](https://redis.io/docs/data-types/streams/)

# ADR-003: Adoption of the Mapper Pattern

## Context
When handling data persistence in the application, we need a structured approach to mapping database records to domain objects. The main requirements are:
- Decoupling business logic from data access.
- Facilitating object-oriented programming principles.
- Improving testability and maintainability of the codebase.

## Decision
We decided to adopt the **Mapper Pattern** for data access due to the following reasons:
- It provides a clean separation between the domain model and the persistence layer.
- It allows easy switching between different database implementations if needed.
- It enhances code organization by encapsulating data mapping logic.
- It works well with ORMs such as SQLAlchemy, improving code maintainability.

## Consequences
- **Positive**: Better separation of concerns, easier maintenance, and improved testability.
- **Negative**: May introduce additional complexity in simple use cases.

## Status
Accepted

## References
- [Mapper Pattern - Martin Fowler](https://martinfowler.com/eaaCatalog/dataMapper.html)

# ADR-005: Use of Entity Framework Core as ORM

## Context
To manage data persistence in the application, we need an Object-Relational Mapping (ORM) framework that provides:
- Seamless integration with .NET applications.
- Support for complex queries, transactions, and database migrations.
- High productivity with minimal boilerplate code.

## Decision
We decided to use **Entity Framework Core (EF Core)** as the ORM for the project due to the following reasons:
- It is the de facto standard ORM for .NET applications, ensuring strong compatibility.
- It supports LINQ queries, making database interactions more intuitive and readable.
- It provides built-in migration management, simplifying schema evolution.
- It supports multiple database providers, allowing flexibility in database choice.

## Consequences
- **Positive**: Increased developer productivity, easier database management, and strong .NET ecosystem integration.
- **Negative**: May introduce performance overhead in highly complex queries, requiring query optimization techniques.

## Status
Accepted

## References
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)