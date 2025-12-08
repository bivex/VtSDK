using System.Reflection;
using VtSdk;
using VtSdk.Domain.Entities;

// Test if we can load the VtSdk assembly
Console.WriteLine("Testing VtSdk assembly loading...");
try
{
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "VtSdk", "bin", "Debug", "net8.0", "VtSdk.dll");
    Console.WriteLine($"Loading from: {path}");
    Console.WriteLine($"File exists: {File.Exists(path)}");

    var assembly = Assembly.LoadFrom(path);
    Console.WriteLine($"Assembly loaded: {assembly.FullName}");

    var types = assembly.GetTypes();
    Console.WriteLine($"Found {types.Length} types:");
    foreach (var type in types)
    {
        Console.WriteLine($"  {type.FullName}");
    }

    // Try to find VirtualDesktopManager
    var managerType = assembly.GetType("VtSdk.VirtualDesktopManager");
    if (managerType != null)
    {
        Console.WriteLine("Found VirtualDesktopManager!");
        Console.WriteLine($"Type: {managerType}");
    }
    else
    {
        Console.WriteLine("VirtualDesktopManager not found!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

Console.WriteLine();
Console.WriteLine("Running live demo: create a new desktop, switch for 2s, then return and clean up.");

try
{
    using var manager = new VirtualDesktopManager();

    // Capture current desktop so we can return to it
    VirtualDesktop? currentDesktop = null;
    try
    {
        currentDesktop = manager.GetCurrentDesktop();
    }
    catch (Exception getCurrentEx)
    {
        Console.WriteLine($"GetCurrentDesktop failed: {getCurrentEx.Message}. Will try fallback.");
    }

    // Fallback: choose the first available desktop if current cannot be retrieved
    if (currentDesktop == null)
    {
        var all = manager.GetDesktops();
        if (all.Count > 0)
        {
            currentDesktop = all.OrderBy(d => d.Index).First();
            Console.WriteLine($"Using fallback desktop: {currentDesktop}");
        }
    }

    if (currentDesktop == null)
    {
        Console.WriteLine("No current desktop detected; aborting live demo.");
        return;
    }

    Console.WriteLine($"Current desktop: {currentDesktop}");

    // Create a temporary desktop
    var tempDesktop = await manager.CreateDesktopAsync("SDK demo (temporary)");
    Console.WriteLine($"Created desktop: {tempDesktop}");

    // Switch to the new desktop
    await manager.SwitchToDesktopAsync(tempDesktop.Id);
    Console.WriteLine("Switched to the temporary desktop. Waiting 2 seconds...");
    await Task.Delay(TimeSpan.FromSeconds(2));

    // Switch back to the original desktop
    await manager.SwitchToDesktopAsync(currentDesktop.Id);
    Console.WriteLine("Returned to the original desktop.");

    // Clean up by removing the temporary desktop
    await manager.RemoveDesktopAsync(tempDesktop.Id);
    Console.WriteLine("Removed the temporary desktop. Demo complete.");
}
catch (Exception demoEx)
{
    Console.WriteLine($"Live demo error: {demoEx.Message}");
}