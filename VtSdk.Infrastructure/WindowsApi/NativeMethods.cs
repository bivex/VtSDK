using System.Runtime.InteropServices;

namespace VtSdk.Infrastructure.WindowsApi;

/// <summary>
/// P/Invoke declarations for Windows API functions.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Window style constants.
    /// </summary>
    [Flags]
    public enum WindowStyles : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
    }

    /// <summary>
    /// Extended window style constants.
    /// </summary>
    [Flags]
    public enum ExtendedWindowStyles : uint
    {
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_LAYOUTRTL = 0x00400000,
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000,
    }

    /// <summary>
    /// GetWindowLongPtr constants.
    /// </summary>
    public enum GetWindowLongIndex : int
    {
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20,
        GWL_ID = -12,
        GWL_HWNDPARENT = -8,
        GWL_HINSTANCE = -6,
        GWL_WNDPROC = -4,
        GWL_USERDATA = -21,
        GWL_DWLP_DLGPROC = 8,
        GWL_DWLP_USER = 8,
        GWL_DWLP_MSGRESULT = 0,
    }

    /// <summary>
    /// ShowWindow constants.
    /// </summary>
    public enum ShowWindowCommands : int
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_FORCEMINIMIZE = 11,
    }

    /// <summary>
    /// Gets the window text.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    /// <summary>
    /// Gets the window text length.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    /// <summary>
    /// Gets the window long value.
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GetWindowLongIndex nIndex);

    /// <summary>
    /// Gets the window long value (32-bit version).
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    public static extern int GetWindowLong(IntPtr hWnd, GetWindowLongIndex nIndex);

    /// <summary>
    /// Enumerates windows.
    /// </summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    /// <summary>
    /// Delegate for EnumWindows callback.
    /// </summary>
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// Checks if a window is visible.
    /// </summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    /// <summary>
    /// Gets the process ID that owns the window.
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    /// <summary>
    /// Gets the window placement.
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// Window placement structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public ShowWindowCommands showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    /// <summary>
    /// Point structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// Rectangle structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// Gets the process name from a process ID.
    /// </summary>
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    /// <summary>
    /// Closes a handle.
    /// </summary>
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);

    /// <summary>
    /// Gets the module file name for a process.
    /// </summary>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, System.Text.StringBuilder lpBaseName, uint nSize);

    /// <summary>
    /// Process access rights.
    /// </summary>
    [Flags]
    public enum ProcessAccessRights : uint
    {
        PROCESS_TERMINATE = 0x0001,
        PROCESS_CREATE_THREAD = 0x0002,
        PROCESS_SET_SESSIONID = 0x0004,
        PROCESS_VM_OPERATION = 0x0008,
        PROCESS_VM_READ = 0x0010,
        PROCESS_VM_WRITE = 0x0020,
        PROCESS_DUP_HANDLE = 0x0040,
        PROCESS_CREATE_PROCESS = 0x0080,
        PROCESS_SET_QUOTA = 0x0100,
        PROCESS_SET_INFORMATION = 0x0200,
        PROCESS_QUERY_INFORMATION = 0x0400,
        PROCESS_SUSPEND_RESUME = 0x0800,
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
        PROCESS_SET_LIMITED_INFORMATION = 0x2000,
        PROCESS_ALL_ACCESS = 0x1FFFFF,
        PROCESS_DELETE = 0x00010000,
        PROCESS_READ_CONTROL = 0x00020000,
        PROCESS_WRITE_DAC = 0x00040000,
        PROCESS_WRITE_OWNER = 0x00080000,
        PROCESS_SYNCHRONIZE = 0x00100000,
    }

    /// <summary>
    /// Gets the class name of a window.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

    /// <summary>
    /// Gets the desktop window.
    /// </summary>
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    /// <summary>
    /// Gets the shell window.
    /// </summary>
    [DllImport("user32.dll")]
    public static extern IntPtr GetShellWindow();

    /// <summary>
    /// CoCreates an instance of a COM class.
    /// </summary>
    [DllImport("ole32.dll", PreserveSig = false)]
    public static extern void CoCreateInstance(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
        IntPtr pUnkOuter,
        uint dwClsContext,
        [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
        out IntPtr ppv);

    /// <summary>
    /// CLSCTX constants.
    /// </summary>
    public static class CLSCTX
    {
        public const uint CLSCTX_INPROC_SERVER = 1;
        public const uint CLSCTX_INPROC_HANDLER = 2;
        public const uint CLSCTX_LOCAL_SERVER = 4;
        public const uint CLSCTX_INPROC_SERVER16 = 8;
        public const uint CLSCTX_REMOTE_SERVER = 16;
        public const uint CLSCTX_INPROC_HANDLER16 = 32;
        public const uint CLSCTX_RESERVED1 = 64;
        public const uint CLSCTX_RESERVED2 = 128;
        public const uint CLSCTX_RESERVED3 = 256;
        public const uint CLSCTX_RESERVED4 = 512;
        public const uint CLSCTX_NO_CODE_DOWNLOAD = 1024;
        public const uint CLSCTX_RESERVED5 = 2048;
        public const uint CLSCTX_NO_CUSTOM_MARSHAL = 4096;
        public const uint CLSCTX_ENABLE_CODE_DOWNLOAD = 8192;
        public const uint CLSCTX_NO_FAILURE_LOG = 16384;
        public const uint CLSCTX_DISABLE_AAA = 32768;
        public const uint CLSCTX_ENABLE_AAA = 65536;
        public const uint CLSCTX_FROM_DEFAULT_CONTEXT = 131072;
        public const uint CLSCTX_ACTIVATE_32_BIT_SERVER = 262144;
        public const uint CLSCTX_ACTIVATE_64_BIT_SERVER = 524288;
        public const uint CLSCTX_ENABLE_CLOAKING = 1048576;
        public const uint CLSCTX_APPCONTAINER = 4194304;
        public const uint CLSCTX_ACTIVATE_AAA_AS_IU = 8388608;
        public const uint CLSCTX_PS_DLL = 2147483648;
        public const uint CLSCTX_ALL = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER;
    }
}