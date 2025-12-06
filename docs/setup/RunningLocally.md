## ▶️ Run API Locally (No Docker Required)

Follow these steps to run the SafeVault API on your machine:

### 1️⃣ Ensure tools are installed

You must have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server (LocalDB, Developer edition, or a remote instance)
- (Optional) SQL Management Tool (Azure Data Studio / SSMS)

To verify .NET is installed:

```bash
dotnet --version
```

You should see a version starting with `8.`

---

### 2️⃣ Configure database connection

Check your connection string located in:

```
SafeVaultApi/appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "Server=(localdb)\\MSSQLLocalDB;Database=SafeVaultDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Update server/credentials if needed based on your SQL setup.

---

### 3️⃣ Apply database migrations and seed data

Run the following commands from the repo root:

```bash
dotnet restore
dotnet ef database update --project Infrastructure --startup-project SafeVaultApi
```

This will:

- Create the database
- Apply the schema
- Seed sample user + accounts + transactions

---

### 4️⃣ Start the API

```bash
dotnet run --project SafeVaultApi
```

If successful, the API will start and display a URL like:

```
http://localhost:5165
```

---

### 5️⃣ Open Swagger UI

Once the server is running, open in browser:

➡️ http://localhost:5165/swagger

You can now explore and test all API endpoints interactively.

---

### Troubleshooting

| Issue | Fix |
|------|-----|
| Database connection failure | Check SQL is running and connection string is correct |
| EF CLI missing | Run: `dotnet tool install --global dotnet-ef` |
| Port already in use | Update `launchSettings.json` to a free port |

---

✔️ You’re now running the SafeVault API locally!


