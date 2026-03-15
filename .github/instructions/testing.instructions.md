---
description: "Use when creating or updating backend tests for the turn-based multiplayer game server in .NET 10. Covers business-logic-focused testing, test scope, structure, and anti-patterns."
name: "Backend Testing Standards"
applyTo: "**/*Test*.cs, **/*.Tests.cs, **/*.Tests.csproj, tests/**/*.cs"
---
# Backend Testing Standards

## Purpose

- Write tests where real behavior happens: domain rules, application use cases, state transitions, and failure paths.
- Prefer tests that protect business outcomes over tests that mirror implementation details.

## What to Prioritize

- Domain invariants and business rules.
- Command/query handler behavior and orchestration logic.
- Edge cases and negative paths.
- Error handling, validation behavior, and idempotency where relevant.
- Mapping from input to observable outcomes (state changes, returned results, published events).

## What to Avoid

- Do not write tests that only verify constructors assign fields.
- Do not write tests that only verify dependency injection wiring.
- Do not write tests for trivial getters/setters or pass-through wrappers without logic.
- Do not over-mock to the point that tests only validate call counts instead of behavior.

## Test Design Rules

- Use clear Arrange-Act-Assert structure.
- Keep each test focused on one behavior.
- Use intention-revealing test names in the form `MethodOrBehavior_ShouldExpectedOutcome_WhenCondition`.
- Assert externally observable outcomes (returned value, persisted state intent, emitted domain event, error result).
- Prefer deterministic test data; avoid randomness unless explicitly seeded.

## Test Levels

### Domain/Application Unit Tests

- Default choice for fast feedback on business logic.
- Mock only boundaries (repositories, gateways, external APIs), not core behavior.

### Integration Tests

- Use for ORM mappings, repository behavior, transaction boundaries, and critical end-to-end flows.
- Test realistic persistence behavior instead of only mocking EF Core interactions.

## Async and Reliability

- Use async test methods for async code paths.
- Always await tasks; avoid fire-and-forget in tests.
- Ensure tests are isolated and can run in parallel unless a shared-resource constraint is documented.

## Minimal Quality Bar

- New business logic should include meaningful tests for success path and at least one failure or edge path.
- Bug fixes should include a regression test that fails before the fix and passes after it.

## Framework Guidance

- Use the existing project test framework and conventions.
- If no framework exists yet for backend tests, prefer `xUnit` as default.
