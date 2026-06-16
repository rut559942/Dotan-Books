---
description: Generate tests for a specific target in the DotanBooks repository.
---

# Test Writing Prompt — DotanBooks

> Write tests for a specific part of the DotanBooks codebase.
>
> The user will provide this prompt plus a target area such as a controller, service, repository, or feature folder.

## Core Goal

Write tests that verify behavior at the correct boundary.

Prefer the smallest test scope that still proves the requirement.
Do not write production code unless the user explicitly asks for it.

## Test Layer Selection

Choose the test type based on the target area:

| Target area | Preferred test type |
|-------------|---------------------|
| `Controllers/` | Controller tests focused on HTTP behavior, request/response shape, status codes, and routing |
| `Service/` | Unit tests or mock-based behavior tests for business rules, orchestration, validations, and side effects |
| `Repository/` | Integration tests against the real SQL Server/EF Core persistence layer |
| `DTOs/`, `Entities/`, shared helpers | Test only if there is behavior that matters at the public boundary |

If the target crosses multiple layers, prefer the narrowest layer that validates the behavior end to end.
Add lower-level tests only when they improve clarity or prevent regressions.

## Repository Conventions

Follow the patterns already used in DotanBooks:

| Rule | Why it matters |
|------|----------------|
| Use xUnit | Matches the repository's existing test suite |
| Use Moq | Keeps unit and behavior tests isolated |
| Use AAA structure | Improves readability and consistency |
| Use `TestMapperFactory` | Reuses the repository's AutoMapper setup |
| Use `SqlServerTestDatabaseFixture` | Enables real repository integration tests |
| Match naming patterns like `Method_WhenCondition_ExpectedResult` | Keeps test names predictable |
| Keep assertions deterministic | Focus on observable behavior, not implementation detail |

## Architecture Rules to Respect

| Rule | Expectation |
|------|-------------|
| Controllers stay thin | Use DTOs only and keep orchestration out of controllers |
| No direct data access from controllers | Do not access repositories or DbContext directly |
| Services own business logic | Put rules, validation, and orchestration in services |
| Repositories get integration tests | Cover persistence behavior when SQL/EF behavior matters |
| Avoid private-detail tests | Prefer the public behavior boundary whenever possible |

## What to Produce

Return only the test code or the smallest useful set of test files.

If multiple test files are needed, group them by the target layer and keep each file focused on one responsibility.

When the target is ambiguous or missing, ask for the exact path or symbol before generating tests.

## Suggested Workflow

1. Inspect the target area and determine the test layer.
2. Identify the public behavior that should be verified.
3. Reuse the repo's test infrastructure where appropriate.
4. Write tests that cover success paths, failure paths, and side effects when relevant.
5. Keep the tests readable, minimal, and consistent with existing patterns.

## Do Not

- Do not invent frameworks or test infrastructure that does not exist in this repo
- Do not add production features while writing tests
- Do not write brittle tests tied to implementation details unless the behavior cannot be verified another way
- Do not create unnecessary test layers for a simple target