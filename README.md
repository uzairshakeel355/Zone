# ShopZone.Api

Backend for ShopZone — ASP.NET Core 6 Web API, PostgreSQL, JWT authentication.

## Prerequisites
- .NET 6 SDK
- Docker Desktop

## Setup
1. `docker compose up -d` — starts Postgres
2. `dotnet user-secrets set "Jwt:Key" "<your-secret>"`
3. `dotnet ef database update`
4. `dotnet run` (or F5 in Visual Studio) — Swagger opens automatically

## Stack
ASP.NET Core 6 · EF Core 6 · PostgreSQL · Identity Core · JWT Bearer Auth

## Scope
Weeks 1-4 complete: authentication, product/category catalog, cart, and checkout/orders.
Weeks 5-8 (admin panel, real-time order tracking, search/performance, deployment) were
scoped in the original plan but not built — documented here as a clear extension path,
not an oversight.