using VtSdk.Domain.Entities;
using VtSdk.Domain.ValueObjects;

namespace VtSdk.Domain.Services;

/// <summary>
/// Defines operations for managing virtual desktops.
/// </summary>
public interface IDesktopManager
{
    /// <summary>
    /// Gets all virtual desktops currently available on the system.
    /// </summary>
    /// <returns>A collection of all virtual desktops.</returns>
    IReadOnlyCollection<VirtualDesktop> GetDesktops();

    /// <summary>
    /// Gets the currently active virtual desktop.
    /// </summary>
    /// <returns>The active desktop, or null if no desktop is active.</returns>
    VirtualDesktop? GetCurrentDesktop();

    /// <summary>
    /// Switches to the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to switch to.</param>
    /// <returns>True if the switch was successful, false otherwise.</returns>
    Task<bool> SwitchToDesktopAsync(DesktopId desktopId);

    /// <summary>
    /// Switches to the next virtual desktop in sequence.
    /// If currently on the last desktop, wraps around to the first desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    Task<bool> SwitchToNextDesktopAsync();

    /// <summary>
    /// Switches to the previous virtual desktop in sequence.
    /// If currently on the first desktop, wraps around to the last desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    Task<bool> SwitchToPreviousDesktopAsync();

    /// <summary>
    /// Creates a new virtual desktop with an optional name.
    /// </summary>
    /// <param name="name">Optional display name for the new desktop.</param>
    /// <returns>The newly created virtual desktop.</returns>
    Task<VirtualDesktop> CreateDesktopAsync(string? name = null);

    /// <summary>
    /// Removes the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to remove.</param>
    /// <returns>True if the removal was successful, false otherwise.</returns>
    Task<bool> RemoveDesktopAsync(DesktopId desktopId);

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to move.</param>
    /// <param name="desktopId">The unique identifier of the target desktop.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    Task<bool> MoveWindowToDesktopAsync(WindowHandle windowHandle, DesktopId desktopId);
}