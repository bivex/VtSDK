using System.Reflection;
using System.Runtime.InteropServices;
using VtSdk;
using VtSdk.Domain.Entities;


// Skip manual COM initialization - let .NET handle it automatically
Console.WriteLine ( "Letting .NET handle COM initialization automatically." );

try
{
    // Test if we can load the VtSdk assembly
    Console.WriteLine ( "Testing VtSdk assembly loading..." );
    try
    {
        var path = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "VtSdk", "bin", "Debug", "net8.0", "VtSdk.dll" );
        Console.WriteLine ( $"Loading from: {path}" );
        Console.WriteLine ( $"File exists: {File.Exists(path)}" );

        var assembly = Assembly.LoadFrom ( path );
        Console.WriteLine ( $"Assembly loaded: {assembly.FullName}" );

        var types = assembly.GetTypes();
        Console.WriteLine ( $"Found {types.Length} types:" );
        foreach ( var type in types )
        {
            Console.WriteLine ( $"  {type.FullName}" );
        }

        // Try to find VirtualDesktopManager
        var managerType = assembly.GetType ( "VtSdk.VirtualDesktopManager" );
        if ( managerType != null )
        {
            Console.WriteLine ( "Found VirtualDesktopManager!" );
            Console.WriteLine ( $"Type: {managerType}" );
        }
        else
        {
            Console.WriteLine ( "VirtualDesktopManager not found!" );
        }
    }
    catch ( Exception ex )
    {
        Console.WriteLine ( $"Error: {ex.Message}" );
        Console.WriteLine ( $"Stack trace: {ex.StackTrace}" );
    }

    Console.WriteLine();
    Console.WriteLine ( "Checking Virtual Desktop availability..." );

// Check if Virtual Desktops are enabled
Console.WriteLine("Checking Task View status...");
try
{
    // Check if we can at least create the VirtualDesktopManager
    Console.WriteLine("Attempting to create VirtualDesktopManager...");
    using var manager = new VirtualDesktopManager();
    Console.WriteLine("VirtualDesktopManager created successfully.");

    // Try to get current desktop first (this might work even if GetDesktops fails)
    Console.WriteLine("Attempting to get current desktop...");
    IReadOnlyCollection<VtSdk.Domain.Entities.VirtualDesktop> desktops = null;
    bool desktopsRetrieved = false;

    try
    {
        var currentDesktop = manager.GetCurrentDesktop();
        Console.WriteLine($"Current desktop found: {currentDesktop?.Name ?? "Unnamed"} (ID: {currentDesktop?.Id})");
        Console.WriteLine("Virtual Desktops appear to be available!");

        // If current desktop works, try to get all desktops
        Console.WriteLine("Attempting to get all desktops...");
        desktops = manager.GetDesktops();
        desktopsRetrieved = true;
        Console.WriteLine ( $"Virtual Desktops available: {desktops.Count} desktop(s) found" );
    }
    catch (Exception currentEx)
    {
        Console.WriteLine($"Getting current desktop failed: {currentEx.Message}");

        // Try to get desktops as fallback
        Console.WriteLine("Attempting to get desktops directly...");
        try
        {
            desktops = manager.GetDesktops();
            desktopsRetrieved = true;
            Console.WriteLine ( $"Virtual Desktops available: {desktops.Count} desktop(s) found" );
        }
        catch (Exception desktopsEx)
        {
            Console.WriteLine($"Getting desktops also failed: {desktopsEx.Message}");
            desktopsRetrieved = false;
        }
    }

    if (!desktopsRetrieved || desktops == null || desktops.Count == 0)
    {
        Console.WriteLine();
        Console.WriteLine("No virtual desktops found. This could mean:");
        Console.WriteLine("1. Virtual Desktops are not enabled");
        Console.WriteLine("2. Only one desktop exists (default state)");
        Console.WriteLine("3. The Virtual Desktop service is not available");
        Console.WriteLine();
        Console.WriteLine("To enable Virtual Desktops:");
        Console.WriteLine("- Press Win + Tab to open Task View");
        Console.WriteLine("- Click 'New desktop' button");
        Console.WriteLine("- Or go to Settings > System > Multitasking > Virtual desktops");
        Console.WriteLine();
        Console.WriteLine("After enabling, restart your computer if needed.");
        return;
    }

    foreach ( var desktop in desktops.OrderBy ( d => d.Index ) )
    {
        Console.WriteLine ( $"  Desktop {desktop.Index}: {desktop.Name ?? "Unnamed"} (ID: {desktop.Id})" );
    }
}
catch ( Exception checkEx )
{
    Console.WriteLine ( $"Virtual Desktop check failed: {checkEx.Message}" );
    Console.WriteLine ( $"Exception type: {checkEx.GetType().Name}" );

    if ( checkEx.InnerException != null )
    {
        Console.WriteLine ( $"Inner exception: {checkEx.InnerException.Message}" );
        Console.WriteLine ( $"Inner exception type: {checkEx.InnerException.GetType().Name}" );
    }

    Console.WriteLine();
    Console.WriteLine("Troubleshooting steps:");
    Console.WriteLine("1. Ensure you're running Windows 10 Pro or Enterprise");
    Console.WriteLine("2. Enable Virtual Desktops: Win + Tab > New desktop");
    Console.WriteLine("3. Check Windows Settings > System > Multitasking > Virtual desktops");
    Console.WriteLine("4. Restart your computer if Virtual Desktops were just enabled");
    return;
}

    Console.WriteLine();
    Console.WriteLine ( "Running live demo: create a new desktop, switch for 2s, then return and clean up." );

    try
    {
        using var manager = new VirtualDesktopManager();

        // Capture current desktop so we can return to it
        VirtualDesktop? currentDesktop = null;
        try
        {
            currentDesktop = manager.GetCurrentDesktop();
        }
        catch ( Exception getCurrentEx )
        {
            Console.WriteLine ( $"GetCurrentDesktop failed: {getCurrentEx.Message}. Will try fallback." );
        }

        // Fallback: choose the first available desktop if current cannot be retrieved
        if ( currentDesktop == null )
        {
            var all = manager.GetDesktops();
            if ( all.Count > 0 )
            {
                currentDesktop = all.OrderBy ( d => d.Index ).First();
                Console.WriteLine ( $"Using fallback desktop: {currentDesktop}" );
            }
        }

        if ( currentDesktop == null )
        {
            Console.WriteLine ( "No current desktop detected; aborting live demo." );
            return;
        }

        Console.WriteLine ( $"Current desktop: {currentDesktop}" );

        // Create a temporary desktop
        var tempDesktop = await manager.CreateDesktopAsync ( "SDK demo (temporary)" );
        Console.WriteLine ( $"Created desktop: {tempDesktop}" );

        // Switch to the new desktop
        await manager.SwitchToDesktopAsync ( tempDesktop.Id );
        Console.WriteLine ( "Switched to the temporary desktop. Waiting 2 seconds..." );
        await Task.Delay ( TimeSpan.FromSeconds ( 2 ) );

        // Switch back to the original desktop
        await manager.SwitchToDesktopAsync ( currentDesktop.Id );
        Console.WriteLine ( "Returned to the original desktop." );

        // Clean up by removing the temporary desktop
        await manager.RemoveDesktopAsync ( tempDesktop.Id );
        Console.WriteLine ( "Removed the temporary desktop. Demo complete." );
    }
    catch ( Exception demoEx )
    {
        Console.WriteLine ( $"Live demo error: {demoEx.Message}" );
    }
}
finally
{
    // COM uninitialization handled automatically by .NET
    Console.WriteLine ( "Demo completed." );
}

// P/Invoke for COM initialization
internal static class ComHelper
{
    [DllImport("ole32.dll")]
    public static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

    [DllImport("ole32.dll")]
    public static extern void CoUninitialize();

    public const uint COINIT_APARTMENTTHREADED = 0x2; // STA
    public const uint COINIT_MULTITHREADED = 0x0;     // MTA
}