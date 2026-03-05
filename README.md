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
