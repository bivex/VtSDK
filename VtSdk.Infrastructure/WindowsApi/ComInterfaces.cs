using System.Runtime.InteropServices;

namespace VtSdk.Infrastructure.WindowsApi;

/// <summary>
/// COM interface for virtual desktop manager.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
internal interface IVirtualDesktopManager
{
    /// <summary>
    /// Checks if a window is on the current virtual desktop.
    /// </summary>
    [PreserveSig]
    int IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow, out bool onCurrentDesktop);

    /// <summary>
    /// Gets the desktop ID for a window.
    /// </summary>
    [PreserveSig]
    int GetWindowDesktopId(IntPtr topLevelWindow, out Guid desktopId);

    /// <summary>
    /// Moves a window to the specified desktop.
    /// </summary>
    [PreserveSig]
    int MoveWindowToDesktop(IntPtr topLevelWindow, [MarshalAs(UnmanagedType.LPStruct)] Guid desktopId);
}

/// <summary>
/// COM interface for virtual desktop manager internal operations.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("f31574d6-b682-4cdc-bd56-1827860abec6")]
internal interface IVirtualDesktopManagerInternal
{
    /// <summary>
    /// Gets the count of virtual desktops.
    /// </summary>
    void GetCount(out int count);

    /// <summary>
    /// Moves a view to a desktop.
    /// </summary>
    void MoveViewToDesktop(IntPtr view, IntPtr desktop);

    /// <summary>
    /// Checks if a view can be moved.
    /// </summary>
    void CanViewMoveDesktops(IntPtr view, out bool canMove);

    /// <summary>
    /// Gets the current desktop.
    /// </summary>
    void GetCurrentDesktop(out IntPtr desktop);

    /// <summary>
    /// Gets all desktops.
    /// </summary>
    void GetDesktops(out IntPtr desktops);

    /// <summary>
    /// Gets adjacent desktops.
    /// </summary>
    void GetAdjacentDesktop(IntPtr desktop, int direction, out IntPtr adjacentDesktop);

    /// <summary>
    /// Switches to a desktop.
    /// </summary>
    void SwitchDesktop(IntPtr desktop);

    /// <summary>
    /// Creates a desktop.
    /// </summary>
    void CreateDesktopW(out IntPtr desktop);

    /// <summary>
    /// Removes a desktop.
    /// </summary>
    void RemoveDesktop(IntPtr desktop, IntPtr fallbackDesktop);

    /// <summary>
    /// Finds a desktop by ID.
    /// </summary>
    void FindDesktop([MarshalAs(UnmanagedType.LPStruct)] Guid desktopId, out IntPtr desktop);
}

/// <summary>
/// COM interface for application view.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("372e1d3b-38d3-42e4-a15b-8ab2b178f513")]
internal interface IApplicationView
{
    // ... other methods ...
}

/// <summary>
/// COM interface for application view collection.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("1841c6d7-4f9d-42c0-af41-8747538f10e5")]
internal interface IApplicationViewCollection
{
    // ... other methods ...
}

/// <summary>
/// COM interface for virtual desktop (Windows 10).
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("FF72FFDD-BE7E-43FC-9C03-AD81681E88E4")]
internal interface IVirtualDesktop_Win10
{
    /// <summary>
    /// Checks if a view is visible on this desktop.
    /// </summary>
    bool IsViewVisible(IApplicationView view);

    /// <summary>
    /// Gets the desktop ID.
    /// </summary>
    Guid GetId();
}

/// <summary>
/// COM interface for virtual desktop (Windows 11 22H2+).
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("536D3495-B208-4CC9-AE26-DE8111275BF8")]
internal interface IVirtualDesktop_Win11_22H2
{
    /// <summary>
    /// Checks if a view is visible on this desktop.
    /// </summary>
    bool IsViewVisible(IApplicationView view);

    /// <summary>
    /// Gets the desktop ID.
    /// </summary>
    Guid GetId();

    /// <summary>
    /// Gets the desktop name.
    /// </summary>
    [return: MarshalAs(UnmanagedType.HString)]
    string GetName();

    /// <summary>
    /// Gets the wallpaper path.
    /// </summary>
    [return: MarshalAs(UnmanagedType.HString)]
    string GetWallpaperPath();

    /// <summary>
    /// Checks if this is a remote desktop.
    /// </summary>
    bool IsRemote();
}

/// <summary>
/// COM interface for virtual desktop (Windows 11 23H2+).
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("3F07F4BE-B107-441A-AF0F-39D82529072C")]
internal interface IVirtualDesktop_Win11_Newer
{
    /// <summary>
    /// Checks if a view is visible on this desktop.
    /// </summary>
    bool IsViewVisible(IApplicationView view);

    /// <summary>
    /// Gets the desktop ID.
    /// </summary>
    Guid GetId();

    /// <summary>
    /// Gets the desktop name.
    /// </summary>
    [return: MarshalAs(UnmanagedType.HString)]
    string GetName();

    /// <summary>
    /// Gets the wallpaper path.
    /// </summary>
    [return: MarshalAs(UnmanagedType.HString)]
    string GetWallpaperPath();

    /// <summary>
    /// Checks if this is a remote desktop.
    /// </summary>
    bool IsRemote();
}

/// <summary>
/// COM interface for virtual desktop.
/// Uses the Windows 10 GUID as default.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("FF72FFDD-BE7E-43FC-9C03-AD81681E88E4")] // Windows 10 GUID
internal interface IVirtualDesktop
{
    /// <summary>
    /// Checks if a view is visible on this desktop.
    /// </summary>
    bool IsViewVisible(IApplicationView view);

    /// <summary>
    /// Gets the desktop ID.
    /// </summary>
    Guid GetId();
}

/// <summary>
/// COM interface for virtual desktop notification service.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0cd45e71-d927-4f89-8198-92eb6f7ea986")]
internal interface IVirtualDesktopNotification
{
    /// <summary>
    /// Called when the current desktop changes.
    /// </summary>
    void CurrentVirtualDesktopChanged(IntPtr oldDesktop, IntPtr newDesktop);

    /// <summary>
    /// Called when a virtual desktop is created.
    /// </summary>
    void VirtualDesktopCreated(IntPtr desktop);

    /// <summary>
    /// Called when a virtual desktop is removed.
    /// </summary>
    void VirtualDesktopDestroyed(IntPtr desktop, IntPtr fallbackDesktop);

    /// <summary>
    /// Called when a view changes desktops.
    /// </summary>
    void ViewVirtualDesktopChanged(IntPtr view);
}

/// <summary>
/// Service provider interface for getting virtual desktop services.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
internal interface IServiceProvider10
{
    [PreserveSig]
    int QueryService(ref Guid serviceId, ref Guid interfaceId, out IntPtr service);
}

/// <summary>
/// Generic service provider interface.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
internal interface IServiceProvider
{
    [PreserveSig]
    int QueryService(ref Guid serviceId, ref Guid interfaceId, out IntPtr service);
}
