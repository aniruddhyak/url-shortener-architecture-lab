# How to Start the Application

This solution is a .NET 9 ASP.NET Core Web API using EF Core and SQLite. The selectable hosts are `src\FullHost` and `src\SearchHost`.

## Prerequisites

Install or verify:

- .NET 9 SDK
- Entity Framework Core CLI

Check the .NET SDK:

```powershell
dotnet --version
```

Check the EF Core CLI:

```powershell
dotnet ef --version
```

If `dotnet ef` is not installed, install it with:

```powershell
dotnet tool install --global dotnet-ef
```

## Start from a New Database

Open PowerShell in the project folder:

```powershell
cd Q:\Personal\UrlShortenerArchitectureLab
```

Restore dependencies:

```powershell
dotnet restore
```

Apply the EF Core migration to create the SQLite schema:

```powershell
dotnet ef database update --project .\src\UrlShortener.Web\UrlShortener.Web.csproj
```

Start the API:

```powershell
dotnet run --project .\src\FullHost\FullHost.csproj
```

The API will display the local HTTP address in the terminal. Use `SearchHost.csproj` instead to run the search-only host.

## Swagger

While the application is running in the Development environment, open:

```text
http://localhost:5000/swagger
```

Use the actual port shown by `dotnet run` if it is different.

## Test the API

Create a short URL:

```powershell
Invoke-RestMethod `
    -Uri http://localhost:5000/urls `
    -Method Post `
    -ContentType "application/json" `
    -Body '{"url":"https://google.com"}'
```

Example response:

```json
{
  "shortCode": "abc123"
}
```

Resolve a short code:

```powershell
Invoke-RestMethod http://localhost:5000/urls/abc123
```

Search URLs:

```powershell
Invoke-RestMethod "http://localhost:5000/search?q=google"
```

## Stop the Application

Press:

```text
Ctrl+C
```

## Troubleshooting

If the API returns:

```text
SQLite Error 1: 'no such table: UrlEntries'
```

Stop the API, apply the migration, and start it again:

```powershell
dotnet ef database update
dotnet run
```

The SQLite database is stored in the project folder as:

```text
urlshortener.db
```
