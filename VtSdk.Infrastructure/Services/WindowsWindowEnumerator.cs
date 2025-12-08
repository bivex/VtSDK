using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Exceptions;
using VtSdk.Domain.Services;
using VtSdk.Domain.ValueObjects;
using VtSdk.Infrastructure.WindowsApi;

namespace VtSdk.Infrastructure.Services;

/// <summary>
/// Windows-specific implementation of the window enumerator.
/// </summary>
public class WindowsWindowEnumerator : IWindowEnumerator, IDisposable
{
    private readonly IVirtualDesktopManager _virtualDesktopManager;
    private List<Window>? _cachedWindows;
    private DateTime _lastRefresh;
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromSeconds(1);
    private bool _disposed;

    /// <summary>
    /// CLSID for Virtual Desktop Manager.
    /// </summary>
    private static readonly Guid CLSID_VirtualDesktopManager = new("aa509086-5ca9-4c25-8f95-589d3c07b48a");

    /// <summary>
    /// IID for Virtual Desktop Manager.
    /// </summary>
    private static readonly Guid IID_IVirtualDesktopManager = new("a5cd92ff-29be-454c-8d04-d82879fb3f1b");

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsWindowEnumerator"/> class.
    /// </summary>
    public WindowsWindowEnumerator()
    {
        try
        {
            // Create Virtual Desktop Manager for desktop ID queries
            NativeMethods.CoCreateInstance(
                CLSID_VirtualDesktopManager,
                IntPtr.Zero,
                NativeMethods.CLSCTX.CLSCTX_ALL,
                IID_IVirtualDesktopManager,
                out IntPtr virtualDesktopManagerPtr);

            _virtualDesktopManager = (IVirtualDesktopManager)Marshal.GetObjectForIUnknown(virtualDesktopManagerPtr);
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to initialize window enumerator.", ex);
        }
    }

    /// <summary>
    /// Gets all windows currently running on the system.
    /// </summary>
    /// <returns>A collection of all windows.</returns>
    public IReadOnlyCollection<Window> GetAllWindows()
    {
        EnsureCacheValid();
        return _cachedWindows!.AsReadOnly();
    }

    /// <summary>
    /// Gets all windows currently on the specified desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop.</param>
    /// <returns>A collection of windows on the specified desktop.</returns>
    public IReadOnlyCollection<Window> GetWindowsForDesktop(DesktopId desktopId)
    {
        EnsureCacheValid();
        return _cachedWindows!.Where(w => w.DesktopId?.Equals(desktopId) == true).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets a window by its handle.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to retrieve.</param>
    /// <returns>The window with the specified handle, or null if not found.</returns>
    public Window? GetWindowByHandle(WindowHandle windowHandle)
    {
        EnsureCacheValid();
        return _cachedWindows!.FirstOrDefault(w => w.Handle.Equals(windowHandle));
    }

    /// <summary>
    /// Gets windows by their process ID.
    /// </summary>
    /// <param name="processId">The process ID to filter by.</param>
    /// <returns>A collection of windows belonging to the specified process.</returns>
    public IReadOnlyCollection<Window> GetWindowsByProcessId(int processId)
    {
        EnsureCacheValid();
        return _cachedWindows!.Where(w => w.ProcessId == processId).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets windows by their process name.
    /// </summary>
    /// <param name="processName">The process name to filter by.</param>
    /// <returns>A collection of windows belonging to processes with the specified name.</returns>
    public IReadOnlyCollection<Window> GetWindowsByProcessName(string processName)
    {
        EnsureCacheValid();
        return _cachedWindows!.Where(w => w.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets visible windows only.
    /// </summary>
    /// <returns>A collection of visible windows.</returns>
    public IReadOnlyCollection<Window> GetVisibleWindows()
    {
        EnsureCacheValid();
        return _cachedWindows!.Where(w => w.IsVisible).ToList().AsReadOnly();
    }

    /// <summary>
    /// Refreshes the window enumeration cache.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RefreshAsync()
    {
        await Task.Run(() => EnumerateWindows());
    }

    /// <summary>
    /// Ensures the window cache is valid and not stale.
    /// </summary>
    private void EnsureCacheValid()
    {
        if (_cachedWindows == null || (DateTime.Now - _lastRefresh) > _cacheTimeout)
        {
            EnumerateWindows();
        }
    }

    /// <summary>
    /// Enumerates all windows on the system.
    /// </summary>
    private void EnumerateWindows()
    {
        var windows = new List<Window>();
        var desktopWindow = NativeMethods.GetDesktopWindow();
        var shellWindow = NativeMethods.GetShellWindow();

        NativeMethods.EnumWindows((hWnd, lParam) =>
        {
            try
            {
                // Skip desktop and shell windows
                if (hWnd == desktopWindow || hWnd == shellWindow)
                {
                    return true;
                }

                // Skip invisible windows
                if (!NativeMethods.IsWindowVisible(hWnd))
                {
                    return true;
                }

                // Skip windows without captions (tool windows, etc.)
                var style = (NativeMethods.WindowStyles)(long)NativeMethods.GetWindowLongPtr(hWnd, NativeMethods.GetWindowLongIndex.GWL_STYLE);
                if ((style & NativeMethods.WindowStyles.WS_CAPTION) == 0)
                {
                    return true;
                }

                var window = CreateWindowFromHandle(hWnd);
                if (window != null)
                {
                    windows.Add(window);
                }
            }
            catch
            {
                // Skip windows that cause exceptions
            }

            return true;
        }, IntPtr.Zero);

        _cachedWindows = windows;
        _lastRefresh = DateTime.Now;
    }

    /// <summary>
    /// Creates a Window entity from a window handle.
    /// </summary>
    private Window? CreateWindowFromHandle(IntPtr hWnd)
    {
        try
        {
            // Get window title
            var titleLength = NativeMethods.GetWindowTextLength(hWnd);
            if (titleLength == 0)
            {
                return null; // Skip windows without titles
            }

            var titleBuilder = new StringBuilder(titleLength + 1);
            NativeMethods.GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
            var title = titleBuilder.ToString();

            // Get process information
            NativeMethods.GetWindowThreadProcessId(hWnd, out uint processId);
            var processName = GetProcessName((int)processId);

            // Get window state
            var placement = new NativeMethods.WINDOWPLACEMENT { length = Marshal.SizeOf<NativeMethods.WINDOWPLACEMENT>() };
            NativeMethods.GetWindowPlacement(hWnd, ref placement);

            var isMinimized = placement.showCmd == NativeMethods.ShowWindowCommands.SW_SHOWMINIMIZED ||
                             placement.showCmd == NativeMethods.ShowWindowCommands.SW_MINIMIZE ||
                             placement.showCmd == NativeMethods.ShowWindowCommands.SW_SHOWMINNOACTIVE ||
                             placement.showCmd == NativeMethods.ShowWindowCommands.SW_FORCEMINIMIZE;

            var isMaximized = placement.showCmd == NativeMethods.ShowWindowCommands.SW_SHOWMAXIMIZED;

            // Get desktop ID
            var result = _virtualDesktopManager.GetWindowDesktopId(hWnd, out Guid desktopGuid);
            DesktopId? desktopId = result == 0 ? new DesktopId(desktopGuid) : null; // S_OK

            var windowHandle = new WindowHandle(hWnd);
            return new Window(
                windowHandle,
                title,
                processName,
                (int)processId,
                true, // We already filtered for visible windows
                isMinimized,
                isMaximized,
                desktopId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the process name for a given process ID.
    /// </summary>
    private static string GetProcessName(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            return process.ProcessName;
        }
        catch
        {
            return "Unknown";
        }
    }

    /// <summary>
    /// Releases unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
            }

            // Dispose unmanaged resources
            if (_virtualDesktopManager != null)
            {
                Marshal.ReleaseComObject(_virtualDesktopManager);
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~WindowsWindowEnumerator()
    {
        Dispose(false);
    }
}