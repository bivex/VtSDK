using VtSdk.Application.UseCases.Commands;
using VtSdk.Application.UseCases.Queries;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Services;
using VtSdk.Domain.ValueObjects;

namespace VtSdk.Application.Services;

/// <summary>
/// High-level service for virtual desktop operations.
/// This service orchestrates command and query handlers to provide
/// a clean API for virtual desktop management.
/// </summary>
public class VirtualDesktopService
{
    private readonly IDesktopManager _desktopManager;
    private readonly IWindowEnumerator _windowEnumerator;
    private readonly SwitchDesktopCommandHandler _switchDesktopHandler;
    private readonly CreateDesktopCommandHandler _createDesktopHandler;
    private readonly MoveWindowCommandHandler _moveWindowHandler;
    private readonly GetDesktopsQueryHandler _getDesktopsHandler;
    private readonly GetWindowsForDesktopQueryHandler _getWindowsForDesktopHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualDesktopService"/> class.
    /// </summary>
    /// <param name="desktopManager">The desktop manager service.</param>
    /// <param name="windowEnumerator">The window enumerator service.</param>
    /// <param name="switchDesktopHandler">The switch desktop command handler.</param>
    /// <param name="createDesktopHandler">The create desktop command handler.</param>
    /// <param name="moveWindowHandler">The move window command handler.</param>
    /// <param name="getDesktopsHandler">The get desktops query handler.</param>
    /// <param name="getWindowsForDesktopHandler">The get windows for desktop query handler.</param>
    public VirtualDesktopService(
        IDesktopManager desktopManager,
        IWindowEnumerator windowEnumerator,
        SwitchDesktopCommandHandler switchDesktopHandler,
        CreateDesktopCommandHandler createDesktopHandler,
        MoveWindowCommandHandler moveWindowHandler,
        GetDesktopsQueryHandler getDesktopsHandler,
        GetWindowsForDesktopQueryHandler getWindowsForDesktopHandler)
    {
        _desktopManager = desktopManager ?? throw new ArgumentNullException(nameof(desktopManager));
        _windowEnumerator = windowEnumerator ?? throw new ArgumentNullException(nameof(windowEnumerator));
        _switchDesktopHandler = switchDesktopHandler ?? throw new ArgumentNullException(nameof(switchDesktopHandler));
        _createDesktopHandler = createDesktopHandler ?? throw new ArgumentNullException(nameof(createDesktopHandler));
        _moveWindowHandler = moveWindowHandler ?? throw new ArgumentNullException(nameof(moveWindowHandler));
        _getDesktopsHandler = getDesktopsHandler ?? throw new ArgumentNullException(nameof(getDesktopsHandler));
        _getWindowsForDesktopHandler = getWindowsForDesktopHandler ?? throw new ArgumentNullException(nameof(getWindowsForDesktopHandler));
    }

    /// <summary>
    /// Gets all virtual desktops currently available on the system.
    /// </summary>
    /// <returns>A read-only collection of all virtual desktops, ordered by index.</returns>
    public IReadOnlyCollection<VirtualDesktop> GetDesktops()
    {
        var query = new GetDesktopsQuery();
        return _getDesktopsHandler.Handle(query);
    }

    /// <summary>
    /// Gets the currently active virtual desktop.
    /// </summary>
    /// <returns>The active desktop, or null if no desktop is active or available.</returns>
    public VirtualDesktop? GetCurrentDesktop()
    {
        return _desktopManager.GetCurrentDesktop();
    }

    /// <summary>
    /// Switches to the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to switch to.</param>
    /// <returns>True if the switch was successful, false otherwise.</returns>
    public async Task<bool> SwitchToDesktopAsync(DesktopId desktopId)
    {
        var command = new SwitchDesktopCommand(desktopId);
        return await _switchDesktopHandler.HandleAsync(command);
    }

    /// <summary>
    /// Switches to the next virtual desktop in sequence.
    /// If currently on the last desktop, wraps around to the first desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public async Task<bool> SwitchToNextDesktopAsync()
    {
        return await _desktopManager.SwitchToNextDesktopAsync();
    }

    /// <summary>
    /// Switches to the previous virtual desktop in sequence.
    /// If currently on the first desktop, wraps around to the last desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public async Task<bool> SwitchToPreviousDesktopAsync()
    {
        return await _desktopManager.SwitchToPreviousDesktopAsync();
    }

    /// <summary>
    /// Creates a new virtual desktop with an optional name.
    /// </summary>
    /// <param name="name">Optional display name for the new desktop. If null, a default name will be assigned.</param>
    /// <returns>The newly created virtual desktop.</returns>
    public async Task<VirtualDesktop> CreateDesktopAsync(string? name = null)
    {
        var command = new CreateDesktopCommand(name);
        return await _createDesktopHandler.HandleAsync(command);
    }

    /// <summary>
    /// Removes the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to remove.</param>
    /// <returns>True if the removal was successful, false otherwise.</returns>
    public async Task<bool> RemoveDesktopAsync(DesktopId desktopId)
    {
        return await _desktopManager.RemoveDesktopAsync(desktopId);
    }

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to move.</param>
    /// <param name="desktopId">The unique identifier of the target desktop.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    public async Task<bool> MoveWindowToDesktopAsync(WindowHandle windowHandle, DesktopId desktopId)
    {
        var command = new MoveWindowCommand(windowHandle, desktopId);
        return await _moveWindowHandler.HandleAsync(command);
    }

    /// <summary>
    /// Gets all windows currently running on the system.
    /// </summary>
    /// <returns>A read-only collection of all windows.</returns>
    public IReadOnlyCollection<Window> GetAllWindows()
    {
        return _windowEnumerator.GetAllWindows();
    }

    /// <summary>
    /// Gets all windows currently on the specified desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop.</param>
    /// <returns>A read-only collection of windows on the specified desktop.</returns>
    public IReadOnlyCollection<Window> GetWindowsForDesktop(DesktopId desktopId)
    {
        var query = new GetWindowsForDesktopQuery(desktopId);
        return _getWindowsForDesktopHandler.Handle(query);
    }
}