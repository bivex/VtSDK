using VtSdk.Application.UseCases.Commands;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Services;

namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Handler for the CreateDesktopCommand.
/// </summary>
public class CreateDesktopCommandHandler
{
    private readonly IDesktopManager _desktopManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDesktopCommandHandler"/> class.
    /// </summary>
    /// <param name="desktopManager">The desktop manager service.</param>
    public CreateDesktopCommandHandler(IDesktopManager desktopManager)
    {
        _desktopManager = desktopManager ?? throw new ArgumentNullException(nameof(desktopManager));
    }

    /// <summary>
    /// Handles the create desktop command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <returns>The newly created virtual desktop.</returns>
    public async Task<VirtualDesktop> HandleAsync(CreateDesktopCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return await _desktopManager.CreateDesktopAsync(command.Name);
    }
}