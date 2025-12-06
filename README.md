# 🔐 SafeVault API

SafeVault is the backend for a modern digital banking experience.  
It provides a **.NET 8 Clean Architecture Web API** that exposes endpoints for:

- Users (Persons)
- Accounts
- Transactions

The API supports:

- Viewing accounts and balances
- Internal transfers between accounts
- Airtime purchases via an external stub
- Viewing transaction history

A separate Vue.js SPA (SafeVault UI) consumes this API.

---

## 🚀 Tech Stack

- **API:** .NET 8 Web API (Clean Architecture)
- **Database:** SQL Server (production)
- **ORM:** Entity Framework Core
- **Containerisation:** Docker & Docker Compose
- **Frontend:** Vue.js SPA (separate repository)
- **Testing:** NUnit + WebApplicationFactory + NSubstitute

---

## 📦 Quick Start

### Option 1 — Run API locally

<pre><code>
dotnet restore
dotnet build
dotnet run --project SafeVaultApi
</code></pre>

---
### Option 2 — Run API + SQL via Docker Compose

If you have a docker-compose.yml in the repo root, you can start the API and SQL Server together:

<pre><code>
docker compose up --build
</code></pre>

This will:

- Build and run the **SafeVault API** container
- Start a **SQL Server** container
- Optionally start any **stub services** used for external integrations (e.g. airtime)

Once up and running, access Swagger UI:

➡️ `http://localhost:8080/swagger`

> ⚠️ Ports may vary depending on your `docker-compose.yml`.

---

## 🧪 Run Tests

The backend includes automated tests that run in a **Testing** environment that:

- Does **not** require SQL Server
- Skips database migrations and seeding
- Uses mocked repositories and/or in-memory persistence

To execute all backend tests:

<pre><code>
dotnet test
</code></pre>

More information:  
➡️ [`docs/testing/TestingStrategy.md`](docs/testing/TestingStrategy.md)

---

## 📑 Documentation Index

All detailed documentation (architecture diagrams, setup instructions, testing, and assessment mapping) lives inside the `docs` folder.

Start here:

➡️ [`docs/index.md`](docs/index.md)

---

You can navigate to:

- Architecture Overview
- ERD & Domain Modelling
- Sequence & Class Diagrams
- Setup Guides (Local + Docker)
- Testing Strategy
- Assessment Requirements Alignment

---

## 🔗 Related Repository

Frontend SPA for this API:

➡️ **SafeVault UI:** <https://github.com/LunganiNjilo/safevault-ui>

---

