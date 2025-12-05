# Testing Strategy

The SafeVault API includes an automated backend test suite designed to validate
core business rules, error handling, and controller behavior without requiring
a running SQL Server instance.

Testing focuses on maintaining confidence in functionality while supporting
Clean Architecture principles.

---

## Approach

The test framework uses:

- NUnit — Test runner and assertion framework
- WebApplicationFactory — Spins up a real in-memory instance of the API
- NSubstitute — Mocking repositories and Unit of Work
- HTTP calls — Controller testing through API surface
- Test environment configuration — Prevents migrations and SQL dependency

Instead of hitting a real database, tests mock:

- IAccountRepository
- IUserRepository
- ITransactionRepository
- IUnitOfWork

This ensures reliability, speed, and full isolation.

---

## What Is Being Tested

| Category | Coverage |
|---------|----------|
| Account Rules | Debit/Credit validation, insufficient funds, account closure checks |
| Transaction Rules | Amount invalid cases, manual transaction operations |
| User Access | Profile retrieval, user ID validation |
| API Behavior | HTTP status codes, response DTO shape |

These tests enforce assessment-required constraints such as:

- No posting to closed accounts
- Balance must always update from transactions
- Invalid amounts are rejected
- Transactions must persist correctly via Unit of Work

---

## Example Test Flow

1. Arrange  
   - Configure controller dependencies using mocks
2. Act  
   - Send real HTTP request to the API
3. Assert  
   - Verify response status and domain effects (e.g., Commit called)

All performed via an instance of:

```csharp
CustomWebApplicationFactory<Program>
```

---

## Why We Do Not Use SQL Server in Tests

| Reason | Benefit |
|--------|---------|
| No external services required | Faster local and CI runs |
| Prevents data dependencies between tests | Full isolation |
| Avoids migration failures during testing | Only production runs migrations |
| Focus on business + API behavior | Clean Architecture integrity |

This follows the testing golden rule:

> “Unit and functional tests should not fail due to infrastructure.”

---

## What Is Not Tested (Yet)

| Area | Rationale |
|------|----------|
| Authentication flows | Stubbed authentication for core functionality testing |
| UI integration | Covered separately in the SafeVault UI repository |
| Full DB lifecycle | Validated in Docker deployment setup |

---

## Expanding the Suite

Future improvements can include:

- Integration tests backed by Testcontainers + SQL Server
- Authentication/Authorization rule validation
- Performance testing under load
- Mutation testing for rule robustness

The foundation is built for scaling test complexity when required.

---

## Summary

✔ Business rules validated  
✔ Controllers tested via real HTTP execution  
✔ No dependency on real SQL Server  
✔ Clean Architecture maintained  
✔ Matches Senior-level assessment expectations

SafeVault can evolve confidently — changes are protected by automation.
