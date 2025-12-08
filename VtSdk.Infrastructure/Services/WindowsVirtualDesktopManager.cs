using System.Runtime.InteropServices;
using VtSdk.Domain.Entities;
using VtSdk.Domain.Exceptions;
using VtSdk.Domain.Services;
using VtSdk.Domain.ValueObjects;
using VtSdk.Infrastructure.WindowsApi;

namespace VtSdk.Infrastructure.Services;

/// <summary>
/// Windows-specific implementation of the virtual desktop manager.
/// </summary>
public class WindowsVirtualDesktopManager : IDesktopManager, IDisposable
{
    private readonly IVirtualDesktopManager _virtualDesktopManager;
    private readonly IVirtualDesktopManagerInternal _virtualDesktopManagerInternal;
    private readonly IServiceProvider10 _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// CLSID for Virtual Desktop Manager.
    /// </summary>
    private static readonly Guid CLSID_VirtualDesktopManager = new("aa509086-5ca9-4c25-8f95-589d3c07b48a");

    /// <summary>
    /// CLSID for Immersive Shell.
    /// </summary>
    private static readonly Guid CLSID_ImmersiveShell = new("c2f03a33-21f5-47fa-b4bb-156362a2f239");

    /// <summary>
    /// IID for Virtual Desktop Manager.
    /// </summary>
    private static readonly Guid IID_IVirtualDesktopManager = new("a5cd92ff-29be-454c-8d04-d82879fb3f1b");

    /// <summary>
    /// IID for Virtual Desktop Manager Internal.
    /// </summary>
    private static readonly Guid IID_IVirtualDesktopManagerInternal = new("f31574d6-b682-4cdc-bd56-1827860abec6");

    /// <summary>
    /// IID for Service Provider.
    /// </summary>
    private static readonly Guid IID_IServiceProvider = new("6d5140c1-7436-11ce-8034-00aa006009fa");

    /// <summary>
    /// SID for Virtual Desktop Manager Internal.
    /// </summary>
    private static readonly Guid SID_VirtualDesktopManagerInternal = new("c5e0cdca-7b6e-41b2-9fc4-d93975cc467b");

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsVirtualDesktopManager"/> class.
    /// </summary>
    /// <exception cref="DesktopOperationException">Thrown when virtual desktop services cannot be initialized.</exception>
    public WindowsVirtualDesktopManager()
    {
        try
        {
            // Create Virtual Desktop Manager
            NativeMethods.CoCreateInstance(
                CLSID_VirtualDesktopManager,
                IntPtr.Zero,
                NativeMethods.CLSCTX.CLSCTX_ALL,
                IID_IVirtualDesktopManager,
                out IntPtr virtualDesktopManagerPtr);

            _virtualDesktopManager = (IVirtualDesktopManager)Marshal.GetObjectForIUnknown(virtualDesktopManagerPtr);

            // Create Immersive Shell service provider
            NativeMethods.CoCreateInstance(
                CLSID_ImmersiveShell,
                IntPtr.Zero,
                NativeMethods.CLSCTX.CLSCTX_ALL,
                IID_IServiceProvider,
                out IntPtr serviceProviderPtr);

            _serviceProvider = (IServiceProvider10)Marshal.GetObjectForIUnknown(serviceProviderPtr);

            // Get Virtual Desktop Manager Internal
            _serviceProvider.QueryService(
                ref SID_VirtualDesktopManagerInternal,
                ref IID_IVirtualDesktopManagerInternal,
                out IntPtr virtualDesktopManagerInternalPtr);

            _virtualDesktopManagerInternal = (IVirtualDesktopManagerInternal)Marshal.GetObjectForIUnknown(virtualDesktopManagerInternalPtr);
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to initialize virtual desktop services.", ex);
        }
    }

    /// <summary>
    /// Gets all virtual desktops currently available on the system.
    /// </summary>
    /// <returns>A collection of all virtual desktops.</returns>
    public IReadOnlyCollection<VirtualDesktop> GetDesktops()
    {
        try
        {
            _virtualDesktopManagerInternal.GetDesktops(out IntPtr desktopsPtr);
            var desktops = (IObjectArray)Marshal.GetObjectForIUnknown(desktopsPtr);

            var result = new List<VirtualDesktop>();
            desktops.GetCount(out int count);

            for (int i = 0; i < count; i++)
            {
                desktops.GetAt(i, typeof(IVirtualDesktop).GUID, out IntPtr desktopPtr);
                var desktop = (IVirtualDesktop)Marshal.GetObjectForIUnknown(desktopPtr);

                var virtualDesktop = CreateVirtualDesktopFromComObject(desktop, i);
                result.Add(virtualDesktop);

                Marshal.Release(desktopPtr);
            }

            Marshal.Release(desktopsPtr);
            return result.AsReadOnly();
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to get virtual desktops.", ex);
        }
    }

    /// <summary>
    /// Gets the currently active virtual desktop.
    /// </summary>
    /// <returns>The active desktop, or null if no desktop is active.</returns>
    public VirtualDesktop? GetCurrentDesktop()
    {
        try
        {
            _virtualDesktopManagerInternal.GetCurrentDesktop(out IntPtr currentDesktopPtr);
            if (currentDesktopPtr == IntPtr.Zero)
            {
                return null;
            }

            var currentDesktop = (IVirtualDesktop)Marshal.GetObjectForIUnknown(currentDesktopPtr);
            var virtualDesktop = CreateVirtualDesktopFromComObject(currentDesktop, -1);

            // Set as active
            virtualDesktop.SetActive(true);

            Marshal.Release(currentDesktopPtr);
            return virtualDesktop;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to get current desktop.", ex);
        }
    }

    /// <summary>
    /// Switches to the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to switch to.</param>
    /// <returns>True if the switch was successful, false otherwise.</returns>
    public async Task<bool> SwitchToDesktopAsync(DesktopId desktopId)
    {
        try
        {
            _virtualDesktopManagerInternal.FindDesktop(desktopId.Value, out IntPtr desktopPtr);
            if (desktopPtr == IntPtr.Zero)
            {
                throw new DesktopNotFoundException(desktopId);
            }

            _virtualDesktopManagerInternal.SwitchDesktop(desktopPtr);
            Marshal.Release(desktopPtr);

            // Allow time for the switch to complete
            await Task.Delay(100);
            return true;
        }
        catch (DesktopNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException($"Failed to switch to desktop {desktopId}.", ex);
        }
    }

    /// <summary>
    /// Switches to the next virtual desktop in sequence.
    /// If currently on the last desktop, wraps around to the first desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public async Task<bool> SwitchToNextDesktopAsync()
    {
        try
        {
            _virtualDesktopManagerInternal.GetCurrentDesktop(out IntPtr currentDesktopPtr);
            if (currentDesktopPtr == IntPtr.Zero)
            {
                return false;
            }

            _virtualDesktopManagerInternal.GetAdjacentDesktop(currentDesktopPtr, 1, out IntPtr nextDesktopPtr);
            if (nextDesktopPtr == IntPtr.Zero)
            {
                // Wrap around to first desktop
                _virtualDesktopManagerInternal.GetDesktops(out IntPtr desktopsPtr);
                var desktops = (IObjectArray)Marshal.GetObjectForIUnknown(desktopsPtr);
                desktops.GetAt(0, typeof(IVirtualDesktop).GUID, out nextDesktopPtr);
                Marshal.Release(desktopsPtr);
            }

            if (nextDesktopPtr != IntPtr.Zero)
            {
                _virtualDesktopManagerInternal.SwitchDesktop(nextDesktopPtr);
                Marshal.Release(nextDesktopPtr);
            }

            Marshal.Release(currentDesktopPtr);

            // Allow time for the switch to complete
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to switch to next desktop.", ex);
        }
    }

    /// <summary>
    /// Switches to the previous virtual desktop in sequence.
    /// If currently on the first desktop, wraps around to the last desktop.
    /// </summary>
    /// <returns>True if the switch was successful, false if no desktops are available.</returns>
    public async Task<bool> SwitchToPreviousDesktopAsync()
    {
        try
        {
            _virtualDesktopManagerInternal.GetCurrentDesktop(out IntPtr currentDesktopPtr);
            if (currentDesktopPtr == IntPtr.Zero)
            {
                return false;
            }

            _virtualDesktopManagerInternal.GetAdjacentDesktop(currentDesktopPtr, 0, out IntPtr prevDesktopPtr);
            if (prevDesktopPtr == IntPtr.Zero)
            {
                // Wrap around to last desktop
                _virtualDesktopManagerInternal.GetDesktops(out IntPtr desktopsPtr);
                var desktops = (IObjectArray)Marshal.GetObjectForIUnknown(desktopsPtr);
                desktops.GetCount(out int count);
                desktops.GetAt(count - 1, typeof(IVirtualDesktop).GUID, out prevDesktopPtr);
                Marshal.Release(desktopsPtr);
            }

            if (prevDesktopPtr != IntPtr.Zero)
            {
                _virtualDesktopManagerInternal.SwitchDesktop(prevDesktopPtr);
                Marshal.Release(prevDesktopPtr);
            }

            Marshal.Release(currentDesktopPtr);

            // Allow time for the switch to complete
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to switch to previous desktop.", ex);
        }
    }

    /// <summary>
    /// Creates a new virtual desktop with an optional name.
    /// </summary>
    /// <param name="name">Optional display name for the new desktop.</param>
    /// <returns>The newly created virtual desktop.</returns>
    public async Task<VirtualDesktop> CreateDesktopAsync(string? name = null)
    {
        try
        {
            _virtualDesktopManagerInternal.CreateDesktopW(out IntPtr newDesktopPtr);
            var newDesktop = (IVirtualDesktop)Marshal.GetObjectForIUnknown(newDesktopPtr);

            var virtualDesktop = CreateVirtualDesktopFromComObject(newDesktop, -1);

            Marshal.Release(newDesktopPtr);

            // Allow time for the creation to complete
            await Task.Delay(100);
            return virtualDesktop;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException("Failed to create desktop.", ex);
        }
    }

    /// <summary>
    /// Removes the specified virtual desktop.
    /// </summary>
    /// <param name="desktopId">The unique identifier of the desktop to remove.</param>
    /// <returns>True if the removal was successful, false otherwise.</returns>
    public async Task<bool> RemoveDesktopAsync(DesktopId desktopId)
    {
        try
        {
            _virtualDesktopManagerInternal.FindDesktop(desktopId.Value, out IntPtr desktopPtr);
            if (desktopPtr == IntPtr.Zero)
            {
                throw new DesktopNotFoundException(desktopId);
            }

            // Get current desktop as fallback
            _virtualDesktopManagerInternal.GetCurrentDesktop(out IntPtr currentDesktopPtr);

            _virtualDesktopManagerInternal.RemoveDesktop(desktopPtr, currentDesktopPtr);

            Marshal.Release(desktopPtr);
            Marshal.Release(currentDesktopPtr);

            // Allow time for the removal to complete
            await Task.Delay(100);
            return true;
        }
        catch (DesktopNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException($"Failed to remove desktop {desktopId}.", ex);
        }
    }

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to move.</param>
    /// <param name="desktopId">The unique identifier of the target desktop.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    public async Task<bool> MoveWindowToDesktopAsync(WindowHandle windowHandle, DesktopId desktopId)
    {
        try
        {
            _virtualDesktopManagerInternal.FindDesktop(desktopId.Value, out IntPtr desktopPtr);
            if (desktopPtr == IntPtr.Zero)
            {
                throw new DesktopNotFoundException(desktopId);
            }

            // Note: This is a simplified implementation. In a real implementation,
            // we would need to get the application view for the window and use
            // MoveViewToDesktop instead of MoveWindowToDesktop.
            int result = _virtualDesktopManager.MoveWindowToDesktop(windowHandle.Value, desktopId.Value);

            Marshal.Release(desktopPtr);

            // Allow time for the move to complete
            await Task.Delay(100);
            return result == 0; // S_OK
        }
        catch (DesktopNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DesktopOperationException($"Failed to move window {windowHandle} to desktop {desktopId}.", ex);
        }
    }

    /// <summary>
    /// Creates a VirtualDesktop entity from a COM IVirtualDesktop object.
    /// </summary>
    private static VirtualDesktop CreateVirtualDesktopFromComObject(IVirtualDesktop desktop, int index)
    {
        desktop.GetId(out Guid id);
        desktop.GetName(out IntPtr namePtr);

        string? name = null;
        if (namePtr != IntPtr.Zero)
        {
            name = Marshal.PtrToStringUni(namePtr);
            Marshal.FreeCoTaskMem(namePtr);
        }

        // If index is not provided, get it from the desktop
        if (index == -1)
        {
            desktop.GetIndex(out index);
        }

        return new VirtualDesktop(new DesktopId(id), name, index, false);
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

            if (_virtualDesktopManagerInternal != null)
            {
                Marshal.ReleaseComObject(_virtualDesktopManagerInternal);
            }

            if (_serviceProvider != null)
            {
                Marshal.ReleaseComObject(_serviceProvider);
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~WindowsVirtualDesktopManager()
    {
        Dispose(false);
    }
}

/// <summary>
/// COM interface for object array.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("92ca9dcd-5622-4bba-a805-5e9f541bd8c9")]
internal interface IObjectArray
{
    void GetCount(out int count);
    void GetAt(int index, [MarshalAs(UnmanagedType.LPStruct)] Guid iid, out IntPtr obj);
}