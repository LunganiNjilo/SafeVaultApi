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

```bash
dotnet restore
dotnet build
dotnet run --project SafeVaultApi
