# 🐳 SafeVault — Docker Compose Guide

This guide explains how to run the SafeVault API and SQL Server database locally using Docker Compose.  
No need to install SQL Server manually — everything runs in containers.

---

## ▶️ Prerequisites

Ensure you have the following installed:

- Docker Desktop (Windows / Mac)
- .NET 8 SDK (optional — for local development)

---

## 📁 Project Structure

After cloning the repository, the root folder should look similar to:

SafeVault/  
 ├─ SafeVaultApi/  
 ├─ Infrastructure/  
 ├─ docker-compose.yml  
 ├─ SafeVault.sln  

Run all commands from the repository root:

    ...\SafeVault>

---

## 🚀 Running SafeVault

From the project root directory, run:

    docker compose up --build

This will:

1. Build the SafeVault API Docker image  
2. Start the SQL Server database container  
3. Apply Entity Framework Core migrations automatically  
4. Expose the API on your local machine

Once the containers are running, you can access:

- Swagger UI: https://localhost:5001/swagger  
- HTTP API:  http://localhost:5000  
- HTTPS API: https://localhost:5001  

---

## 🗄 Database Connection

The API connects to the SQL Server container with the following settings:

| Setting | Value                    |
|--------|--------------------------|
| Server | sqlserver            |
| Database | SafeVaultDb            |
| Port   | 1433                     |
| Username | sa                     |
| Password | Your_password123 |

No local SQL Server installation is required; the database runs entirely in Docker.

---

## 🛑 Stopping Containers

To stop the API and database containers:

    docker compose down

To stop the containers and remove the database volume (fresh database next run):

    docker compose down -v

Use the `-v` option when you want to reset the database state completely.

---

## 🧪 Running Tests in Docker (Optional)

If you have a separate compose file for tests (for example `docker-compose.tests.yml`), you can run:

    docker compose -f docker-compose.tests.yml up --build --abort-on-container-exit

This will:

- Start the API and database containers  
- Execute the NUnit test suite  
- Stop all containers once tests complete

---

## ❓ Troubleshooting

| Issue                        | Possible Cause                         | Resolution                                      |
|-----------------------------|----------------------------------------|------------------------------------------------|
| Ports already in use        | Other services using 5000/5001/1433    | Change ports in `docker-compose.yml` and retry |
| Swagger HTTPS warning       | Self-signed dev certificate            | Proceed past the browser security warning      |
| Database not ready on start | SQL Server still initializing          | Re-run `docker compose up --build`             |
| Code changes not reflected  | Docker using cached image layers       | Always include `--build` when restarting       |

---

## 👨‍💻 Local Development (Hybrid Mode)

You can run SQL Server in Docker and the API via `dotnet run` for easier debugging.

Start only the database container:

    docker compose up -d safeguard-sql

Then run the API project locally:

    dotnet run --project SafeVaultApi

This setup keeps the database in Docker while allowing hot reload and local debugging for the API.

---
