# Task Management API

A .NET 8 Web API for managing **projects** and **tasks** with JWT authentication. The solution is organized as a layered application that demonstrates enterprise patterns: clean architecture, dependency injection, SOLID, DTOs, global exception handling, and validation.

## Architecture Requirements

The project demonstrates the following architectural and design practices:

| Requirement | How it is implemented in this project |
|-------------|----------------------------------------|
| **Clean Architecture** | Layers depend inward: **Domain** (entities, contracts, exceptions) has no infrastructure references; **Service** implements business logic; **Presistence** handles EF Core and repositories; **Presentation** exposes HTTP APIs; **Chatty** (`TaskManagementApi`) is the composition root that wires everything together. |
| **Dependency Injection** | Services are registered in extension methods (`AddPresistenceConfig`, `AddServiceConfiguration`) and consumed via constructor injection in controllers and services. ASP.NET Core Identity and JWT authentication are also registered in `Program.cs`. |
| **SOLID Principles** | **S**ingle responsibility per layer (e.g. `ProjectService` for project rules, `GenericRepository` for data access). **O**pen/closed via specifications and interfaces. **L**iskov through `BaseEntity<T>` and generic repositories. **I**nterface segregation (`IProjectService`, `ITaskService`, `IAuthService`, `IUnitOfWork`). **D**ependency inversion: high-level code depends on abstractions in `Domain` and `ServiceAbstraction`, not concrete persistence. |
| **DTO Usage** | Request/response models live in **SharedData/DTOs** (e.g. `CreateProjectDTO`, `TaskResultDTO`). AutoMapper profiles map between DTOs and domain entities so controllers and clients never depend on persistence models directly. |
| **Global Exception Handling** | `CustomExceptionMiddleware` catches unhandled exceptions, maps domain exceptions (`NotFoundException`, `BadRequestException`) to HTTP status codes, and returns a consistent `ErrorModel` JSON payload. |
| **Validation** | Data annotations on DTOs (`[Required]`, `[MaxLength]`, `[Compare]`) plus `ModelState` checks in controllers return structured `ApiResponse` validation errors before business logic runs. |

---

## Solution structure

```
TaskManegment/
├── Chatty/                    # Web host (TaskManagementApi) — Program.cs, middleware, Serilog
├── Domain/                    # Entities, repository contracts, specifications, domain exceptions
├── Service/                   # Business services, AutoMapper, specification implementations
├── ServiceAbstraction/        # Service interfaces (IProjectService, ITaskService, IAuthService, …)
├── Presistence/               # EF Core DbContext, migrations, GenericRepository, Unit of Work
├── Presentation/              # API controllers
└── SharedData/                # DTOs, enums, ApiResponse wrapper, constants
```

### Layer dependencies

```
Presentation  →  ServiceAbstraction, SharedData
Service       →  Domain, ServiceAbstraction, SharedData
Presistence   →  Domain
Chatty        →  Presentation, Presistence, Service
Domain        →  (no project references to outer layers)
```

---

## Features

### Authentication & authorization

- **Register** — Create users with ASP.NET Core Identity (`ApplicationUser`).
- **Login** — Email/password sign-in; returns JWT access token.
- **Refresh token** — Rotate access tokens using a stored refresh token.
- **JWT Bearer** — Protected project and task endpoints require a valid token (`[Authorize]` on `ProjectController` and `TaskController`).

### Project management

- Create, read, update, and delete projects.
- Projects are scoped to the authenticated user (`OwnerId`).
- Fields: name, description, created date.

### Task management

- Create tasks under a project (title, description, status, due date, priority).
- Update task status.
- List tasks by project.
- Delete tasks.
- Ownership enforced via specifications (user must own the project).

### Data access patterns

- **Repository pattern** — `IGenaricRepository<T, TK>` with `GenericRepository` implementation.
- **Unit of Work** — `IUnitOfWork` coordinates repositories and `SaveChangesAsync`.
- **Specification pattern** — Query filters such as `ProjectsByOwnerSpecification`, `TaskByIdAndOwnerSpecification` keep data access logic reusable and testable.
- **Entity Framework Core** — SQL Server with fluent configurations in `Presistence/Data/Configuration`.

### Cross-cutting concerns

- **Structured logging** — Serilog (console + rolling file) and `ILoggingService` for operation-level logs.
- **HTTP request logging** — `UseSerilogRequestLogging()` in the pipeline.
- **Uniform API responses** — `ApiResponse<T>` wrapper for success/failure payloads.
- **CORS** — `AllowAll` policy for development.
- **Swagger / OpenAPI** — Available in Development environment.

### Domain model

| Entity | Purpose |
|--------|---------|
| `ApplicationUser` | Identity user (auth) |
| `RefreshToken` | JWT refresh token storage |
| `Project` | Container for tasks; owned by a user |
| `TaskItem` | Task with status, priority, due date, project link |

### Task status & priority

- **Status:** `Pending`, `InProgress`, `Completed` (see `SharedData/Enums/TaskItemStatus.cs`)
- **Priority:** `Low`, `Medium`, `High` (see `SharedData/Enums/TaskPriority.cs`)

---

## API endpoints

### Auth (`/api/Auth`) — no authorization required

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/Auth/Register` | Register a new user |
| POST | `/api/Auth/Login` | Login and receive JWT |
| POST | `/api/Auth/RefreshToken?token={refreshToken}` | Refresh access token |

### Projects (`/api/Project`) — requires JWT

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/Project` | Create project |
| GET | `/api/Project` | List current user's projects |
| GET | `/api/Project/{id}` | Get project by id |
| PUT | `/api/Project/{id}` | Update project |
| DELETE | `/api/Project/{id}` | Delete project |

### Tasks (`/api/Task`) — requires JWT

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/Task` | Create task |
| PUT | `/api/Task/{id}/status` | Update task status |
| GET | `/api/Task/project/{projectId}` | List tasks for a project |
| DELETE | `/api/Task/{id}` | Delete task |

---

## Architecture deep dive

### Clean Architecture

- **Domain** holds core business types (`Project`, `TaskItem`), contracts (`IUnitOfWork`, `ISpecification`), and domain exceptions.
- **Application logic** lives in **Service** (`ProjectService`, `TaskService`, `AuthService`).
- **Infrastructure** is **Presistence** (EF Core, SQL Server, repositories).
- **Presentation** stays thin: validate input, call services, map HTTP status codes.
- **Chatty** bootstraps DI, middleware, authentication, and logging.

### Dependency Injection

Registrations (simplified):

```csharp
// Program.cs
builder.Services.AddPresistenceConfig(builder.Configuration);
builder.Services.AddServiceConfiguration();

// PresistenceLayerConfiguration.cs
services.AddDbContext<ApplicationDbContext>(...);
services.AddScoped<IUnitOfWork, UnitOfWork>();

// ServiceLayerConfigurations.cs
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IProjectService, ProjectService>();
services.AddScoped<ITaskService, TaskService>();
services.AddSingleton<ILoggingService, LoggingService>();
services.AddAutoMapper(...);
```

Controllers receive `IProjectService`, `ITaskService`, or `IAuthService` via constructor injection.

### DTO usage

| DTO | Usage |
|-----|--------|
| `RegisterDTO`, `LoginDTO` | Auth requests |
| `RegisterResultDTO`, `LoginResultDTO` | Auth responses |
| `CreateProjectDTO`, `UpdateProjectDTO`, `ProjectResultDTO` | Project CRUD |
| `CreateTaskDTO`, `UpdateTaskStatusDTO`, `TaskResultDTO` | Task operations |

AutoMapper profiles: `ProjectMappingProfile`, `TaskMappingProfile`.

### Global exception handling

`CustomExceptionMiddleware` runs early in the pipeline and:

- Maps `NotFoundException` → **404**
- Maps `BadRequestException` → **400**
- Maps other exceptions → **500**
- Returns JSON `ErrorModel` with `statusCode` and `message`
- Handles unmatched routes with a 404 response body

### Validation

Example DTO validation (`CreateProjectDTO`):

```csharp
[Required]
[MaxLength(200)]
public string Name { get; set; }

[MaxLength(1000)]
public string Description { get; set; }
```

Controllers check `ModelState.IsValid` and return:

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": { "Name": ["The Name field is required."] }
}
```

`RegisterDTO` additionally uses `[Compare("Password")]` for confirm-password validation.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full instance)
- Optional: Visual Studio 2022 or VS Code

## Getting started

### 1. Clone and restore

```bash
git clone <repository-url>
cd TaskManegment
dotnet restore
```

### 2. Configure database and JWT

Edit `Chatty/appsettings.json` (or `appsettings.Development.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementApi;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "JWT": {
    "Issuer": "TaskManagementApi",
    "Audience": "TaskManagementApi",
    "Key": "<your-secret-key-at-least-32-characters>"
  }
}
```

### 3. Apply migrations

```bash
cd Presistence
dotnet ef database update --startup-project ../Chatty
```

### 4. Run the API

```bash
cd Chatty
dotnet run
```

- **Swagger UI:** `https://localhost:<port>/swagger` (Development)
- Use **Login** or **Register**, then authorize in Swagger with `Bearer <token>` for protected endpoints.

## Tech stack

| Technology | Role |
|------------|------|
| .NET 8 | Runtime and Web API |
| ASP.NET Core Identity | User management |
| JWT Bearer | API authentication |
| Entity Framework Core 8 | ORM |
| SQL Server | Database |
| AutoMapper | DTO ↔ entity mapping |
| Serilog | Logging |
| Swashbuckle | OpenAPI / Swagger |

## Additional documentation

- [LOGGING_GUIDE.md](LOGGING_GUIDE.md) — Serilog setup and `ILoggingService` usage examples.

## License

See repository license file if present.
