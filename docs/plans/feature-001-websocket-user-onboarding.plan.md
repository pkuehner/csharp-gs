# Feature Plan 001: WebSocket User Onboarding

## Goal

Build the first backend feature in `.NET 10`: a WebSocket API that resolves a player identity and persists users in SQLite.

## Locked Product Decisions

- If client provides a UUID that does not exist, reject the connection request.
- On first successful anonymous connect, always assign a random server-generated name.

## MVP Scope

- Expose WebSocket endpoint `GET /ws`.
- Accept initial `connect` message from client.
- Support two connection modes:
- Anonymous connect: no `userId` provided.
- Known user connect: `userId` provided.
- Persist users in SQLite via EF Core.
- User model includes `Id` (UUID) and `Name`.
- Return `connected` response over socket with resolved identity.

## Architecture (DDD + DI)

### `GameServer.Domain`

- `User` aggregate root, invariants for identity/name.
- Domain errors for invalid states.

### `GameServer.Application`

- `ConnectUser` use case/handler.
- DTOs for inbound connect request and outbound connected response.
- Abstractions: `IUserRepository`, `IUserNameGenerator`, `IClock` (optional).

### `GameServer.Infrastructure`

- `GameServerDbContext` using EF Core + SQLite provider.
- `UserRepository` implementation.
- Migrations and persistence configuration.

### `GameServer.Api`

- WebSocket endpoint and message loop.
- Thin transport layer: parse message, call application handler, send response.
- DI composition root.

## WebSocket Contract (MVP)

### Client -> Server

```json
{ "type": "connect", "userId": "optional-uuid" }
```

Notes:
- First connect can be anonymous (omit `userId`).
- Name is not accepted from client in MVP; server assigns random name for first-time user.

### Server -> Client (Success)

```json
{ "type": "connected", "userId": "uuid", "name": "random-name", "isNewUser": true }
```

or

```json
{ "type": "connected", "userId": "uuid", "name": "existing-name", "isNewUser": false }
```

### Server -> Client (Failure)

```json
{ "type": "error", "code": "USER_NOT_FOUND", "message": "User ID does not exist." }
```

Other failure codes to define during implementation:
- `INVALID_MESSAGE`
- `INVALID_USER_ID`
- `INTERNAL_ERROR`

## Data Model (SQLite)

Table: `Users`
- `Id` TEXT PRIMARY KEY (UUID string)
- `Name` TEXT NOT NULL
- `CreatedAtUtc` TEXT NOT NULL

Constraints:
- `Id` unique and immutable.
- `Name` required and non-empty.

## Delivery Milestones

1. Solution Scaffolding

- Create solution and projects:
- `src/GameServer.Api`
- `src/GameServer.Application`
- `src/GameServer.Domain`
- `src/GameServer.Infrastructure`
- `tests/GameServer.Domain.Tests`
- `tests/GameServer.Application.Tests`
- `tests/GameServer.Infrastructure.IntegrationTests`
- Wire project references according to layer boundaries.

2. Domain Implementation

- Implement `User` aggregate and invariant checks.
- Add value object or helper for strongly-typed user identifier if beneficial.

3. Application Use Case

- Implement `ConnectUser` handler with flows:
- Anonymous connect -> create user with generated UUID + random name.
- Provided UUID exists -> load user.
- Provided UUID missing -> return `USER_NOT_FOUND`.
- Implement clean result type for success/failure mapping.

4. Infrastructure Persistence

- Add EF Core DbContext and entity configuration.
- Implement repository methods:
- `GetByIdAsync`
- `AddAsync`
- `SaveChangesAsync`
- Add first migration for `Users` table.

5. API WebSocket Endpoint

- Enable WebSockets in ASP.NET Core.
- Add `/ws` endpoint.
- Parse first message and dispatch to handler.
- Return structured success/error JSON responses.

6. Logging and Observability

- Add logs for connection opened/closed.
- Log connect decision path (anonymous/new, known/existing, known/missing).
- Log failures with correlation/user context where available.

7. Test Implementation (Logic-Focused)

- Domain tests for user invariants and name validity.
- Application tests for all connect decision branches.
- Integration tests for SQLite persistence behavior.
- WebSocket integration test for end-to-end connect handshake.

8. Hardening Pass

- Validate payload size and malformed JSON handling.
- Add cancellation token flow and clean socket shutdown handling.
- Review error contract consistency.

## Testing Strategy (Must-Haves)

- Test business logic and behavior outcomes.
- Do not add tests that only verify constructor assignment or DI wiring.
- Ensure new logic has success + edge/failure coverage.
- Add regression tests for every bug fix.

## Definition of Done

- Anonymous client can connect and receive generated UUID and random name.
- Generated user is persisted in SQLite.
- Existing UUID reconnect resolves same user identity.
- Unknown UUID is rejected with `USER_NOT_FOUND`.
- Migration exists and DB can be initialized cleanly.
- Logic-focused tests pass.

## Implementation Order Recommendation

1. Milestone 1-2
2. Milestone 3
3. Milestone 4
4. Milestone 5
5. Milestone 7
6. Milestone 6 and 8
