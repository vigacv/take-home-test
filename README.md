# Solution

## Architecture

The backend follows the clean architecture pattern, separating concerns in 4 layers:

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `Fundo.Domain` | Domain entities and repository interfaces |
| Infrastructure | `Fundo.Infrastructure` | EF Core implementation with SQL Server, migrations, seed data |
| Application | `Fundo.Applications.WebApi` | Controllers, services, request/response models, middleware |
| Tests | `Fundo.Services.Tests` | Unit and integration tests |

The frontend is a basic Angular app that displays loans retrieved from the backend API and incorporates login.

## Implementation Approach

1. **Update .NET and Angular to latest versions:** The project came with .NET 6 and Angular 19, which are outdated. Updated to .NET 10 and Angular 21, which involved some refactoring and updating dependencies.
2. **Resolve vulnerabilities:** Some packages included in the solution had known vulnerabilities. Before adding more dependencies, existing packages were updated to resolve them.
3. **Update .gitignore:** Some files were incorrectly ignored, like `package-lock.json` and EF migrations, so those exclusions were removed.
4. **Implement domain models and infrastructure:** Implemented domain entities, repository interfaces, and the EF Core SQL Server implementation including migrations and seed data.
5. **Add RESTful API endpoints:** The core deliverable — implemented following clean architecture conventions with unit and integration test coverage.
6. **Containerize the backend:** Implemented a Dockerfile to build the backend image and a Docker Compose file to run it alongside the SQL Server image. Challenges encountered are described below.
7. **Frontend features:** Connected the Angular frontend to the backend to display loans.
8. **Logging and exception handling** *(optional)*: Used Serilog for structured logging and added a global exception handler middleware.
9. **Authentication** *(optional)*: Implemented JWT Bearer authentication, requiring a valid token on all loan endpoints. Updated the frontend to support login/logout.
10. **GitHub Actions pipeline** *(optional)*: Added a CI pipeline to build and test the backend on every push.
11. **Swagger/OpenAPI documentation** *(extra)*: Added Swagger with JWT support so reviewers can explore and test the API directly in the browser without needing a separate client.

## Features Completed

All required features were implemented: RESTful API endpoints, EF Core with SQL Server, seed data, unit and integration tests, Docker/Docker Compose, and the Angular frontend. All three optional bonus items (logging, authentication, GitHub Actions) were also completed.

## Challenges Faced

- **Docker image download failures:** When setting up Docker Compose, pulling the `mssql/server` image consistently failed mid-download. After debugging with manual `curl` requests to the specific failing layer URLs, the root cause was a connection reset occurring after a period of time — likely a CDN or ISP-level issue. The workaround was routing through a VPN to pull the image successfully.
- **Integration tests after adding authentication:** Once JWT authentication was added, integration tests stopped working because the test host had no valid token. The solution was to add a custom test authentication handler that automatically satisfies the auth requirement in the `Testing` environment, keeping tests isolated from the auth flow.

## Improvements Given More Time

- Replace the static user list in `appsettings.json` with a proper Users repository backed by SQL Server.
- Harden authentication: add refresh tokens, token revocation, and role-based authorization.
- Expand the frontend: add forms to create loans and make payments.
- Containerize the frontend and include it in Docker Compose.
