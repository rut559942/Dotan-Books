# Service Instructions

## Purpose
Service layer owns business logic, validations, policies, and workflow orchestration across repositories.

## Required practices
- Keep service methods async end-to-end.
- Enforce business invariants and throw domain exceptions from `Utils/Exceptions`.
- Depend on repository interfaces, not concrete implementations.
- Use AutoMapper for Entity/DTO transformation where configured.

## Do
- Validate input semantics (not only shape).
- Keep methods focused and deterministic where possible.
- Use repository abstractions for persistence and retrieval.
- Coordinate cross-repository scenarios inside service methods.

## Don’t
- Don’t use DbContext directly in services.
- Don’t return Entities to controllers.
- Don’t swallow exceptions silently.

## Change checklist
1. Add/extend service interface contract.
2. Implement business logic in service class.
3. Reuse existing exception types for API-consistent errors.
4. Update Program.cs DI registration if a new service is added.
5. Add/adjust unit tests in `DotanBooks.Tests/Unit/Service`.
