using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

public class Win32
{
    [Flags()]
    public enum DeviceContextValues : uint
    {
        /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
        /// than the client rectangle.</summary>
        Window = 0x00000001,
        /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
        /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
        Cache = 0x00000002,
        /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
        /// default attributes when this DC is released.</summary>
        NoResetAttrs = 0x00000004,
        /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
        /// below the window identified by hWnd.</summary>
        ClipChildren = 0x00000008,
        /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
        /// above the window identified by hWnd.</summary>
        ClipSiblings = 0x00000010,
        /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
        /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
        /// set to the upper-left corner of the window identified by hWnd.</summary>
        ParentClip = 0x00000020,
        /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
        /// from the visible region of the returned DC.</summary>
        ExcludeRgn = 0x00000040,
        /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
        /// intersected with the visible region of the returned DC.</summary>
        IntersectRgn = 0x00000080,
        /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
        ExcludeUpdate = 0x00000100,
        /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
        IntersectUpdate = 0x00000200,
        /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
        /// call in effect that would otherwise exclude this window. Used for drawing during
        /// tracking.</summary>
        LockWindowUpdate = 0x00000400,
        /// <summary>DCX_USESTYLE: Undocumented, something related to WM_NCPAINT message.</summary>
        UseStyle = 0x00010000,
        /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
        /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
        /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
        Validate = 0x00200000,
    }
    public enum SendMessageTimeoutFlags
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20,
    }
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll")]
    public static extern IntPtr GetDCEx_IntPtr(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);
    [DllImport("user32.dll")]
    public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(
        string lpClassName,
        string lpWindowName);

    [DllImport("user32.dll")]
    public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(
        IntPtr parentHandle,
        IntPtr childAfter,
        string lclassName,
        string windowTitle);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        UIntPtr wParam,
        IntPtr lParam,
        SendMessageTimeoutFlags fuFlags,
        uint uTimeout,
        out UIntPtr lpdwResult);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public static void setOwnership(Form f)
    {
        IntPtr progman = FindWindow("Progman", null);
        if (progman == IntPtr.Zero)
        {
            Console.Write("Could not find handle to Progman window.");
            return;
        }

        UIntPtr result = UIntPtr.Zero;
        IntPtr msgResult = SendMessageTimeout(
            progman,
            0x052C,
            new UIntPtr(0),
            IntPtr.Zero,
            SendMessageTimeoutFlags.SMTO_NORMAL,
            1000,
            out result);
        if (msgResult == IntPtr.Zero)
        {
            Console.Write("Failed to send message to Progman.");
            return;
        }

        IntPtr workerw = IntPtr.Zero;
        EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
        {
            IntPtr p = FindWindowEx(tophandle,
                                        IntPtr.Zero,
                                        "SHELLDLL_DefView",
                                        "");

            if (p != IntPtr.Zero)
            {
                workerw = FindWindowEx(IntPtr.Zero,
                                           tophandle,
                                           "WorkerW",
                                           "");
            }

            return true;
        }), IntPtr.Zero);

        if (workerw == IntPtr.Zero)
        {
            Console.Write("Could not find handle to WorkerW window.");
            return;
        }

        SetParent(f.Handle, workerw);
    }
}