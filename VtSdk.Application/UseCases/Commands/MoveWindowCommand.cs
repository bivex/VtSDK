using VtSdk.Domain.ValueObjects;

namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Command to move a window to a specific virtual desktop.
/// </summary>
public class MoveWindowCommand
{
    /// <summary>
    /// Gets the handle of the window to move.
    /// </summary>
    public WindowHandle WindowHandle { get; }

    /// <summary>
    /// Gets the ID of the target desktop.
    /// </summary>
    public DesktopId DesktopId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveWindowCommand"/> class.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to move.</param>
    /// <param name="desktopId">The ID of the target desktop.</param>
    public MoveWindowCommand(WindowHandle windowHandle, DesktopId desktopId)
    {
        WindowHandle = windowHandle ?? throw new ArgumentNullException(nameof(windowHandle));
        DesktopId = desktopId ?? throw new ArgumentNullException(nameof(desktopId));
    }
}