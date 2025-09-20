# 🏗️ OnionArchDemo - .NET Onion Architecture Demo

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Onion-orange.svg)](https://en.wikipedia.org/wiki/Onion_architecture)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A comprehensive .NET demo project showcasing **Onion Architecture** with clean separation of concerns, dependency inversion, and enterprise-level patterns. This project serves as a reference implementation for building scalable, maintainable .NET applications.

## 📋 Table of Contents

- [🏗️ Overview](#-overview)
- [🎯 Architecture](#-architecture)
- [📊 Logging Architecture](#-logging-architecture)
- [📁 Project Structure](#-project-structure)
- [🚀 Features](#-features)
- [⚙️ Prerequisites](#️-prerequisites)
- [🛠️ Installation & Setup](#️-installation--setup)
- [🏃‍♂️ Running the Application](#️-running-the-application)
- [📊 Database Configuration](#-database-configuration)
- [🔧 Configuration](#-configuration)
- [🧪 Testing](#-testing)
- [📚 API Documentation](#-api-documentation)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

## 🏗️ Overview

**OnionArchDemo** is a demonstration project that implements the **Onion Architecture** (also known as Clean Architecture) in .NET 8. This architecture promotes:

- 🔄 **Dependency Inversion**: High-level modules don't depend on low-level modules
- 🧩 **Separation of Concerns**: Clear boundaries between different layers
- 🧪 **Testability**: Easy to unit test business logic
- 🔧 **Maintainability**: Changes in one layer don't affect others
- 📈 **Scalability**: Easy to extend and modify

## 🎯 Architecture

### Onion Architecture Diagram

```mermaid
graph TB
    subgraph "🌐 Presentation Layer"
        A[Controllers]
        B[API Endpoints]
        C[SignalR Hubs]
    end
    
    subgraph "🔧 Infrastructure Layer"
        D[Persistence]
        E[External APIs]
        F[File System]
    end
    
    subgraph "⚙️ Application Layer"
        G[Services]
        H[Use Cases]
        I[DTOs]
    end
    
    subgraph "🏛️ Domain Layer"
        J[Entities]
        K[Contracts]
        L[Specifications]
        M[Domain Services]
    end
    
    A --> G
    B --> G
    C --> G
    G --> J
    G --> K
    D --> J
    E --> G
    F --> G
    
    style J fill:#e1f5fe
    style K fill:#e1f5fe
    style L fill:#e1f5fe
    style M fill:#e1f5fe
    style G fill:#f3e5f5
    style H fill:#f3e5f5
    style I fill:#f3e5f5
    style A fill:#fff3e0
    style B fill:#fff3e0
    style C fill:#fff3e0
    style D fill:#e8f5e8
    style E fill:#e8f5e8
    style F fill:#e8f5e8
```

### Layer Dependencies

```mermaid
flowchart LR
    subgraph "Dependencies Flow"
        A[Presentation] --> B[Application]
        B --> C[Domain]
        D[Infrastructure] --> C
        D --> B
    end
    
    subgraph "Dependency Rule"
        E[🔴 No dependencies inward]
        F[🟢 Dependencies point inward]
        G[🟡 Domain has no dependencies]
    end
```

## 📊 Logging Architecture

### Logging Layer Overview

The project implements a comprehensive logging strategy using **Serilog** with structured logging across all layers. The logging architecture follows the Onion Architecture principles, ensuring that logging concerns are properly separated and injected where needed.

```mermaid
graph TB
    subgraph "🌐 Presentation Layer"
        A[Controllers]
        B[SignalR Hubs]
        A1[Request/Response Logging]
        B1[Real-time Event Logging]
    end
    
    subgraph "⚙️ Application Layer"
        C[Services]
        C1[Business Operation Logging]
        C2[Performance Monitoring]
    end
    
    subgraph "🔧 Infrastructure Layer"
        D[Persistence Layer]
        D1[Database Operation Logging]
        D2[Query Performance Logging]
        D3[Transaction Logging]
    end
    
    subgraph "📝 Logging Infrastructure"
        E[Serilog]
        F[Structured Logging]
        G[Multiple Sinks]
        H[Log Aggregation]
    end
    
    A --> A1
    B --> B1
    C --> C1
    C --> C2
    D --> D1
    D --> D2
    D --> D3
    
    A1 --> E
    B1 --> E
    C1 --> E
    C2 --> E
    D1 --> E
    D2 --> E
    D3 --> E
    
    E --> F
    E --> G
    E --> H
    
    style E fill:#ffeb3b
    style F fill:#ffeb3b
    style G fill:#ffeb3b
    style H fill:#ffeb3b
```

### Logging Flow Diagram

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Service
    participant Repository
    participant LoggingRepo
    participant Serilog
    participant LogSinks
    
    Client->>Controller: HTTP Request
    Note over Controller: Log Request Details
    
    Controller->>Service: Call Business Logic
    Note over Service: Log Business Operation Start
    
    Service->>Repository: Data Access Request
    Repository->>LoggingRepo: Log Database Operation
    
    LoggingRepo->>Serilog: Structured Log Entry
    Serilog->>LogSinks: Write to Multiple Destinations
    
    Repository-->>Service: Data Result
    Note over Service: Log Business Operation Success
    
    Service-->>Controller: Business Result
    Note over Controller: Log Response Details
    
    Controller-->>Client: HTTP Response
```

### Logging Repository Pattern

The `LoggingRepository` implements a specialized logging pattern for database operations, providing:

- **Performance Monitoring**: Track operation duration
- **Structured Logging**: Consistent log format across all database operations
- **Error Context**: Rich error information with entity and ID context
- **Async Support**: Non-blocking logging for performance-critical operations

```mermaid
classDiagram
    class ILoggingRepository {
        <<interface>>
        +LogDatabaseOperation(operation, entity, id)
        +LogDatabaseError(operation, entity, ex, id)
        +LogDatabaseOperationAsync(operation, entity, dbOperation, id)
    }
    
    class LoggingRepository {
        -ILogger _logger
        +LogDatabaseOperation(operation, entity, id)
        +LogDatabaseError(operation, entity, ex, id)
        +LogDatabaseOperationAsync(operation, entity, dbOperation, id)
    }
    
    class ILogger {
        <<interface>>
        +LogInformation(message, args)
        +LogError(ex, message, args)
    }
    
    ILoggingRepository <|.. LoggingRepository
    LoggingRepository --> ILogger
```

### Logging Configuration Structure

```mermaid
graph LR
    subgraph "Configuration Sources"
        A[appsettings.json]
        B[appsettings.Development.json]
        C[Environment Variables]
    end
    
    subgraph "Serilog Configuration"
        D[Minimum Log Level]
        E[Log Sinks]
        F[Enrichers]
        G[Filters]
    end
    
    subgraph "Log Destinations"
        H[Console]
        I[File System]
        J[Database]
        K[External Services]
    end
    
    A --> D
    B --> D
    C --> D
    
    D --> E
    E --> H
    E --> I
    E --> J
    E --> K
    
    style D fill:#4caf50
    style E fill:#4caf50
    style F fill:#4caf50
    style G fill:#4caf50
```

### Logging Usage Examples

#### 1. **Database Operation Logging**
```csharp
// In Repository Layer
public async Task<User> GetByIdAsync(int id)
{
    return await _loggingRepository.LogDatabaseOperationAsync(
        "SELECT",           // Operation type
        "User",            // Entity name
        async () => await _context.Users.FindAsync(id),  // Database operation
        id                 // Entity ID for context
    );
}
```

#### 2. **Business Operation Logging**
```csharp
// In Service Layer
public async Task<bool> ProcessUserRequest(int userId)
{
    _logger.LogInformation("Processing user request for User ID: {UserId}", userId);
    
    try
    {
        var result = await _userRepository.ProcessRequestAsync(userId);
        _logger.LogInformation("User request processed successfully for User ID: {UserId}", userId);
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process user request for User ID: {UserId}", userId);
        throw;
    }
}
```

#### 3. **Request/Response Logging**
```csharp
// In Controller Layer
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id)
{
    _logger.LogInformation("GET request received for user with ID: {UserId}", id);
    
    try
    {
        var user = await _userService.GetByIdAsync(id);
        _logger.LogInformation("User retrieved successfully. ID: {UserId}, Name: {UserName}", id, user.Name);
        return Ok(user);
    }
    catch (NotFoundException ex)
    {
        _logger.LogWarning("User not found with ID: {UserId}", id);
        return NotFound();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
        return StatusCode(500, "Internal server error");
    }
}
```

### Logging Benefits

| Benefit | Description | Implementation |
|---------|-------------|----------------|
| **🔍 Observability** | Complete visibility into application behavior | Structured logging with context |
| **📊 Performance Monitoring** | Track operation duration and bottlenecks | Automatic timing in LoggingRepository |
| **🐛 Debugging** | Rich context for troubleshooting | Entity IDs, operation types, error details |
| **📈 Analytics** | Log analysis for business insights | Structured format for easy parsing |
| **🔒 Compliance** | Audit trail for regulatory requirements | Complete operation logging |
| **🚀 Production Support** | Real-time monitoring and alerting | Multiple log sinks and formats |

### Log Sinks Configuration

The project supports multiple log destinations:

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 31
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ]
  }
}
```

## 📁 Project Structure

```
OnionArchDemo/
├── 🏛️ Domain/                          # Core Business Logic
│   ├── Contracts/                      # Interfaces & Contracts
│   │   ├── IGenaricRepository.cs      # Generic Repository Interface
│   │   ├── IUnitOfWork.cs             # Unit of Work Interface
│   │   └── SpecificationContracts/    # Specification Pattern
│   ├── Entities/                      # Domain Entities
│   │   ├── BaseEntity.cs              # Base Entity Class
│   │   ├── CoreEntities/              # Business Entities
│   │   └── IdentityEntity/            # Identity Entities
│   └── Exceptions/                    # Domain Exceptions
│
├── ⚙️ Service/                         # Application Services
│   ├── CoreServices/                  # Business Logic Implementation
│   ├── Auto_Mapper_Profile/           # AutoMapper Configurations
│   ├── Exception_Implementation/      # Exception Handling
│   └── Specefication_Implementation/  # Specification Implementation
│
├── 🔧 ServiceAbstraction/             # Service Interfaces
│   └── Class1.cs                      # Service Contracts
│
├── 🗄️ Presistence/                    # Data Access Layer
│   ├── Data/                          # Database Context
│   │   ├── ApplicationDbContext.cs    # EF Core Context
│   │   ├── Configuration/             # Entity Configurations
│   │   └── DataSeed/                  # Database Seeding
│   ├── Repositories/                  # Repository Implementations
│   │   └── GenericRepository.cs       # Generic Repository
│   ├── UnitOfWork/                    # Unit of Work Pattern
│   │   └── UnitOfWork.cs              # Unit of Work Implementation
│   └── SpeceficationEvaluation.cs     # Specification Evaluator
│
├── 🌐 Presentation/                    # API Layer
│   ├── Controllers/                   # API Controllers
│   └── Hubs/                          # SignalR Hubs
│
├── 📦 SharedData/                      # Shared DTOs & Models
│   ├── DTOs/                          # Data Transfer Objects
│   └── Enums/                         # Shared Enumerations
│
└── 🚀 OnionArchDemo/                   # Web API Project
    ├── Controllers/                   # API Controllers
    ├── Program.cs                     # Application Entry Point
    └── appsettings.json              # Configuration
```

## 🚀 Features

### ✅ Implemented Patterns

- **🏗️ Onion Architecture**: Clean separation of concerns
- **📦 Repository Pattern**: Generic repository with specifications
- **🔄 Unit of Work**: Transaction management
- **🔍 Specification Pattern**: Flexible querying
- **🎯 Dependency Injection**: IoC container configuration
- **🗄️ Entity Framework Core**: ORM with SQL Server
- **📋 AutoMapper**: Object-to-object mapping
- **🔒 Exception Handling**: Centralized error management
- **📝 Structured Logging**: Comprehensive logging with Serilog
- **📊 Performance Monitoring**: Database operation timing and metrics

### 🛠️ Technical Stack

- **.NET 8**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core**: Object-relational mapping
- **SQL Server**: Database engine
- **AutoMapper**: Object mapping
- **Swagger/OpenAPI**: API documentation
- **SignalR**: Real-time communication (planned)
- **Serilog**: Structured logging framework
- **Microsoft.Extensions.Logging**: Logging abstraction

## ⚙️ Prerequisites

Before running this project, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🛠️ Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/onionarchdemo.git
cd onionarchdemo
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Database Connection

Update the connection string in `OnionArchDemo/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnionArchDemoDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Run Database Migrations

```bash
cd Presistence
dotnet ef database update
```

## 🏃‍♂️ Running the Application

### Development Mode

```bash
cd OnionArchDemo
dotnet run
```

The application will be available at:
- **API**: https://localhost:7001
- **Swagger UI**: https://localhost:7001/swagger

### Production Mode

```bash
dotnet publish -c Release
dotnet OnionArchDemo.dll
```

## 📊 Database Configuration

### Entity Framework Setup

The project uses Entity Framework Core with the following configuration:

```csharp
// PresistenceLayerConfiguration.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));
```

### Repository Pattern Implementation

```csharp
// Generic Repository with Specification Pattern
public class GenericRepository<T, TK> : IGenaricRepository<T, TK> 
    where T : BaseEntity<TK>
{
    // CRUD operations with specification support
}
```

## 🔧 Configuration

### App Settings Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Dependency Injection Setup

```csharp
// Program.cs
builder.Services.AddPresistenceConfig(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

### Logging Configuration Setup

```csharp
// Program.cs
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341"));
```

The logging configuration supports:
- **Console Logging**: For development and debugging
- **File Logging**: Daily rolling log files with retention
- **Structured Logging**: JSON format for log aggregation tools
- **Performance Monitoring**: Automatic timing for database operations
- **Error Context**: Rich error information with stack traces

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

```
Tests/
├── Domain.Tests/           # Domain layer tests
├── Service.Tests/          # Service layer tests
├── Presistence.Tests/      # Data access tests
└── Integration.Tests/      # Integration tests
```

## 📚 API Documentation

### Swagger UI

Access the interactive API documentation at:
```
https://localhost:7001/swagger
```

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/weatherforecast` | Get weather forecast |
| POST | `/api/entities` | Create new entity |
| GET | `/api/entities/{id}` | Get entity by ID |
| PUT | `/api/entities/{id}` | Update entity |
| DELETE | `/api/entities/{id}` | Delete entity |

## 🏗️ Architecture Benefits

### 1. **Dependency Inversion**
- High-level modules don't depend on low-level modules
- Both depend on abstractions

### 2. **Separation of Concerns**
- Clear boundaries between layers
- Each layer has a specific responsibility

### 3. **Testability**
- Business logic can be tested independently
- Easy to mock dependencies

### 4. **Maintainability**
- Changes in one layer don't affect others
- Easy to understand and modify

### 5. **Scalability**
- Easy to add new features
- Simple to extend existing functionality

## 🔄 Data Flow

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Service
    participant Repository
    participant LoggingRepo
    participant Database
    
    Client->>Controller: HTTP Request
    Note over Controller: Log Request Details
    
    Controller->>Service: Call Business Logic
    Note over Service: Log Business Operation Start
    
    Service->>Repository: Data Access Request
    Repository->>LoggingRepo: Log Database Operation Start
    
    Repository->>Database: Query/Command
    Database-->>Repository: Result
    
    Repository->>LoggingRepo: Log Database Operation Success
    LoggingRepo-->>Repository: Logging Complete
    
    Repository-->>Service: Data
    Note over Service: Log Business Operation Success
    
    Service-->>Controller: Business Result
    Note over Controller: Log Response Details
    
    Controller-->>Client: HTTP Response
```

## 🔍 Logging Best Practices & Troubleshooting

### Best Practices

1. **Use Structured Logging**: Always use structured logging with parameters instead of string concatenation
   ```csharp
   // ✅ Good
   _logger.LogInformation("User {UserId} accessed {Resource}", userId, resourceName);
   
   // ❌ Bad
   _logger.LogInformation("User " + userId + " accessed " + resourceName);
   ```

2. **Log at Appropriate Levels**:
   - **Trace**: Detailed debugging information
   - **Debug**: General debugging information
   - **Information**: General application flow
   - **Warning**: Unexpected but handled situations
   - **Error**: Errors that need immediate attention
   - **Fatal**: Application cannot continue

3. **Include Context**: Always include relevant context (IDs, operation types, entity names)
4. **Performance Considerations**: Use async logging for performance-critical operations
5. **Error Logging**: Always log exceptions with context and stack traces

### Troubleshooting Common Issues

#### Log Files Not Created
- Check if the `logs` directory exists
- Verify write permissions for the application
- Check Serilog configuration in `appsettings.json`

#### Performance Issues
- Ensure logging is not blocking main operations
- Use async logging methods when possible
- Configure appropriate log levels for production

#### Missing Logs
- Verify log level configuration
- Check if logs are being written to different sinks
- Ensure Serilog is properly configured in `Program.cs`

### Log Analysis Tools

- **Seq**: For log aggregation and analysis
- **ELK Stack**: Elasticsearch, Logstash, Kibana
- **Azure Application Insights**: For Azure-hosted applications
- **Custom Scripts**: Parse structured logs for specific metrics

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines

- Follow the existing code style
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting
- Follow logging best practices when adding new features

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Onion Architecture** by Jeffrey Palermo
- **Clean Architecture** by Robert C. Martin
- **.NET Community** for excellent tooling and documentation

---

<div align="center">

**Made with ❤️ for the .NET Community**

[![GitHub stars](https://img.shields.io/github/stars/yourusername/onionarchdemo?style=social)](https://github.com/yourusername/onionarchdemo)
[![GitHub forks](https://img.shields.io/github/forks/yourusername/onionarchdemo?style=social)](https://github.com/yourusername/onionarchdemo)
[![GitHub issues](https://img.shields.io/github/issues/yourusername/onionarchdemo)](https://github.com/yourusername/onionarchdemo/issues)

</div>