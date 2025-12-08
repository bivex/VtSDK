using VtSdk.Application.UseCases.Queries;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Services;

namespace VtSdk.Application.UseCases.Queries;

/// <summary>
/// Handler for the GetDesktopsQuery.
/// </summary>
public class GetDesktopsQueryHandler
{
    private readonly IDesktopManager _desktopManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDesktopsQueryHandler"/> class.
    /// </summary>
    /// <param name="desktopManager">The desktop manager service.</param>
    public GetDesktopsQueryHandler(IDesktopManager desktopManager)
    {
        _desktopManager = desktopManager ?? throw new ArgumentNullException(nameof(desktopManager));
    }

    /// <summary>
    /// Handles the get desktops query.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <returns>A collection of all virtual desktops.</returns>
    public IReadOnlyCollection<VirtualDesktop> Handle(GetDesktopsQuery query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        return _desktopManager.GetDesktops();
    }
}