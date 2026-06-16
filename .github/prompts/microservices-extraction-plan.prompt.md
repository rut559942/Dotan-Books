---
description: Create a bilingual microservices extraction plan for the DotanBooks monolith.
---

# Microservices Extraction Plan — DotanBooks

> Planning only. Do not implement code, create services, or write migration scripts.
>
> The goal is to help the team think through how to split the current DotanBooks monolith into microservices.

## Output Format

Produce the response as a bilingual two-column table in Markdown.

| עברית | English |
|-------|---------|

Each major section must appear in both languages side by side.

## Scope for DotanBooks

Use the current DotanBooks architecture as the source of truth.
Base the analysis on the existing controllers, services, repositories, DTOs, and entities in the repository.

### Likely domain areas

- Users, authentication, JWT, blocked users, and token management
- Books, categories, authors, and catalog browsing
- Orders and checkout flow
- Promotions and discounts
- Ratings and traffic-related behavior
- Search and autocomplete
- Management/backoffice book operations
- Supporting infrastructure such as Kafka, email, Redis, logging, and cache usage

## Required Sections

1. Bounded contexts
2. Suggested service boundaries
3. Entity and controller ownership mapping
4. Extraction phases
5. API contract strategy
6. Database and migration strategy
7. Async communication strategy
8. Testing strategy
9. Cutover and strangler pattern notes
10. Pre-merge checklist
11. Operational recommendations

## Instructions for the Model

| Guideline | What it means |
|-----------|---------------|
| Keep the document planning-only. | Do not implement code or scaffolding. |
| Prefer business boundaries. | Split by responsibility, not by file structure. |
| Explain ownership decisions. | Say why a service owns an entity or controller. |
| Mention tradeoffs. | Call out uncertain boundaries clearly. |
| Use repo-specific examples. | Refer to real controllers like UsersController and OrdersController. |
| Align with the current stack. | Keep ASP.NET Core, EF Core, AutoMapper, NLog, Redis, and Kafka in mind. |

## Suggested Bilingual Table Structure

For each topic, write the Hebrew explanation in the left column and the English explanation in the right column.

### Example rows

| עברית | English |
|-------|---------|
| שירות המשתמשים יכיל הרשמה, התחברות, JWT וחסימת משתמשים. | The User service should own registration, login, JWT, and blocked-user handling. |
| שירות ההזמנות ינהל checkout, סטטוס הזמנה ואירועי Kafka. | The Order service should manage checkout, order status, and Kafka events. |

## Checklist to Include

Use this checklist at the end of the document:

- [ ] כל שירות מוגדר לפי bounded context ברור
- [ ] ownership ל-entities ול-controllers מופיע במפורש
- [ ] contract API מתואר ברמת high level
- [ ] DB ownership לכל שירות מוסבר
- [ ] תקשורת סינכרונית וא-סינכרונית מוסברת
- [ ] טסטים ו-cutover מתוארים
- [ ] המלצות תפעוליות מופיעות בסוף

## Tone

Use a practical architecture-review tone.

Be direct, concise, and specific.
The document should feel like an internal planning artifact for a team that already owns the DotanBooks codebase.