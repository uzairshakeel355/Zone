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