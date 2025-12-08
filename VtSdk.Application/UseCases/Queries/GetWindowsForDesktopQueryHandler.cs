using VtSdk.Application.UseCases.Queries;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Services;

namespace VtSdk.Application.UseCases.Queries;

/// <summary>
/// Handler for the GetWindowsForDesktopQuery.
/// </summary>
public class GetWindowsForDesktopQueryHandler
{
    private readonly IWindowEnumerator _windowEnumerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWindowsForDesktopQueryHandler"/> class.
    /// </summary>
    /// <param name="windowEnumerator">The window enumerator service.</param>
    public GetWindowsForDesktopQueryHandler(IWindowEnumerator windowEnumerator)
    {
        _windowEnumerator = windowEnumerator ?? throw new ArgumentNullException(nameof(windowEnumerator));
    }

    /// <summary>
    /// Handles the get windows for desktop query.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <returns>A collection of windows on the specified desktop.</returns>
    public IReadOnlyCollection<Window> Handle(GetWindowsForDesktopQuery query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        return _windowEnumerator.GetWindowsForDesktop(query.DesktopId);
    }
}