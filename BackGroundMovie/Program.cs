using System;
using System.Windows.Forms;

namespace BackGroundMovie
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr progman = Win32.FindWindow("Progman", null);
            UIntPtr result = UIntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            Win32.SendMessageTimeout(progman,
                                   0x052C,
                                   new UIntPtr(0),
                                   IntPtr.Zero,
                                   Win32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);
            IntPtr workerw = IntPtr.Zero;

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            Win32.EnumWindows(new Win32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = Win32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            "");

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = Win32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               "");
                }

                return true;
            }), IntPtr.Zero);
            // Get the Device Context of the WorkerW
            IntPtr dc = Win32.GetDCEx(workerw, IntPtr.Zero, (Win32.DeviceContextValues)0x403);
            if (dc != IntPtr.Zero)
            {
                movie movie = new movie();
                Win32.SetParent(movie.Handle, workerw);
                // Start the Application Loop for the Form.
                Application.Run(movie);
                // Create a Graphics instance from the Device Context
            }
        }
        public static void tes()
        {

        }
        
    }
}
