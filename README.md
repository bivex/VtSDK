# Virtual Desktop SDK (VtSdk)

A clean, modern .NET SDK for managing Windows virtual desktops programmatically. Built with Domain-Driven Design (DDD) principles and Clean Architecture.

## 🏗️ Architecture

This SDK follows Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────┐
│         Presentation/API            │ ← Public API, DI setup
│          VtSdk.csproj               │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│         Application Layer           │ ← Use cases, commands, queries
│      VtSdk.Application.csproj       │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│       Domain Layer (Core)           │ ← Business rules, entities, value objects
│        VtSdk.Domain.csproj          │
└─────────────────────────────────────┘
                    │
┌─────────────────────────────────────┐
│       Infrastructure Layer          │ ← Windows API, external dependencies
│    VtSdk.Infrastructure.csproj      │
└─────────────────────────────────────┘
```

## 🎯 What It Does

The Virtual Desktop SDK enables programmatic control of Windows virtual desktops:

- **Get desktop information**: List all desktops, get current desktop, find windows per desktop
- **Switch desktops**: Programmatically switch between virtual desktops
- **Manage desktops**: Create new desktops, remove existing ones
- **Move windows**: Move applications between desktops
- **Monitor changes**: Track desktop switches and window movements

## 🚀 Quick Start

### Basic Usage

```csharp
using VtSdk;

// Create manager instance
using var manager = new VirtualDesktopManager();

// Get all desktops
var desktops = manager.GetDesktops();
foreach (var desktop in desktops)
{
    Console.WriteLine($"{desktop.Name} (Index: {desktop.Index}, Windows: {desktop.WindowCount})");
}

// Get current desktop
var current = manager.GetCurrentDesktop();
Console.WriteLine($"Current desktop: {current?.Name}");

// Switch to next desktop
await manager.SwitchToNextDesktopAsync();

// Create a new desktop
var newDesktop = await manager.CreateDesktopAsync("My Desktop");

// Switch to it
await manager.SwitchToDesktopAsync(newDesktop.Id);

// Move current window to another desktop
await manager.MoveCurrentWindowToDesktopAsync(desktopId);

// Clean up
await manager.RemoveDesktopAsync(newDesktop.Id);
```

### Dependency Injection (ASP.NET Core, WPF, etc.)

```csharp
// In Startup.cs or Program.cs
builder.Services.AddVirtualDesktopSdk();

// In your service
public class MyService
{
    private readonly VirtualDesktopService _desktopService;

    public MyService(VirtualDesktopService desktopService)
    {
        _desktopService = desktopService;
    }

    public async Task DoSomething()
    {
        var desktops = _desktopService.GetDesktops();
        // ... use the service
    }
}
```

## 📋 API Reference

### VirtualDesktopManager

The main entry point for simple usage scenarios.

```csharp
public class VirtualDesktopManager : IDisposable
{
    // Desktop enumeration
    IReadOnlyCollection<VirtualDesktop> GetDesktops()
    VirtualDesktop? GetCurrentDesktop()

    // Desktop switching
    Task<bool> SwitchToDesktopAsync(DesktopId desktopId)
    Task<bool> SwitchToNextDesktopAsync()
    Task<bool> SwitchToPreviousDesktopAsync()

    // Desktop management
    Task<VirtualDesktop> CreateDesktopAsync(string? name = null)
    Task<bool> RemoveDesktopAsync(DesktopId desktopId)

    // Window management
    Task<bool> MoveWindowToDesktopAsync(WindowHandle windowHandle, DesktopId desktopId)
    IReadOnlyCollection<Window> GetWindowsForDesktop(DesktopId desktopId)
    IReadOnlyCollection<Window> GetAllWindows()
}
```

### VirtualDesktopService

Application service for complex scenarios with dependency injection.

```csharp
public class VirtualDesktopService
{
    // All VirtualDesktopManager methods plus additional orchestration
    Task<bool> MoveCurrentWindowToDesktopAsync(DesktopId desktopId)
    // ... additional business logic methods
}
```

### Domain Models

```csharp
// Value Objects (immutable)
public readonly record struct DesktopId(Guid Value);
public readonly record struct WindowHandle(IntPtr Value);

// Entities
public class VirtualDesktop
{
    DesktopId Id { get; }
    string Name { get; }
    int Index { get; }
    bool IsActive { get; }
    IReadOnlyCollection<Window> Windows { get; }
    int WindowCount { get; }
}

public class Window
{
    WindowHandle Handle { get; }
    string Title { get; }
    int ProcessId { get; }
    bool IsVisible { get; }
    bool IsMainWindow { get; }
}
```

## 🔧 Requirements

- **Windows 10 version 1607** or later (virtual desktops introduced)
- **.NET 8.0** or later
- **Windows API access** (runs in user context)

## ⚠️ Limitations

- **Windows-only**: Designed specifically for Windows virtual desktop management
- **User context**: Must run in user session (not system services)
- **API availability**: Some features require Windows 10/11 with virtual desktop support
- **COM dependencies**: Uses Windows COM interfaces that may not be available in all environments

## 🛠️ Building from Source

```bash
# Clone repository
git clone <repository-url>
cd vtSdk

# Build all projects
dotnet build VtSdk.sln

# Run demo
dotnet run --project VtSdk.Demo

# Run tests
dotnet test VtSdk.sln
```

## 🧪 Testing

The SDK includes comprehensive unit tests for domain logic:

```bash
# Run domain tests
dotnet test VtSdk.Domain.Tests

# Run with coverage
dotnet test VtSdk.Domain.Tests --collect:"XPlat Code Coverage"
```

## 📚 Design Principles

This SDK follows modern .NET development best practices:

- **SOLID Principles**: Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
- **Domain-Driven Design**: Rich domain model with value objects, entities, and domain services
- **Clean Architecture**: Dependency flow from outer layers to inner layers
- **Testability**: Dependency injection, interface-based design, pure functions
- **Error Handling**: Custom domain exceptions, proper error propagation
- **Async/Await**: Proper asynchronous programming with cancellation support
- **Resource Management**: IDisposable pattern, proper cleanup
- **Type Safety**: Nullable reference types, immutable types where appropriate

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

[Add your license here]

## 🔗 Related Links

- [Windows Virtual Desktop API Documentation](https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-ivirtualdesktopmanager)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://domainlanguage.com/ddd/)
