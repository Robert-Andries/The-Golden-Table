# The Golden Table
**STATUS: ALPHA (WORK IN PROGRESS)**

This application is currently in active development. Features, architecture, and database schemas are subject to breaking changes.

A highly scalable, technical robust solution for managing restaurant operations. This application acts as the digital backbone for a restaurant, handling real-time order processing, tracking of delivery, reviews, and a staff dashboard.

## Overview
The Golden Table is engineered to handle the complex, high-concurrency environment of a modern restaurant. Core capabilities include:

 - **Order Processing**: End-to-end lifecycle management of tickets from floor to kitchen.

- **Menu & Inventory Catalog**: Item availability and categorization.

- **Staff dashboard for Order & Inventory operations**: Providing a user-friendly way for staff to manage the insights of the system.

The system is architected as a Modular Monolith, each module strictly being build using Clean Architecture and utilizing Domain-Driven Design (DDD) principles. This provides the operational simplicity of a single deployment unit while maintaining the strict logical boundaries and isolation required to easily transition to microservices if future scale demands it.

## Technical Architecture
Instead of a single set deployable unit, the solution is divided into multiple Modules (e.g. Catalog Module). Each module is strictly isolated and implements its own Clean Architecture stack:

 - **Domain** (GoldenTable.Modules.[Name].Domain): The core domain model. Contains rich Entities, Value Objects, and Domain Events. Independent of all other layers and external concerns.

- **Application** (GoldenTable.Modules.[Name].Application): Use case orchestration. Implements CQRS via the Mediator pattern. Contains Commands, Queries, Validators, and internal event handlers.

- **Infrastructure** (GoldenTable.Modules.[Name].Infrastructure): Module-specific implementation details. Contains EF Core configurations for PostgreSQL, Redis caching implementations and module specific repositories.

- **Presentation** (GoldenTable.Modules.[Name].Presentation): Containing methods to map Minimal API endpoints specific to the module's use case.

- **Testing** (GoldenTable.Modules.[Name].Tests.[TestingStrategy]): Having multiple testing strategies (e.g. Unit, integration, E2E) ensuring bug-free operations.

- **Modules** communicate with each other exclusively through an docker hosted RabbitMQ that act as a message broker (for asynchronous communication), ensuring no direct database or domain coupling. To mitigate eventual inconsistencies, the Outbox pattern is applied.

## Tech Stack

**Framework**: .NET 10.0

**Architecture**: Modular Monolith, Clean Architecture (per module), DDD, CQRS

**Database**: PostgreSQL (Relational persistence)

**Caching**: Redis (Distributed caching & idempotency keys)

**Containerization**: Docker & Docker Compose

**Testing**: xUnit, FluentAssertions, RedisContainer, PostgreSqlContainer, Bongo

**CI Pipeline**: GitHub Actions (Build, Test, Snyk, SonarQube)

## Setup Instructions
### Prerequisites
- **.NET 10.0 SDK**
  
- **Docker Desktop** (Required for local PostgreSQL and Redis instances)

- **A preferred IDE** (Visual Studio, JetBrains Rider, or VS Code)

## Installation
### Clone the repository:

```Bash
git clone https://github.com/your-username/the-golden-table.git
cd the-golden-table
```
### Spin up Infrastructure (Database & Cache):
Ensure Docker is running, then start the required services using the provided compose file:

```Bash
docker-compose up -d
```

### Apply Migrations:
Because this is a modular monolith, each module manages its own schema. The main API that orchestrates the modules will automatically apply migration. All you have to do is provide the postgres and redis connection string into `src/Api/GoldenTable.Api/appsettings.json`

### Testing
Quality assurance is embedded into the pipeline. The modular structure isolates unit and integration tests per module(Requires Docker to spin up Testcontainers for PostgreSQL/Redis).
```Bash
dotnet test
```

## CI Pipeline
The project uses a robust GitHub Actions workflow for Continuous Integration:

Triggers on push and pull_request to the main, release or develop branches.

Executes dotnet build in Release configuration.

Runs the full test suite (Unit, Integration, Architecture).

Snyk Security Scan: Analyzes NuGet dependencies and Dockerfiles for known vulnerabilities.

SonarQube Analysis: Enforces code quality, checks for code smells, and validates maintainability indices.
