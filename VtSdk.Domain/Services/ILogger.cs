namespace VtSdk.Domain.Services;

/// <summary>
/// Simple logging interface for the Virtual Desktop SDK.
/// </summary>
public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
    void LogDebug(string message);
}