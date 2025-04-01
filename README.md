# Opah

Opah is a cash flow control system developed on a microservices architecture to simulate the concepts of a distributed topology.

### Components
- [Transaction System](./docs/Transaction.md)
- [Consolidation System](./docs/Consolidation.md)
- [Backend for Frontend](./docs/BFF.md)

## Technologies Used
- .NET 9 - Backend application framework
- PostgreSQL - Relational database
- Redis - In-memory data store for caching and message streaming
- Docker Compose - Container orchestration for local development

## Prerequisites

Ensure you have the following installed on your system:
- [Docker & Docker Compose](https://docs.docker.com/)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Running the Application Locally

Clone the repository:
```shell
git clone https://github.com/jlfjunior/opah.git
cd opah
```
Start the infrastructure and application:
```shell
docker-compose up -d
```
Restore dependencies:
```shell
dotnet restore
```
Build the project:
```shell
dotnet build
```

Apply database migrations (if using Entity Framework Core):
```shell
dotnet ef database update
```

Run the applications:

```shell
cd Opah.Consolidation.API/
dotnet run
```
or 
```shell
cd Opah.Transacion.API/
dotnet run
```
or
```shell
cd Opah.Consolidation.Worker/
dotnet run
```
- The **Consolidation API** application should be available at http://localhost:5287.
- The **Transaction API** application should be available at http://localhost:5116.

## Running Tests

To execute tests, run:
```shell
 dotnet test
```

## Architectural Decisions

For details on the architectural decisions made for this project, please refer to the ADR (Architectural Decision Records) documentation. You can find the records [here](./docs/ADR.md). These records outline key choices regarding:

- Framework and libraries selection
- Database and ORM decisions
- API design patterns
- Scalability and performance considerations
- Security best practices