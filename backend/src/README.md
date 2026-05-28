## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (local) **or** Docker (for containerised setup)

---

## Running with Docker (recommended)

### 1. Export the development certificate (one-time)

```powershell
mkdir $env:USERPROFILE\.aspnet\https\

dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p CertPassword123!

dotnet dev-certs https --trust
```

### 2. Start the containers

From the **repo root**:

```sh
docker compose up -d --build
```

The API will be available at `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

---

## Running locally

### 1. Configure the database connection

Update `appsettings.json` with your connection string. The default targets a local SQL Server Express instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=FundoLoanDb;Integrated Security=True;TrustServerCertificate=True;"
}
```

### 2. Trust the HTTPS development certificate

```sh
dotnet dev-certs https --trust
```

This is a one-time step. Skip it if you prefer to use the HTTP endpoint instead.

### 3. Run the API

From this folder (`src`) run:

```sh
dotnet run --project .\Fundo.Applications.WebApi
```

The API listens on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).  
Database migrations are applied automatically on startup.

Verify it is running:

```http
GET https://localhost:5001/loan
```

Expected response: **200 OK**

---

## Running tests

```sh
dotnet test
```

Tests use an in-memory database — no SQL Server required.

---

## Project structure

| Project | Description |
|---|---|
| `Fundo.Domain` | Entities and repository interfaces |
| `Fundo.Infrastructure` | EF Core `DbContext`, SQL Server and in-memory providers, migrations |
| `Fundo.Applications.WebApi` | ASP.NET Core controllers, services, request/response models |
| `Fundo.Services.Tests` | Unit and integration tests (xUnit, Moq, FluentAssertions) |

---

## Authentication

The API uses **JWT Bearer** authentication. All `/loans` endpoints require a valid token.

### Obtaining a token

```http
POST https://localhost:5001/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

Response:

```json
{
  "accessToken": "<jwt>",
  "expiresInSeconds": 3600
}
```

### Using the token

Include the token in the `Authorization` header for every protected request:

```
Authorization: Bearer <accessToken>
```

### Configuration

Credentials and JWT settings are configured in `appsettings.json`:

```json
"Jwt": {
  "Key": "...",
  "Issuer": "fundo-api",
  "Audience": "fundo-client",
  "ExpiresInMinutes": 60
},
"Users": [
  { "Username": "admin", "Password": "admin123" }
]
```

> **Note:** Authentication is skipped in the `Testing` environment so that integration tests run without a token.

---

## Notes

Feel free to modify the code as needed, but try to **respect and extend the current architecture**, as this is intended to be a replica of the Fundo codebase.