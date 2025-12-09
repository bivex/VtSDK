# Virtual Desktop SDK (VtSdk)

## Front Matter

**Title:** Virtual Desktop SDK (VtSdk) User Guide and Developer Documentation

**Version:** 1.0.0

**Date:** December 9, 2025

**Authors:** VtSdk Development Team

**Revision History:**

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0.0 | 2025-12-09 | VtSdk Team | Initial release of user documentation |

## Introduction

### Purpose

This document provides comprehensive guidance for developers and system administrators who need to integrate Windows virtual desktop management capabilities into their .NET applications. It covers installation, configuration, usage, troubleshooting, and maintenance of the Virtual Desktop SDK.

### Scope

This guide covers:
- Installation and setup of the VtSdk library
- Basic and advanced usage patterns
- API reference and domain models
- Troubleshooting common issues
- Best practices for integration

This guide does not cover:
- Windows operating system administration
- Advanced Windows API programming
- Custom Windows virtual desktop implementations

### Target Audience

**Primary Audience:**
- .NET developers (intermediate to advanced level)
- Application architects designing desktop management features

**Secondary Audience:**
- System administrators managing Windows environments
- Quality assurance engineers testing virtual desktop functionality

**Prerequisites:**
- Proficiency in C# and .NET development
- Understanding of asynchronous programming patterns
- Basic knowledge of Windows operating system concepts
- Experience with dependency injection frameworks (recommended)

### Referenced Documents

- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Windows Virtual Desktop API Reference](https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-ivirtualdesktopmanager)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Concept of Operations

### System Overview

The Virtual Desktop SDK (VtSdk) is a .NET library that provides programmatic access to Windows virtual desktop management capabilities. It enables applications to:

- Enumerate and monitor virtual desktops
- Switch between desktops programmatically
- Create and remove virtual desktops
- Move application windows between desktops
- Track desktop and window state changes

### Typical Usage Scenarios

**Desktop Management Applications:**
Developers building custom desktop managers or virtual desktop enhancers can use VtSdk to create sophisticated user interfaces for desktop organization.

**Automation Tools:**
System administrators can build scripts and tools to automate desktop management tasks, such as organizing applications across multiple desktops based on user preferences or workflows.

**Productivity Applications:**
Applications that enhance user productivity can integrate virtual desktop switching capabilities, such as hotkey managers or workflow automation tools.

**Testing Frameworks:**
QA teams can use the SDK to create automated tests that verify application behavior across different virtual desktop configurations.

### Operating Environment

**Supported Platforms:**
- Windows 10 version 1607 or later
- Windows 11 (all versions)
- .NET 8.0 or later

**Execution Context:**
- Must run in user session (not as a system service)
- Requires access to Windows desktop APIs
- COM components must be available and registered

**Resource Requirements:**
- Minimal memory footprint (< 10MB)
- No significant CPU usage during normal operation
- Network access not required for core functionality

## Installation and Configuration

### System Requirements

**Hardware Requirements:**
- x64 processor architecture
- 100 MB available disk space
- 512 MB RAM (additional for applications using the SDK)

**Software Requirements:**
- Windows 10 version 1607 (Build 14393) or later
- .NET 8.0 Runtime or .NET 8.0 SDK (for development)
- Windows Virtual Desktop feature enabled

### Installation Procedure

#### Option 1: NuGet Package Installation

**Preconditions:**
- .NET 8.0 SDK installed
- Access to NuGet package repository
- Project targets .NET 8.0 or later

**Steps:**

1. Open your .NET project in Visual Studio or your preferred IDE.
2. Open the Package Manager Console (Tools → NuGet Package Manager → Package Manager Console).
3. Execute the following command:
   ```
   Install-Package VtSdk
   ```
4. Alternatively, use the .NET CLI:
   ```
   dotnet add package VtSdk
   ```

**Post-conditions:**
- VtSdk package is added to your project dependencies
- All required assemblies are available in your project

#### Option 2: Building from Source

**Preconditions:**
- Git client installed
- .NET 8.0 SDK installed
- Access to the VtSdk repository

**Steps:**

1. Clone the repository:
   ```
   git clone <repository-url>
   cd vtSdk
   ```

2. Build all projects:
   ```
   dotnet build VtSdk.sln
   ```

3. (Optional) Run the demonstration application:
   ```
   dotnet run --project VtSdk.Demo
   ```

**Post-conditions:**
- All projects compile successfully
- Demo application runs and demonstrates basic functionality

### Configuration

#### Basic Configuration

No additional configuration is required for basic usage. The SDK automatically detects and adapts to the Windows environment.

#### Dependency Injection Setup (Recommended)

**For ASP.NET Core applications:**

```csharp
// In Program.cs or Startup.cs
using VtSdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add VtSdk services
builder.Services.AddVirtualDesktopSdk();

// Configure options if needed
builder.Services.Configure<VirtualDesktopOptions>(options =>
{
    options.DefaultDesktopName = "Main Desktop";
    options.EnableEventMonitoring = true;
});

var app = builder.Build();
```

**For other DI containers:**

```csharp
using VtSdk;

// Register services manually
var services = new ServiceCollection();
services.AddSingleton<IVirtualDesktopManager, WindowsVirtualDesktopManager>();
services.AddSingleton<VirtualDesktopService>();

var provider = services.BuildServiceProvider();
```

### Verification

**Procedure: Verify Installation**

1. Create a new console application.
2. Add the VtSdk package reference.
3. Add the following test code:

```csharp
using VtSdk;

try
{
    using var manager = new VirtualDesktopManager();
    var desktops = manager.GetDesktops();
    Console.WriteLine($"Found {desktops.Count} virtual desktops");
    Console.WriteLine("Installation successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Installation failed: {ex.Message}");
}
```

4. Run the application.
5. Verify that the application detects virtual desktops without errors.

## Procedures

### Basic Desktop Operations

#### Procedure: Enumerate Virtual Desktops

**Purpose:** Retrieve information about all available virtual desktops.

**Preconditions:**
- Windows virtual desktops are enabled
- Application has access to desktop APIs
- VtSdk is properly initialized

**Steps:**

1. Create a `VirtualDesktopManager` instance:
   ```csharp
   using var manager = new VirtualDesktopManager();
   ```

2. Call the `GetDesktops()` method:
   ```csharp
   var desktops = manager.GetDesktops();
   ```

3. Iterate through the results:
   ```csharp
   foreach (var desktop in desktops)
   {
       Console.WriteLine($"Desktop: {desktop.Name}");
       Console.WriteLine($"Index: {desktop.Index}");
       Console.WriteLine($"Windows: {desktop.WindowCount}");
       Console.WriteLine($"Active: {desktop.IsActive}");
   }
   ```

**Expected Results:**
- List of all virtual desktops with their properties
- Current desktop marked as active

**Error Conditions:**
- `VirtualDesktopException` if Windows APIs are unavailable
- Empty collection if virtual desktops are disabled

#### Procedure: Switch Between Desktops

**Purpose:** Programmatically switch to a different virtual desktop.

**Preconditions:**
- Target desktop exists and is accessible
- Application has permission to switch desktops
- VtSdk manager is initialized

**Steps:**

1. Obtain the target desktop identifier:
   ```csharp
   var desktops = manager.GetDesktops();
   var targetDesktop = desktops.FirstOrDefault(d => d.Name.Contains("Work"));
   if (targetDesktop == null) return;
   ```

2. Switch to the target desktop:
   ```csharp
   bool success = await manager.SwitchToDesktopAsync(targetDesktop.Id);
   ```

3. Verify the switch (optional):
   ```csharp
   var current = manager.GetCurrentDesktop();
   Console.WriteLine($"Current desktop: {current?.Name}");
   ```

**Expected Results:**
- Desktop switch completes successfully
- User interface updates to show the new desktop
- Return value is `true`

**Error Conditions:**
- `ArgumentException` if desktop ID is invalid
- `VirtualDesktopException` if switch operation fails
- `false` return value indicates operation failed

#### Procedure: Create New Desktop

**Purpose:** Create a new virtual desktop with optional custom name.

**Preconditions:**
- Windows virtual desktops are enabled
- User has permission to create desktops
- System has capacity for additional desktops

**Steps:**

1. Prepare desktop creation parameters:
   ```csharp
   string desktopName = "Development Workspace";
   ```

2. Create the new desktop:
   ```csharp
   var newDesktop = await manager.CreateDesktopAsync(desktopName);
   ```

3. (Optional) Switch to the new desktop:
   ```csharp
   await manager.SwitchToDesktopAsync(newDesktop.Id);
   ```

4. Verify creation:
   ```csharp
   Console.WriteLine($"Created desktop: {newDesktop.Name} (ID: {newDesktop.Id})");
   ```

**Expected Results:**
- New desktop is created and available
- Desktop appears in system taskbar
- `VirtualDesktop` object returned with valid properties

**Error Conditions:**
- `VirtualDesktopException` if creation fails
- System limit reached (typically 20 desktops)

#### Procedure: Move Window to Different Desktop

**Purpose:** Move an application window from one virtual desktop to another.

**Preconditions:**
- Source window exists and is accessible
- Target desktop exists
- Application has permission to manipulate windows

**Steps:**

1. Identify the window to move:
   ```csharp
   // Get all windows
   var allWindows = manager.GetAllWindows();

   // Find specific window by title
   var targetWindow = allWindows.FirstOrDefault(w =>
       w.Title.Contains("Visual Studio"));
   if (targetWindow == null) return;
   ```

2. Select target desktop:
   ```csharp
   var desktops = manager.GetDesktops();
   var targetDesktop = desktops.FirstOrDefault(d => d.Name.Contains("Dev"));
   if (targetDesktop == null) return;
   ```

3. Move the window:
   ```csharp
   bool success = await manager.MoveWindowToDesktopAsync(
       targetWindow.Handle, targetDesktop.Id);
   ```

**Expected Results:**
- Window moves to target desktop
- Window disappears from current desktop
- Return value is `true`

**Error Conditions:**
- `ArgumentException` if window handle or desktop ID is invalid
- `VirtualDesktopException` if move operation fails
- `false` return value indicates operation failed

### Advanced Operations

#### Procedure: Monitor Desktop Changes

**Purpose:** Track changes to virtual desktop state and windows.

**Preconditions:**
- Event monitoring enabled in configuration
- Application can handle asynchronous events

**Steps:**

1. Set up event handlers:
   ```csharp
   manager.DesktopCreated += (sender, args) =>
   {
       Console.WriteLine($"Desktop created: {args.Desktop.Name}");
   };

   manager.DesktopDestroyed += (sender, args) =>
   {
       Console.WriteLine($"Desktop destroyed: {args.DesktopId}");
   };

   manager.DesktopChanged += (sender, args) =>
   {
       Console.WriteLine($"Switched to desktop: {args.NewDesktop.Name}");
   };
   ```

2. Enable monitoring:
   ```csharp
   manager.EnableEventMonitoring();
   ```

3. Keep application running to receive events:
   ```csharp
   await Task.Delay(Timeout.Infinite);
   ```

**Expected Results:**
- Events fired when desktops are created, destroyed, or switched
- Event arguments contain relevant desktop information

## Troubleshooting and Error Handling

### Common Issues

#### Issue: VirtualDesktopException on Initialization

**Symptoms:**
- Exception thrown when creating `VirtualDesktopManager`
- Message indicates Windows API unavailable

**Possible Causes:**
- Windows virtual desktops not enabled
- Running as system service instead of user session
- COM components not registered

**Resolution Steps:**

1. Verify Windows version supports virtual desktops (Windows 10 1607+).
2. Ensure virtual desktops are enabled in system settings.
3. Confirm application runs in user session, not as service.
4. Try running as administrator if permission issues suspected.

#### Issue: Desktop Operations Fail Silently

**Symptoms:**
- Methods return `false` without throwing exceptions
- Desktop state doesn't change as expected

**Possible Causes:**
- Windows is in a restricted state
- Another application is managing desktops
- System policy restrictions

**Resolution Steps:**

1. Check Windows Event Viewer for related errors.
2. Verify no other desktop management software is running.
3. Test with minimal code to isolate the issue.
4. Restart Windows Explorer if desktop shell issues suspected.

#### Issue: Window Move Operations Fail

**Symptoms:**
- `MoveWindowToDesktopAsync` returns `false`
- Window remains on current desktop

**Possible Causes:**
- Window belongs to system process
- Window has special protection (UAC dialogs, etc.)
- Target desktop no longer exists

**Resolution Steps:**

1. Verify window handle is valid and window still exists.
2. Check if window belongs to current user session.
3. Ensure target desktop still exists.
4. Try moving different windows to test if issue is window-specific.

### Error Messages and Meanings

| Error Message | Meaning | Resolution |
|---------------|---------|------------|
| `VirtualDesktopNotSupportedException` | Windows version doesn't support virtual desktops | Upgrade to Windows 10 1607+ or Windows 11 |
| `ComInitializationException` | COM components failed to initialize | Restart application or system |
| `AccessDeniedException` | Insufficient permissions | Run as administrator or check user rights |
| `InvalidDesktopIdException` | Desktop identifier is invalid | Refresh desktop list and retry |
| `WindowNotFoundException` | Window handle is invalid | Refresh window list and retry |

### Support and Escalation

**Community Support:**
- GitHub Issues: Report bugs and request features
- Stack Overflow: Ask questions with `vtsdk` tag

**Professional Support:**
- Contact the development team for enterprise support options
- Check documentation repository for known issues and workarounds

## Information for Uninstallation

### When to Uninstall

Uninstallation may be necessary when:
- Removing the SDK from development environment
- Upgrading to incompatible version
- Troubleshooting integration issues
- Cleaning up unused dependencies

### Uninstallation Procedure

#### Option 1: Remove NuGet Package

**Preconditions:**
- Project is open in development environment
- No code dependencies on VtSdk remain

**Steps:**

1. Open Package Manager Console in Visual Studio.
2. Execute uninstall command:
   ```
   Uninstall-Package VtSdk
   ```
3. Alternatively, use .NET CLI:
   ```
   dotnet remove package VtSdk
   ```
4. Clean and rebuild project:
   ```
   dotnet clean
   dotnet build
   ```

**Post-conditions:**
- VtSdk package removed from project
- No references to VtSdk assemblies

#### Option 2: Remove Source Installation

**Preconditions:**
- Source code repository cloned locally
- No other projects depend on local VtSdk build

**Steps:**

1. Remove cloned repository:
   ```
   rd /s vtSdk
   ```
2. Clean NuGet cache (optional):
   ```
   dotnet nuget locals all --clear
   ```
3. Remove any local package references.

**Post-conditions:**
- All VtSdk source code and build artifacts removed
- System ready for clean reinstallation if needed

### Data Cleanup

The VtSdk library does not create persistent system data. However, applications using VtSdk may have stored configuration data that should be cleaned up:

1. Remove application configuration files containing VtSdk settings.
2. Clear any cached desktop state information.
3. Reset application preferences to defaults.

### Verification

**Procedure: Verify Uninstallation**

1. Attempt to build project that previously used VtSdk.
2. Verify compilation errors indicate missing VtSdk references.
3. Confirm no VtSdk assemblies in output directory.

## Appendices

### Appendix A: API Reference

#### VirtualDesktopManager Class

Main entry point for simple usage scenarios.

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

    // Event monitoring
    void EnableEventMonitoring()
    event EventHandler<DesktopEventArgs> DesktopCreated
    event EventHandler<DesktopEventArgs> DesktopDestroyed
    event EventHandler<DesktopChangedEventArgs> DesktopChanged
}
```

#### VirtualDesktopService Class

Application service for complex scenarios with dependency injection.

```csharp
public class VirtualDesktopService
{
    // All VirtualDesktopManager methods plus orchestration
    Task<bool> MoveCurrentWindowToDesktopAsync(DesktopId desktopId)

    // Additional business logic methods
    Task<VirtualDesktop> GetOrCreateDesktopAsync(string name)
    Task<bool> OrganizeWindowsByApplicationAsync()
}
```

### Appendix B: Domain Models

#### Value Objects

```csharp
public readonly record struct DesktopId(Guid Value);
public readonly record struct WindowHandle(IntPtr Value);
```

#### Entities

```csharp
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

### Appendix C: Glossary

**COM (Component Object Model):** Microsoft's framework for developing software components that can interact with each other.

**Dependency Injection:** A design pattern where dependencies are provided to a class rather than the class creating them itself.

**Domain-Driven Design (DDD):** An approach to software development that centers the development on programming a domain model.

**NuGet:** The package manager for .NET that makes it easy to install and update libraries and tools.

**Virtual Desktop:** A feature in Windows that allows users to organize windows and applications across multiple desktop spaces.

**Window Handle:** A unique identifier assigned to each window by the Windows operating system.

### Appendix D: Index

**API Reference:** See Appendix A

**Configuration:** See Installation and Configuration section

**Creating desktops:** See Procedure: Create New Desktop

**Dependency injection:** See Installation and Configuration section

**Desktop enumeration:** See Procedure: Enumerate Virtual Desktops

**Desktop switching:** See Procedure: Switch Between Desktops

**Error handling:** See Troubleshooting and Error Handling section

**Event monitoring:** See Procedure: Monitor Desktop Changes

**Installation:** See Installation and Configuration section

**Moving windows:** See Procedure: Move Window to Different Desktop

**Troubleshooting:** See Troubleshooting and Error Handling section

**Uninstallation:** See Information for Uninstallation section
