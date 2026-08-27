# URL Shortener Architecture Lab

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Learning-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)

A hands-on software architecture and platform engineering lab that progressively evolves a simple URL shortener into a production-oriented, cloud-native system.

> The URL shortener is the vehicle. The real subject is evolutionary architecture: how to improve boundaries, composition, deployment, scalability, and operability without rewriting everything at once.

## Table of Contents

- [Why This Project Exists](#why-this-project-exists)
- [Learning Objectives](#learning-objectives)
- [Current Status](#current-status)
- [Technology Stack](#technology-stack)
- [Current Capabilities](#current-capabilities)
- [Current Architecture](#current-architecture)
- [Solution Structure](#solution-structure)
- [Request Flow](#request-flow)
- [Getting Started](#getting-started)
- [Docker Journey](#docker-journey)
- [Kubernetes Journey](#kubernetes-journey)
- [Architecture Evolution](#architecture-evolution)
- [Roadmap](#roadmap)
- [Future Enhancements](#future-enhancements)

## Why This Project Exists

Many systems begin as a small application and gradually acquire new requirements: independent ownership, selective deployment, containerization, horizontal scale, reliable data stores, routing, and observability.

This lab makes that progression explicit. Each phase introduces one architectural or platform concern while preserving working behavior wherever possible. The result is a practical record of design decisions, trade-offs, and migration seams rather than a collection of disconnected technology demos.

The project is intentionally evolutionary:

```text
Simple application
        |
        v
Clear module boundaries
        |
        v
Selective composition
        |
        v
Containerized hosts
        |
        v
Kubernetes deployment
        |
        v
Scalable, observable cloud-native system
```

## Learning Objectives

- Design and evolve a traditional layered monolith.
- Establish business-oriented module boundaries.
- Use separate assemblies as compile-time boundaries.
- Compose selected controllers into different hosts with Application Parts.
- Practice dependency inversion between modules.
- Understand the operational impact of Docker image and process boundaries.
- Move from local development to repeatable container and Kubernetes workflows.
- Identify when a database, ingress, scaling, or observability decision is justified.
- Prefer incremental, reversible architecture changes over premature distribution.

## Current Status

The application currently demonstrates an **assembly-based Modular Monolith with Selective Composition**.

```text
One solution
    |
    +-- Multiple module assemblies
    +-- Multiple optional hosts
    +-- One process per running host
    +-- One PostgreSQL database
```

`FullHost` exposes the complete API. `SearchHost` selectively exposes only the search controller using ASP.NET Core Application Parts. Both hosts reuse the same module assemblies and remain part of the same monolithic solution.

## Technology Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 9 |
| Web framework | ASP.NET Core Web API |
| Persistence | Entity Framework Core |
| Database | PostgreSQL |
| API documentation | Swagger / OpenAPI |
| Modular composition | .NET assemblies and Application Parts |
| Containers | Docker (Phase 5 in progress) |
| Orchestration | Kubernetes (planned) |
| Local database | PostgreSQL |

## Current Capabilities

- Create a short URL.
- Resolve a short code to its original URL.
- Search stored URLs.
- Run a full API host.
- Run a search-only host.
- Select controller assemblies per host.
- Preserve one shared EF Core model and PostgreSQL database.
- Demonstrate dependency inversion through a Search-owned repository abstraction.

### API Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/urls` | Creates a short URL. |
| `GET` | `/urls/{shortCode}` | Resolves a short code. |
| `GET` | `/search?q=google` | Searches original URLs. |

## Current Architecture

### FullHost

```text
+------------------+
|      Client      |
+--------+---------+
         |
         v
+------------------+       +-----------------------------+
|    FullHost      |------>| Urls Module                 |
|                  |       | Controller                  |
| Application      |       | Service                     |
| Parts:            |       | Repository                  |
| - Urls            |       +--------------+--------------+
| - Search          |                      |
+--------+---------+                      v
         |                    +-----------------------------+
         +------------------->| Search Module               |
                              | Controller                  |
                              | Service                     |
                              +--------------+--------------+
                                             |
                                             v
                              +-----------------------------+
                              | Shared Web Data             |
                              | AppDbContext -> PostgreSQL  |
                              +-----------------------------+
```

### SearchHost

```text
+------------------+
|      Client      |
+--------+---------+
         |
         v
+------------------+
|    SearchHost    |
|                  |
| Application     |
| Part:           |
| - Search only   |
+--------+---------+
         |
         v
+-----------------------------+
| Search Module               |
| Controller -> Service       |
+--------------+--------------+
               |
               v
+-----------------------------+
| Urls Repository             |
| Shared AppDbContext         |
| PostgreSQL                 |
+-----------------------------+
```

The SearchHost does not load the URLs controller assembly, so URL routes return `404`. Search can still function because its repository abstraction is implemented by the URL repository at runtime. This is selective endpoint composition, not independent deployment of the modules.

### Dependency Direction

```text
Search Module
    |
    +--> ISearchRepository
              ^
              |
Urls Module - UrlRepository
              |
              v
        AppDbContext
              |
              v
            PostgreSQL
```

The Search module owns the abstraction it consumes. The URL module provides the implementation. This improves ownership and dependency direction without pretending the modules are already independently deployable.

## Solution Structure

```text
UrlShortenerArchitectureLab.sln
|
+-- src/
    |
    +-- FullHost/
    |   +-- Program.cs
    |   +-- FullHost.csproj
    |   +-- Properties/launchSettings.json
    |
    +-- SearchHost/
    |   +-- Program.cs
    |   +-- SearchHost.csproj
    |   +-- Properties/launchSettings.json
    |
    +-- UrlShortener.Modules.Urls/
    |   +-- Controllers/
    |   +-- DependencyInjection/
    |   +-- Models/
    |   +-- Repositories/
    |   +-- Services/
    |
    +-- UrlShortener.Modules.Search/
    |   +-- Controllers/
    |   +-- DependencyInjection/
    |   +-- Models/
    |   +-- Repositories/
    |   +-- Services/
    |
    +-- UrlShortener.Web/
        +-- Data/AppDbContext.cs
        +-- Migrations/
        +-- Program.cs
        +-- UrlShortener.Web.csproj
```

The Web project owns EF Core, migrations, PostgreSQL configuration, and shared host infrastructure. The module assemblies own business capabilities and controllers. The hosts decide which controllers are composed into a running application.

## Request Flow

For `POST /urls` on `FullHost`:

```text
HTTP request
    -> FullHost routing
    -> UrlsController
    -> IUrlService / UrlService
    -> IUrlRepository / UrlRepository
    -> AppDbContext
    -> PostgreSQL
    -> HTTP response
```

For `GET /search?q=google`:

```text
HTTP request
    -> selected SearchController
    -> SearchService
    -> ISearchRepository
    -> UrlRepository implementation
    -> AppDbContext
    -> PostgreSQL
    -> search response
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- EF Core CLI

Verify the tools:

```powershell
dotnet --version
dotnet ef --version
```

Install the EF Core CLI if necessary:

```powershell
dotnet tool install --global dotnet-ef
```

### Restore and Build

From the repository root:

```powershell
dotnet restore .\UrlShortenerArchitectureLab.sln
dotnet build .\UrlShortenerArchitectureLab.sln
```

### Apply the Database Migration

The database is shared by the hosts and stored at the repository root:

```powershell
dotnet ef database update `
  --project .\src\UrlShortener.Web\UrlShortener.Web.csproj `
  --startup-project .\src\UrlShortener.Web\UrlShortener.Web.csproj
```

### Run FullHost

```powershell
dotnet run `
  --project .\src\FullHost\FullHost.csproj `
  --launch-profile http
```

Open Swagger at:

```text
http://localhost:5082/swagger
```

### Run SearchHost

```powershell
dotnet run `
  --project .\src\SearchHost\SearchHost.csproj `
  --launch-profile http
```

Open SearchHost Swagger at:

```text
http://localhost:5083/swagger
```

SearchHost exposes only `/search`. Requests to `/urls` and `/urls/{shortCode}` return `404`.

## Docker Journey

Dockerization is the next platform step in this lab. The purpose is not simply to package the application; it is to make the host boundary, runtime dependencies, configuration, and deployment artifact explicit.

The planned container workflow is:

```text
Source code
    -> Multi-stage Docker build
    -> Small runtime image
    -> One host process per container
    -> Repeatable local and cloud execution
```

Key questions this phase explores:

- Which host should be containerized?
- Should `FullHost` and `SearchHost` be separate images or one image with different commands?
- How should PostgreSQL storage be provisioned and persisted?
- Which configuration belongs in environment variables?
- What should the container health check verify?
- How do image size, startup time, and non-root execution affect production readiness?

## Kubernetes Journey

Kubernetes is planned after containerization and Compose. It will provide a place to study scheduling, service discovery, configuration, health probes, scaling, and ingress routing.

The planned deployment model is:

```text
+------------------+
| Ingress / Gateway|
+--------+---------+
         |
   +-----+------+
   |            |
   v            v
FullHost      SearchHost
   |            |
   +-----+------+
         |
         v
  Persistent data layer
```

The Kubernetes phase will intentionally address operational concerns in sequence:

1. Deploy a single host.
2. Add configuration and secrets handling.
3. Add readiness and liveness probes.
4. Add Services and ingress routing.
5. Evaluate replicas and stateful database requirements.
6. Evaluate PostgreSQL storage and horizontal scaling requirements.

## Architecture Evolution

### Phase 1: Traditional Monolith

One ASP.NET Core project with a straightforward flow:

```text
Controller -> Service -> Repository -> EF Core -> PostgreSQL
```

### Phase 2: Folder-Based Modular Monolith

Code ownership shifted from purely technical folders to business modules while remaining one application.

### Phase 3: Assembly-Based Modular Monolith

Modules moved into separate class libraries, creating compile-time project boundaries without creating separate deployable services.

### Phase 4: Selective Composition

`FullHost` and `SearchHost` selectively compose module controllers through Application Parts. The running host determines which endpoints are exposed.

### Phase 5: Dependency Inversion

Search moved from depending directly on `IUrlRepository` to owning and consuming `ISearchRepository`. The URL repository remains the implementation, so runtime coupling is still explicit.

### Future Phases

Containerization, orchestration, database migration, scaling, routing, and observability will be added only when they answer a concrete architectural or operational question.

## Roadmap

| Phase | Focus | Status |
| --- | --- | --- |
| 1 | Traditional Monolith | ✅ Completed |
| 2 | Modular Monolith | ✅ Completed |
| 3 | Assembly-Based Modules | ✅ Completed |
| 4 | Selective Composition with Application Parts | ✅ Completed |
| 5 | Dockerization | 🚧 In Progress |
| 6 | Docker Compose | 📋 Planned |
| 7 | Kubernetes Deployment | 📋 Planned |
| 8 | PostgreSQL Migration | ✅ Completed |
| 9 | Horizontal Scaling and Ingress Routing | 📋 Planned |
| 10 | Observability and Production Readiness | 📋 Planned |

## Future Enhancements

- Add Dockerfiles for the selected hosts.
- Add Docker Compose for repeatable local multi-host execution.
- Tune PostgreSQL schema, pooling, backups, and operational policies.
- Add Kubernetes manifests or Helm-based deployment configuration.
- Add ingress routing and host-specific traffic policies.
- Add structured logging, metrics, distributed tracing, and health checks.
- Add resilience and failure-mode testing.
- Add CI validation for build, migrations, container images, and deployment manifests.
- Measure rather than assume the need for further module extraction.

## Design Principles

- Prefer clear boundaries before distributed systems.
- Keep changes incremental and behavior-preserving.
- Treat deployment topology as an explicit architectural decision.
- Use abstractions to express capabilities, not to hide accidental complexity.
- Separate module ownership from runtime deployment.
- Make state, scaling, and operational trade-offs visible.
- Introduce infrastructure when it solves a demonstrated problem.
