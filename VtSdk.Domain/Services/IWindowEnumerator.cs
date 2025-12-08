using VtSdk.Domain.Entities;
using VtSdk.Domain.ValueObjects;

namespace VtSdk.Domain.Services;

/// <summary>
/// Defines operations for enumerating and managing Windows application windows.
/// </summary>
public interface IWindowEnumerator
{
    /// <summary>
    /// Gets all windows currently running on the system.
    /// </summary>
    /// <returns>A collection of all windows.</returns>
    IReadOnlyCollection<Window> GetAllWindows();

    /// <summary>
    /// Gets all windows currently on the specified desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop.</param>
    /// <returns>A collection of windows on the specified desktop.</returns>
    IReadOnlyCollection<Window> GetWindowsForDesktop(DesktopId desktopId);

    /// <summary>
    /// Gets a window by its handle.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to retrieve.</param>
    /// <returns>The window with the specified handle, or null if not found.</returns>
    Window? GetWindowByHandle(WindowHandle windowHandle);

    /// <summary>
    /// Gets windows by their process ID.
    /// </summary>
    /// <param name="processId">The process ID to filter by.</param>
    /// <returns>A collection of windows belonging to the specified process.</returns>
    IReadOnlyCollection<Window> GetWindowsByProcessId(int processId);

    /// <summary>
    /// Gets windows by their process name.
    /// </summary>
    /// <param name="processName">The process name to filter by.</param>
    /// <returns>A collection of windows belonging to processes with the specified name.</returns>
    IReadOnlyCollection<Window> GetWindowsByProcessName(string processName);

    /// <summary>
    /// Gets visible windows only.
    /// </summary>
    /// <returns>A collection of visible windows.</returns>
    IReadOnlyCollection<Window> GetVisibleWindows();

    /// <summary>
    /// Refreshes the window enumeration cache.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RefreshAsync();
}