# Products API

ASP.NET Core Web API on **.NET 10** built with **Clean Architecture**, **CQRS (MediatR)**, **EF Core (Code First)** and **SQL Server**.

## Architecture

```
src/
  ProductsApi.Domain          Entities only (Product). No dependencies.
  ProductsApi.Application     CQRS commands/queries + handlers, DTOs, FluentValidation validators,
                              MediatR validation pipeline, repository / unit-of-work abstractions.
                              Depends on Domain.
  ProductsApi.Infrastructure  EF Core DbContext, entity configuration, migrations, repository
                              implementation. Depends on Application (and transitively Domain).
  ProductsApi.Api             Controllers, Swagger, exception handling, composition root (DI).
                              Depends on Application + Infrastructure.
tests/
  ProductsApi.Tests           xUnit tests (domain, validators, handlers, controller).
```

Dependency rule: `Api -> Infrastructure -> Application -> Domain`. Application defines
`IProductRepository` / `IUnitOfWork`; Infrastructure implements them, so business logic never
references EF Core.

**Request flow:** Controller -> `ISender.Send(command/query)` -> `ValidationBehavior`
(FluentValidation) -> handler -> repository -> EF Core -> SQL Server.

| Endpoint | Request | Success | Errors |
|---|---|---|---|
| `GET /api/products` | `GetProductsQuery` | `200` list of products | - |
| `GET /api/products/{id}` | `GetProductByIdQuery` | `200` product | `400` id <= 0, `404` not found |
| `POST /api/products` | `CreateProductCommand` | `201` + `Location` header | `400` validation errors (RFC 7807) |

Unhandled exceptions return `500` as `ProblemDetails`.

**Product rules:** `Name` required, max 200 chars. `Price` > 0, at most 2 decimal places
(`decimal(18,2)` in SQL Server).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server 2019+ (local install, SQL Server Express, or Docker)
- EF Core CLI tool: `dotnet tool install --global dotnet-ef`

## SQL Server configuration

The connection string is `ConnectionStrings:DefaultConnection` in
`src/ProductsApi.Api/appsettings.json`. The default targets SQL Server on `localhost,1433` with the
`sa` user.

Start SQL Server with Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_strong_Passw0rd" \
  -p 1433:1433 --name products-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Do not commit real credentials. Override the connection string without editing files:

```bash
# environment variable (double underscore = section separator)
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=ProductsDb;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"

# or user-secrets
dotnet user-secrets init --project src/ProductsApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>" --project src/ProductsApi.Api
```

Windows LocalDB example: `Server=(localdb)\MSSQLLocalDB;Database=ProductsDb;Trusted_Connection=True;`

## Migrations

An `InitialCreate` migration is included in
`src/ProductsApi.Infrastructure/Persistence/Migrations`. Create/update the database:

```bash
dotnet ef database update \
  --project src/ProductsApi.Infrastructure \
  --startup-project src/ProductsApi.Api
```

Add a new migration after changing the model:

```bash
dotnet ef migrations add <Name> \
  --project src/ProductsApi.Infrastructure \
  --startup-project src/ProductsApi.Api \
  --output-dir Persistence/Migrations
```

> **Note:** the included `InitialCreate` migration and model snapshot were written by hand (the
> authoring environment had no NuGet access, so `dotnet ef` could not run). If EF reports pending
> model changes or a snapshot mismatch, delete `Persistence/Migrations` and regenerate with
> `dotnet ef migrations add InitialCreate ...` as shown above.

## Logging

Logging uses **Serilog** (`Serilog.AspNetCore`), configured in the `Serilog` section of `appsettings.json`.

- **Sinks:** console, plus a daily rolling file at `logs/products-api-<date>.log` (14 days kept; `logs/` is git-ignored).
- **Per request:** `UseSerilogRequestLogging` writes one summary line (method, path, status, elapsed).
- **Per command/query:** `LoggingBehavior` in the MediatR pipeline logs start, duration and failures. It wraps `ValidationBehavior`, so validation failures are logged too. Request payloads are never logged.
- **Errors:** `GlobalExceptionHandler` logs unhandled exceptions with the full stack trace.
- **Levels:** change them under `Serilog:MinimumLevel`. `appsettings.Development.json` sets `Debug` and shows EF Core SQL commands.

Use it in new code by injecting `ILogger<T>`.

## Build, run and test

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/ProductsApi.Api --launch-profile http
```


they **do not exercise EF Core or SQL Server**. Verify the database layer manually with the steps
above (migrate, then run the curl calls against a real SQL Server).
