using VtSdk.Domain.Services;

namespace VtSdk.Infrastructure.Services;

/// <summary>
/// Simple console-based logger implementation.
/// </summary>
public class ConsoleLogger : VtSdk.Domain.Services.ILogger
{
    private readonly string _category;

    public ConsoleLogger(string category = "VtSdk")
    {
        _category = category;
    }

    public void LogInformation(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] INFO {_category}: {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] WARN {_category}: {message}");
    }

    public void LogError(string message, Exception? exception = null)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR {_category}: {message}");
        if (exception != null)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR {_category}: Exception: {exception.Message}");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR {_category}: StackTrace: {exception.StackTrace}");
        }
    }

    public void LogDebug(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] DEBUG {_category}: {message}");
    }
}
