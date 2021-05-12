using System;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using System.Configuration;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.WinForms;
using System.Threading;

namespace BackGroundMovie
{
    public partial class movie : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, string lpvParam, uint fuWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 1;
        private const uint SPIF_SENDWININICHANGE = 2;

        // 壁紙にしたい画像のパス
        private readonly static string exefolder = Directory.GetCurrentDirectory();


        ChromiumWebBrowser cefBrowser;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int MK_LBUTTON = 0x0001;
        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            cefBrowser = new ChromiumWebBrowser();
            // Add it to the form and fill it to the form window.
            this.Controls.Add(cefBrowser);
            cefBrowser.Dock = DockStyle.Fill;
        }


        public movie()
        {
            InitializeChromium();
            SystemEvents.PowerModeChanged += (sender, e) =>
            {
                this.axWindowsMediaPlayer1.fullScreen = false;
                this.axWindowsMediaPlayer1.settings.volume = 0;
                this.axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultFile;
                this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
                Program.setparent();
            };



            InitializeComponent();
            if (Path.GetExtension(Properties.Settings.Default.DefaultFile) != ".html" && Path.GetExtension(Properties.Settings.Default.DefaultFile) != ".htm")
            {
                axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultFile;
            }
            else
            {
                cefBrowser.Load(Properties.Settings.Default.DefaultFile);
            }

        }


        private void movie_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.setMode("loop", true);
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop");
            string beforeWallpaper = (string)key.GetValue(@"WallPaper");
            key.Close();
            File.Copy(beforeWallpaper, @"./BeforeWallpaper.png", true);
        }
        private async Task CheckWindow()
        {
            bool isPause = false;
            await Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(1000);
                    StringBuilder sb = new StringBuilder(65535);
                    Win32.GetWindowText(Win32.GetForegroundWindow(), sb, 65535);
                    if (sb.Length == 0)
                    {
                        if (isPause == true)
                        {
                            axWindowsMediaPlayer1.Ctlcontrols.play();
                        }
                    }
                    else
                    {
                        isPause = true;
                        axWindowsMediaPlayer1.Ctlcontrols.pause();
                    }
                }
            });
        }


        private void ほかの動画を流すToolStripMenuItem_Click(object sender, EventArgs e)
        {


            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();


            ofd.Filter = "mp4ファイル(*.mp4)|*.mp4|すべてのファイル(*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.axWindowsMediaPlayer1.URL = ofd.FileName;
            }
            DialogResult result = MessageBox.Show("デフォルトに設定しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.DefaultFile = ofd.FileName;
                Properties.Settings.Default.Save();
            }
            this.axWindowsMediaPlayer1.Visible = true;
            cefBrowser.Visible = false;
            this.timer1.Enabled = true;
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder img = new StringBuilder(exefolder + @"/BeforeWallpaper.png");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, img.ToString(), SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition > axWindowsMediaPlayer1.Ctlcontrols.currentItem.duration - 0.01)
            {
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
            }
        }

        private void htmlファイルからToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            ofd.Filter = "htmlファイル(*.html)|*.html|すべてのファイル(*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.axWindowsMediaPlayer1.Visible = false;
                cefBrowser.Visible = true;
                cefBrowser.Load(ofd.FileName);
            }
            DialogResult result = MessageBox.Show("デフォルトに設定しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.DefaultFile = ofd.FileName;
                Properties.Settings.Default.Save();
            }


        }
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Application_ThreadExceptionによる例外通知です。");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            var pt = Cursor.Position;
            MouseEvent mouseEvent = new MouseEvent(pt.X, pt.Y, CefEventFlags.None);
            cefBrowser.GetBrowser().GetHost().SendMouseMoveEvent(mouseEvent, false);
        }

        private void urlからToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = Microsoft.VisualBasic.Interaction.InputBox("urlを入力", "url", default, -1, -1);
            cefBrowser.Load(str);
        }
    }
}
