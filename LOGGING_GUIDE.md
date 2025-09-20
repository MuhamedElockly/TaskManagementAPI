# Logging System Guide

## Overview
This project now includes a comprehensive logging system that works without MediatR. It provides structured logging with performance tracking and error handling.

## What Gets Logged Automatically

### 1. HTTP Request Logging
- **Every API request** is automatically logged via `app.UseSerilogRequestLogging()`
- **Request details**: Method, URL, status code, duration
- **Location**: Console + `logs/log-YYYYMMDD.txt` files

### 2. Application Startup/Shutdown
- **Configuration loading**
- **Service registration**
- **Database connection status**

## How to Use the Logging Service

### 1. Basic Logging Methods

```csharp
using ServiceAbstraction;

public class MyController : ControllerBase
{
    private readonly ILoggingService _loggingService;

    public MyController(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    public IActionResult MyAction()
    {
        // Information logging
        _loggingService.LogInformation("User {UserId} accessed {Action}", userId, "MyAction");
        
        // Warning logging
        _loggingService.LogWarning("User {UserId} attempted invalid operation", userId);
        
        // Error logging
        try
        {
            // Your code here
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Operation failed for user {UserId}", userId);
        }
        
        return Ok();
    }
}
```

### 2. Operation Logging with Performance Tracking

```csharp
// Synchronous operation
var result = _loggingService.LogOperation("GetUserData", () =>
{
    // Your business logic here
    return userData;
});

// Asynchronous operation
var result = await _loggingService.LogOperationAsync("SaveUserData", async () =>
{
    // Your async business logic here
    await SaveToDatabase(userData);
    return success;
});
```

### 3. Database Operation Logging

```csharp
public class UserRepository
{
    private readonly ILoggingRepository _loggingRepository;

    public async Task<User> GetUserAsync(int userId)
    {
        return await _loggingRepository.LogDatabaseOperationAsync(
            "SELECT", 
            "User", 
            async () => await _context.Users.FindAsync(userId),
            userId
        );
    }
}
```

## Log File Structure

### Location
- **Path**: `Chatty/logs/log-YYYYMMDD.txt`
- **Format**: JSON (structured logging)
- **Rotation**: Daily with size limits

### Sample Log Output
```json
{
  "Timestamp": "2024-12-01T10:30:00.000Z",
  "Level": "Information",
  "MessageTemplate": "Starting operation: {OperationName}",
  "Properties": {
    "OperationName": "GetChatHistory",
    "MachineName": "DESKTOP-ABC123",
    "ThreadId": 1
  }
}
```

## Log Levels

### Information
- **Use for**: Normal operations, user actions, successful operations
- **Example**: "User logged in", "Chat message saved"

### Warning
- **Use for**: Unexpected but handled situations
- **Example**: "Invalid input provided", "Rate limit approaching"

### Error
- **Use for**: Exceptions, failures, system errors
- **Example**: "Database connection failed", "File not found"

### Debug
- **Use for**: Detailed debugging information
- **Example**: "SQL query executed", "Cache hit/miss"

## Best Practices

### 1. Use Structured Logging
```csharp
// ✅ Good - Structured
_loggingService.LogInformation("User {UserId} accessed {Resource}", userId, resourceName);

// ❌ Bad - String concatenation
_loggingService.LogInformation("User " + userId + " accessed " + resourceName);
```

### 2. Include Context
```csharp
// ✅ Good - Rich context
_loggingService.LogInformation("Processing order {OrderId} for user {UserId} with {ItemCount} items", 
    orderId, userId, itemCount);

// ❌ Bad - Minimal context
_loggingService.LogInformation("Processing order");
```

### 3. Log Exceptions Properly
```csharp
// ✅ Good - Include exception object
_loggingService.LogError(ex, "Failed to save user {UserId}", userId);

// ❌ Bad - Only message
_loggingService.LogError("Failed to save user");
```

### 4. Performance Tracking
```csharp
// Use LogOperation for automatic timing
var result = await _loggingService.LogOperationAsync("DatabaseQuery", async () =>
{
    return await _database.QueryAsync(sql);
});
```

## Configuration

### Serilog Settings (appsettings.json)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": true,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  }
}
```

## Troubleshooting

### Logs Not Appearing
1. Check if `logs` folder exists in your application directory
2. Verify Serilog configuration in `Program.cs`
3. Check file permissions for the logs directory

### Performance Issues
1. Use `LogOperation` methods for automatic timing
2. Avoid logging in tight loops
3. Consider log level filtering for production

### File Size Issues
1. Logs rotate daily automatically
2. Check `rollOnFileSizeLimit` setting
3. Monitor disk space in logs directory
