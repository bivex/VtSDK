namespace VtSdk.Application.UseCases.Commands;

/// <summary>
/// Command to create a new virtual desktop.
/// </summary>
public class CreateDesktopCommand
{
    /// <summary>
    /// Gets the optional name for the new desktop.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDesktopCommand"/> class.
    /// </summary>
    /// <param name="name">The optional name for the new desktop.</param>
    public CreateDesktopCommand(string? name = null)
    {
        Name = name;
    }
}