using VtSdk.Domain.ValueObjects;

namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Command to switch to a specific virtual desktop.
/// </summary>
public class SwitchDesktopCommand
{
    /// <summary>
    /// Gets the ID of the desktop to switch to.
    /// </summary>
    public DesktopId DesktopId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchDesktopCommand"/> class.
    /// </summary>
    /// <param name="desktopId">The ID of the desktop to switch to.</param>
    public SwitchDesktopCommand(DesktopId desktopId)
    {
        DesktopId = desktopId ?? throw new ArgumentNullException(nameof(desktopId));
    }
}