# Copilot Instructions

You are the software engineer for this turn-based online multiplayer board game backend.

## Stack Defaults

- Backend stack is `.NET 10` unless explicitly asked otherwise.
- Prefer modern C# language features supported by the selected SDK.

## Architecture Defaults

- Follow Domain-Driven Design (DDD) with clear boundaries between domain, application, infrastructure, and API layers.
- Keep domain logic inside domain entities, value objects, and domain services.
- Keep controllers/endpoints thin and delegate orchestration to application services or handlers.
- Model aggregates explicitly and preserve invariants via aggregate roots.

## Dependency Injection Defaults

- Use dependency injection by default for services, repositories, and external integrations.
- Depend on abstractions in higher layers and wire implementations in the composition root.
- Avoid service locators and hard-coded infrastructure instantiation in domain/application logic.

## Data Access Defaults

- Use `Entity Framework Core` as the default ORM.
- Keep persistence concerns in the infrastructure layer.
- Avoid leaking EF Core-specific behavior into domain models when possible.
- Use direct SQL only when there is a clear performance or operational reason.

## Reuse and Generalization

- Prefer reusable, composable abstractions over duplicated logic.
- Generalize repeated patterns when it improves maintainability and clarity.
- Avoid premature abstraction; prefer concrete implementations until reuse is justified.

## Quality Defaults

- Write testable code with clear seams and injectable dependencies.
- Favor async I/O for database and network operations.
- Use explicit, intention-revealing names for domain concepts.
- Add focused logging for business-critical workflows and failure paths.

## Testing Defaults

- Write useful tests for business and application logic where behavior actually happens.
- Do not spend effort on tests that only validate pure constructors, dependency injection wiring, or trivial property assignment.
- Prioritize tests for domain rules, state transitions, edge cases, and failure paths.
- Keep tests intention-revealing and focused on outcomes, not implementation details.

## Rule Strictness

- These rules are strong preferences.
- If a better alternative is needed for a specific case, explain the rationale briefly in comments or PR notes.
- When multiple valid options exist, choose the one that best improves maintainability, testability, reuse, and DDD boundary clarity.
