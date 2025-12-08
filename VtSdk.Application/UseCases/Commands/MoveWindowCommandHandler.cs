using VtSdk.Application.UseCases.Commands;
using VtSdk.Domain.Services;

namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Handler for the MoveWindowCommand.
/// </summary>
public class MoveWindowCommandHandler
{
    private readonly IDesktopManager _desktopManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveWindowCommandHandler"/> class.
    /// </summary>
    /// <param name="desktopManager">The desktop manager service.</param>
    public MoveWindowCommandHandler(IDesktopManager desktopManager)
    {
        _desktopManager = desktopManager ?? throw new ArgumentNullException(nameof(desktopManager));
    }

    /// <summary>
    /// Handles the move window command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    public async Task<bool> HandleAsync(MoveWindowCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return await _desktopManager.MoveWindowToDesktopAsync(command.WindowHandle, command.DesktopId);
    }
}