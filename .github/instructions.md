# DotanBooks Agent Instructions

## System summary
DotanBooks is a layered ASP.NET Core REST API for an online bookstore. The API handles users, books, categories, promotions, orders, and traffic monitoring (RATING table).

## Tech stack
- .NET 9, C#
- ASP.NET Core Web API
- EF Core (Code First, SQL Server)
- AutoMapper
- JWT auth
- NLog
- xUnit + Moq + FluentAssertions

## Project map
- DotanBooks/: API layer (controllers, middleware, Program.cs)
- Service/: business logic layer
- Repository/: data access layer, StoreContext, EF migrations
- Entitiys/: domain entities (folder name is intentionally misspelled)
- DTOs/: API contracts + MappingProfiles
- Utils/: shared exceptions/utilities
- DotanBooks.Tests/: Unit + Integration tests

## Architecture contract
- Required flow: Controller -> Service -> Repository -> StoreContext.
- Keep controllers thin and DTO-based.
- Keep business rules in Service.
- Keep data access in Repository only.
- Implement and preserve async end-to-end.
- Register every new interface/implementation in Program.cs.

## Agent workflow (default)
1. Inspect existing layer files first; follow existing patterns before adding new code.
2. Implement minimal scoped changes in the correct layer.
3. Update DI registration and AutoMapper profile when needed.
4. For schema changes, create EF migration with explicit project/startup flags.
5. Validate with targeted tests first, then full `dotnet test DotanBooks.sln`.

## Build and run commands (PowerShell)
Run from solution root:
- Build: `dotnet build DotanBooks.sln`
- Tests: `dotnet test DotanBooks.sln`
- Run API: `dotnet run --project DotanBooks/DotanBooks.csproj`

## EF migration commands
- Add migration:
  `dotnet ef migrations add <Name> --project Repository/Repository.csproj --startup-project DotanBooks/DotanBooks.csproj --output-dir Migrations`
- Update database:
  `dotnet ef database update --project Repository/Repository.csproj --startup-project DotanBooks/DotanBooks.csproj`

## Reliability notes
- Integration tests use a real SQL Server (not in-memory).
- If tests cannot connect, set `DOTANBOOKS_TEST_CONNECTION`.
- Common fallback instances are LocalDB and SQLEXPRESS.
- Avoid editing generated migration snapshots unless migration generation requires it.

## Security hygiene
- Never add secrets to source-controlled files.
- Do not duplicate sensitive SMTP-style settings in code/config.
- Prefer environment variables or secret stores for credentials.

## Layer-specific playbooks
- Controller guidance: `.github/controller-instructions.md`
- Service guidance: `.github/service-instructions.md`
- Repository guidance: `.github/repository-instructions.md`
