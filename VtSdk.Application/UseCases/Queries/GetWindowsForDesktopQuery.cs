using VtSdk.Domain.ValueObjects;

namespace VtSdk.Application.UseCases.Queries;

/// <summary>
/// Query to get all windows for a specific virtual desktop.
/// </summary>
public class GetWindowsForDesktopQuery
{
    /// <summary>
    /// Gets the ID of the desktop to get windows for.
    /// </summary>
    public DesktopId DesktopId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWindowsForDesktopQuery"/> class.
    /// </summary>
    /// <param name="desktopId">The ID of the desktop to get windows for.</param>
    public GetWindowsForDesktopQuery(DesktopId desktopId)
    {
        DesktopId = desktopId ?? throw new ArgumentNullException(nameof(desktopId));
    }
}