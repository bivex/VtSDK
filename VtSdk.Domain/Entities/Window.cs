using VtSdk.Domain.ValueObjects;

namespace VtSdk.Domain.Entities;

/// <summary>
/// Represents a Windows application window.
/// </summary>
public class Window
{
    /// <summary>
    /// Gets the window handle.
    /// </summary>
    public WindowHandle Handle { get; }

    /// <summary>
    /// Gets the title/caption of the window.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the process name of the application that owns the window.
    /// </summary>
    public string ProcessName { get; }

    /// <summary>
    /// Gets the process ID of the application that owns the window.
    /// </summary>
    public int ProcessId { get; }

    /// <summary>
    /// Gets a value indicating whether the window is visible.
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the window is minimized.
    /// </summary>
    public bool IsMinimized { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the window is maximized.
    /// </summary>
    public bool IsMaximized { get; private set; }

    /// <summary>
    /// Gets the ID of the virtual desktop this window is currently on.
    /// </summary>
    public DesktopId? DesktopId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    /// <param name="handle">The window handle.</param>
    /// <param name="title">The window title.</param>
    /// <param name="processName">The process name.</param>
    /// <param name="processId">The process ID.</param>
    /// <param name="isVisible">Whether the window is visible.</param>
    /// <param name="isMinimized">Whether the window is minimized.</param>
    /// <param name="isMaximized">Whether the window is maximized.</param>
    /// <param name="desktopId">The ID of the desktop the window is on.</param>
    public Window(
        WindowHandle handle,
        string title,
        string processName,
        int processId,
        bool isVisible,
        bool isMinimized,
        bool isMaximized,
        DesktopId? desktopId)
    {
        Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ProcessName = processName ?? throw new ArgumentNullException(nameof(processName));
        ProcessId = processId;
        IsVisible = isVisible;
        IsMinimized = isMinimized;
        IsMaximized = isMaximized;
        DesktopId = desktopId;
    }

    /// <summary>
    /// Updates the window title.
    /// </summary>
    /// <param name="title">The new title.</param>
    public void UpdateTitle(string title)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    /// <summary>
    /// Updates the window visibility state.
    /// </summary>
    /// <param name="isVisible">Whether the window is visible.</param>
    public void UpdateVisibility(bool isVisible)
    {
        IsVisible = isVisible;
    }

    /// <summary>
    /// Updates the window minimization state.
    /// </summary>
    /// <param name="isMinimized">Whether the window is minimized.</param>
    public void UpdateMinimizedState(bool isMinimized)
    {
        IsMinimized = isMinimized;
    }

    /// <summary>
    /// Updates the window maximization state.
    /// </summary>
    /// <param name="isMaximized">Whether the window is maximized.</param>
    public void UpdateMaximizedState(bool isMaximized)
    {
        IsMaximized = isMaximized;
    }

    /// <summary>
    /// Updates the desktop ID where this window resides.
    /// </summary>
    /// <param name="desktopId">The new desktop ID.</param>
    public void UpdateDesktopId(DesktopId? desktopId)
    {
        DesktopId = desktopId;
    }

    /// <summary>
    /// Returns a string representation of the window.
    /// </summary>
    /// <returns>A string containing the window's title, handle, and process information.</returns>
    public override string ToString()
    {
        return $"{Title} (Handle: {Handle}, Process: {ProcessName} [{ProcessId}], Visible: {IsVisible})";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Window other && Handle.Equals(other.Handle);

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Handle.GetHashCode();
}