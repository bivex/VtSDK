using Microsoft.Extensions.DependencyInjection;
using VtSdk.Application.Services;
using VtSdk.Application.UseCases.Commands;
using VtSdk.Application.UseCases.Queries;
using VtSdk.Domain.Services;
using VtSdk.Infrastructure.Services;
using VtSdk.Domain.Entities;
using VtSdk.Domain.ValueObjects;

namespace VtSdk;

/// <summary>
/// Provides extension methods and utilities for integrating the Virtual Desktop SDK.
/// </summary>
public static class VirtualDesktopSdk
{
    /// <summary>
    /// Registers all Virtual Desktop SDK services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddVirtualDesktopSdk(this IServiceCollection services)
    {
        // Domain services
        services.AddSingleton<IDesktopManager, WindowsVirtualDesktopManager>();
        services.AddSingleton<IWindowEnumerator, WindowsWindowEnumerator>();

        // Application services
        services.AddTransient<VirtualDesktopService>();

        // Command handlers
        services.AddTransient<SwitchDesktopCommandHandler>();
        services.AddTransient<CreateDesktopCommandHandler>();
        services.AddTransient<MoveWindowCommandHandler>();

        // Query handlers
        services.AddTransient<GetDesktopsQueryHandler>();
        services.AddTransient<GetWindowsForDesktopQueryHandler>();

        return services;
    }

    /// <summary>
    /// Gets the virtual desktop service from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The virtual desktop service instance.</returns>
    public static VirtualDesktopService GetVirtualDesktopService(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<VirtualDesktopService>();
    }
}
/// <summary>
/// A simplified, high-level API for common virtual desktop operations.
/// This class provides easy access to virtual desktop functionality without requiring
/// dependency injection setup. It's suitable for console applications, scripts, and
/// simple desktop applications.
/// </summary>
/// <remarks>
/// <para>This class internally manages all dependencies and provides a synchronous-like
/// API for desktop operations. For more advanced scenarios or dependency injection
/// integration, consider using <see cref="VirtualDesktopService"/> directly.</para>
/// <para>All operations are asynchronous where appropriate to avoid blocking the UI thread.</para>
/// </remarks>
public class VirtualDesktopManager : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly VirtualDesktopService _desktopService;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualDesktopManager"/> class.
    /// </summary>
    /// <exception cref="DesktopOperationException">Thrown when virtual desktop services cannot be initialized.</exception>
    public VirtualDesktopManager()
    {
        var services = new ServiceCollection();
        services.AddVirtualDesktopSdk();
        _serviceProvider = services.BuildServiceProvider();
        _desktopService = _serviceProvider.GetVirtualDesktopService();
    }

    /// <summary>
    /// Gets all virtual desktops currently available on the system.
    /// </summary>
    /// <returns>A read-only collection of all virtual desktops, ordered by index.</returns>
    public IReadOnlyCollection<Domain.Entities.VirtualDesktop> GetDesktops() =>
        _desktopService.GetDesktops();

    /// <summary>
    /// Gets the currently active virtual desktop.
    /// </summary>
    /// <returns>The active desktop, or null if no desktop is active or available.</returns>
    public Domain.Entities.VirtualDesktop? GetCurrentDesktop() =>
        _desktopService.GetCurrentDesktop();

    /// <summary>
    /// Switches to the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to switch to.</param>
    /// <returns>True if the switch was successful, false otherwise.</returns>
    /// <exception cref="DesktopNotFoundException">Thrown when the specified desktop does not exist.</exception>
    /// <exception cref="DesktopOperationException">Thrown when the switch operation fails.</exception>
    public Task<bool> SwitchToDesktopAsync(Domain.ValueObjects.DesktopId desktopId) =>
        _desktopService.SwitchToDesktopAsync(desktopId);

    /// <summary>
    /// Switches to the next virtual desktop in sequence.
    /// If currently on the last desktop, wraps around to the first desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public Task<bool> SwitchToNextDesktopAsync() =>
        _desktopService.SwitchToNextDesktopAsync();

    /// <summary>
    /// Switches to the previous virtual desktop in sequence.
    /// If currently on the first desktop, wraps around to the last desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public Task<bool> SwitchToPreviousDesktopAsync() =>
        _desktopService.SwitchToPreviousDesktopAsync();

    /// <summary>
    /// Creates a new virtual desktop with an optional name.
    /// </summary>
    /// <param name="name">Optional display name for the new desktop. If null, a default name will be assigned.</param>
    /// <returns>The newly created virtual desktop.</returns>
    /// <exception cref="DesktopOperationException">Thrown when desktop creation fails.</exception>
    public Task<Domain.Entities.VirtualDesktop> CreateDesktopAsync(string? name = null) =>
        _desktopService.CreateDesktopAsync(name);

    /// <summary>
    /// Removes the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to remove.</param>
    /// <returns>True if the removal was successful, false otherwise.</returns>
    /// <exception cref="DesktopNotFoundException">Thrown when the specified desktop does not exist.</exception>
    /// <exception cref="InvalidDesktopOperationException">Thrown when attempting to remove the last remaining desktop.</exception>
    public Task<bool> RemoveDesktopAsync(Domain.ValueObjects.DesktopId desktopId) =>
        _desktopService.RemoveDesktopAsync(desktopId);

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to move.</param>
    /// <param name="desktopId">The unique identifier of the target desktop.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    /// <exception cref="WindowNotFoundException">Thrown when the specified window does not exist.</exception>
    /// <exception cref="DesktopNotFoundException">Thrown when the target desktop does not exist.</exception>
    public Task<bool> MoveWindowToDesktopAsync(Domain.ValueObjects.WindowHandle windowHandle, Domain.ValueObjects.DesktopId desktopId) =>
        _desktopService.MoveWindowToDesktopAsync(windowHandle, desktopId);

    /// <summary>
    /// Gets all windows currently running on the system.
    /// </summary>
    /// <returns>A read-only collection of all windows.</returns>
    public IReadOnlyCollection<Domain.Entities.Window> GetAllWindows() =>
        _desktopService.GetAllWindows();

    /// <summary>
    /// Gets all windows currently on the specified desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop.</param>
    /// <returns>A read-only collection of windows on the specified desktop.</returns>
    public IReadOnlyCollection<Domain.Entities.Window> GetWindowsForDesktop(Domain.ValueObjects.DesktopId desktopId) =>
        _desktopService.GetWindowsForDesktop(desktopId);

    /// <summary>
    /// Disposes the manager and releases all unmanaged resources.
    /// </summary>
    /// <remarks>
    /// Call this method when you are finished using the manager to ensure
    /// proper cleanup of Windows API resources.
    /// </remarks>
    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

