using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TestProject
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, string lpvParam, uint fuWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 1;
        private const uint SPIF_SENDWININICHANGE = 2;

        // 壁紙にしたい画像のパス
        private readonly static string exefolder = Directory.GetCurrentDirectory();
        private static StringBuilder img = new StringBuilder(@"D:\for miku back\BackGroundMovie\BackGroundMovie\bin\Debug\BeforeWallpaper.png");

        static void Main(string[] args)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, img.ToString(), SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
