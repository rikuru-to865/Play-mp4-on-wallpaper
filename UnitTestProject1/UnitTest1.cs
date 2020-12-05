using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [TestMethod]
        public void TestMethod1()
        {
            Thread.Sleep(3000);
            StringBuilder blank = new StringBuilder("BackGroundMovie - Microsoft Visual Studio");
            StringBuilder sb = new StringBuilder(65535);//65535に特に意味はない
            GetWindowText(GetForegroundWindow(), sb, 65535);
            Console.WriteLine(sb);
            if(sb.Length == 0)
            {
                Console.WriteLine("デスクトップ");
            }
            else
            {
                Console.WriteLine("no");
            }
        }
    }
}
