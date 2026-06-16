# Controller Instructions

## Purpose
Controllers define HTTP contracts and orchestration only. They should not contain business logic or direct data access.

## Required practices
- Accept and return DTOs only (never expose Entities directly).
- Keep methods thin: validate request shape, call Service, map response codes.
- Use async signatures (`Task<IActionResult>`, `Task<ActionResult<T>>`).
- Rely on centralized exception middleware for domain exceptions.
- Keep route naming consistent with existing API style.

## Do
- Use `[ApiController]` and clear `[Route]` patterns.
- Return meaningful status codes (`200/201/204/400/404/409/422/403`) per service outcome.
- Use `[FromBody]`, `[FromQuery]`, `[FromRoute]` explicitly when useful.

## Don’t
- Don’t query DbContext/Repository directly from controller.
- Don’t perform business rule checks in controller.
- Don’t map Entity -> DTO manually if AutoMapper profile already exists.

## Change checklist
1. Add/adjust endpoint method.
2. Ensure Service interface includes required method.
3. Ensure DTO contract exists/updated.
4. Verify response status and payload contract.
5. Add/adjust tests if endpoint behavior changes.
