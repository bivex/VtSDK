using System.Reflection;

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