# Dotan Books REST API

## Project Overview
This project is a REST API built with ASP.NET Core on .NET 9 using C#.

The system follows REST principles, with clear layer separation, end-to-end asynchronous programming, and a design focused on scalability and long-term maintainability.

## Architecture
The project is structured into three layers:

1. **Application Layer** – API layer (Controllers and Middleware).
2. **Service Layer** – Business logic layer.
3. **Repository Layer** – Data access layer.

Communication between layers is implemented using **Dependency Injection (DI)** to achieve **decoupling**, improve testability, and support future extensibility.

## Asynchronous Programming and Performance
All access to business logic and data is handled **asynchronously** (`async/await`) to free server threads, improve response times under load, and increase scalability.

## Database and ORM
Database access is implemented using **Entity Framework Core** (ORM) with a **Code First** approach.

All database operations are asynchronous, including CRUD and migrations, in line with modern web server performance best practices.

## DTO Layer and Mapping
The system includes a dedicated **DTO** layer designed to:
- Prevent circular dependencies between layers.
- Separate Domain/Entity models from the API contract.
- Maintain clear architectural boundaries.

DTOs are implemented as **records**, which are well-suited for data transfer scenarios.

Mapping between Entities and DTOs (both directions) is handled by **AutoMapper**.

## Configuration Management
All configurations are stored outside the codebase in:
- `appsettings.json`
- `appsettings.Development.json`

This approach enables clean environment management, better maintainability, and proper separation between configuration and business code.

## Logging and Error Handling
The project uses **NLog** extensively for monitoring, troubleshooting, and observability.

Errors are handled centrally through an **Error Handling Middleware** to ensure:
- Consistent API error responses.
- Systematic error logging.
- Improved production monitoring and incident analysis.

In addition, all incoming traffic is stored in the **RATING** table for traffic monitoring purposes.

## Testing
The project includes automated tests in two main categories:
- **Unit Tests** – Isolated business logic tests.
- **Integration Tests** – Cross-layer/component interaction tests.

This testing strategy helps maintain high code quality and prevent regressions.

## Technology Stack
- ASP.NET Core (.NET 9)
- C#
- Entity Framework Core (Code First)
- AutoMapper
- NLog
- xUnit (Unit and Integration Tests)

## Docker Quick Start

The repository now includes containerization for the current monolith:
- [Dockerfile](Dockerfile)
- [docker-compose.yml](docker-compose.yml)
- [.env.example](.env.example)

### 1. Prepare environment values

Create a new file named `.env` at the solution root and copy values from `.env.example`.

Required variables:
- `DB_SA_PASSWORD`
- `REDIS_PASSWORD`
- `JWT_KEY`
- `EMAIL_SENDER_ADDRESS`
- `EMAIL_SENDER_PASSWORD`

### 2. Build and run

From the solution root:

```bash
docker compose up --build
```

Services:
- API: `http://localhost:8080`
- SQL Server: `localhost:1433`
- Redis: `localhost:6379`

### 3. Notes for container runtime

- Database migrations run automatically on API startup.
- Startup retries are enabled to handle SQL Server warm-up.
- Redis runs with password protection (`requirepass`) and receives its password from `.env`.
- API Redis connection settings are provided through environment variables in `docker-compose.yml`.
- HTTPS redirection is disabled in Docker via environment variable (`EnableHttpsRedirection=false`).
- CORS origins can be controlled with environment variables, for example:
	- `Cors__AllowedOrigins__0=http://localhost:4200`

### 4. Verify Redis cache behavior

1. Start the stack with Docker Desktop open:

```bash
docker compose up --build
```

2. Confirm running containers in Docker Desktop:
- `dotanbooks-api`
- `dotanbooks-sqlserver`
- `dotanbooks-redis`

3. Trigger cached GET endpoints (for example authors/categories/promotions) twice and verify application behavior.

4. Enter the Redis container and inspect keys/TTL:

```bash
docker exec -it dotanbooks-redis redis-cli -a "$REDIS_PASSWORD"
```

PowerShell (Windows):

```powershell
docker exec -it dotanbooks-redis redis-cli -a "$env:REDIS_PASSWORD"
```

Inside `redis-cli`:

```text
KEYS *
TTL authors:all
GET authors:all
TTL categories:all
TTL promotions:all
```

5. Wait until TTL expires and call the same endpoint again. You should see key recreation after the request.

### 5. Stop and clean

```bash
docker compose down
```

To also remove the SQL data volume:

```bash
docker compose down -v
```


## Kafka Integration Guide

This section explains how to run Kafka locally, start the API and consumer, and verify that order events are produced and consumed.

### 1. Prerequisites

- **Docker Desktop** (Kafka, ZooKeeper, Kafka UI)
- **.NET 9 SDK** (or higher)

### 2. Start Kafka Infrastructure

From the solution root, run:

```bash
docker compose up -d
```

This starts the Kafka-related services defined in `docker-compose.yml`.

### 3. Open Kafka UI

- URL: `http://localhost:8081`
- Cluster name: `local`
- Go to **Topics** and open `OrderCreatedTopic` to inspect messages.

### 4. Run the API

In a separate terminal:

```bash
cd DotanBooks
dotnet run
```

Swagger URL:

- `http://localhost:5180/swagger`

### 5. Run the Kafka Consumer

In another terminal:

```bash
cd OrderLoggerConsumer
dotnet run
```

Recommended consumer setting:

- `AutoOffsetReset = AutoOffsetReset.Earliest` (useful for reading historical messages).

### 6. Verification Flow

1. Open Swagger and click **Authorize**.
2. Enter token in format: `Bearer <your-jwt-token>`.
3. Send `POST /api/Orders/checkout` with valid `userId` and order details.
4. Check the consumer terminal for consumed-event logs.
5. Refresh Kafka UI (`OrderCreatedTopic`) and verify new records appear.

### 7. Troubleshooting

- **Consumer cannot connect to Kafka**: verify `BootstrapServers` is `localhost:9092`.
- **No messages consumed**: use a unique `GroupId` or reset offsets.
- **No data in Kafka UI**: confirm producer code publishes to `OrderCreatedTopic` and all Docker containers are running.
