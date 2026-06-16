# Repository Instructions

## Purpose
Repository layer encapsulates EF Core data access and query/update operations.

## Required practices
- Use `StoreContext` only in Repository layer.
- Use async EF methods (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, etc.).
- Keep repositories focused on persistence concerns, not business policy.
- Keep query intent explicit and predictable.

## Do
- Add interface + implementation for new repository capabilities.
- Keep transactions/SaveChanges behavior consistent with existing patterns.
- Add indexes/constraints via EF model configuration and migrations when needed.
- For schema changes, generate migration with explicit project/startup arguments.

## Don’t
- Don’t add business validations here (move to Service).
- Don’t call blocking sync EF APIs in request flow.
- Don’t hand-edit generated migration snapshot unless unavoidable.

## Migration and DB checklist
1. Update Entity and `StoreContext` mapping.
2. Generate migration:
   `dotnet ef migrations add <Name> --project Repository/Repository.csproj --startup-project DotanBooks/DotanBooks.csproj --output-dir Migrations`
3. Apply migration (local/dev):
   `dotnet ef database update --project Repository/Repository.csproj --startup-project DotanBooks/DotanBooks.csproj`
4. Run targeted integration tests and then full test suite.

## Testing notes
- Integration tests in this repo use a real SQL Server fixture.
- If connection fails, set `DOTANBOOKS_TEST_CONNECTION`.
