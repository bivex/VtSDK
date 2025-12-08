using VtSdk.Application.UseCases.Commands;
using VtSdk.Domain.Services;

namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Handler for the SwitchDesktopCommand.
/// </summary>
public class SwitchDesktopCommandHandler
{
    private readonly IDesktopManager _desktopManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchDesktopCommandHandler"/> class.
    /// </summary>
    /// <param name="desktopManager">The desktop manager service.</param>
    public SwitchDesktopCommandHandler(IDesktopManager desktopManager)
    {
        _desktopManager = desktopManager ?? throw new ArgumentNullException(nameof(desktopManager));
    }

    /// <summary>
    /// Handles the switch desktop command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <returns>True if the switch was successful, false otherwise.</returns>
    public async Task<bool> HandleAsync(SwitchDesktopCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return await _desktopManager.SwitchToDesktopAsync(command.DesktopId);
    }
}