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
        ChromiumWebBrowser cefBrowser;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, string lpvParam, uint fuWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 1;
        private const uint SPIF_SENDWININICHANGE = 2;
        public movie()
        {
            SystemEvents.PowerModeChanged += (sender, e) =>
            {
                this.axWindowsMediaPlayer1.fullScreen = false;
                this.axWindowsMediaPlayer1.settings.volume = 0;
                this.axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultFile;
                this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
                Program.setparent();
            };

            //Runキーを開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);
            //値の名前に製品名、値のデータに実行ファイルのパスを指定し、書き込む
            regkey.SetValue(Application.ProductName, Application.ExecutablePath);
            //閉じる
            regkey.Close();

            InitializeComponent();
            if (Path.GetExtension(Properties.Settings.Default.DefaultFile) != ".html" && Path.GetExtension(Properties.Settings.Default.DefaultFile) != ".htm")
            {
                axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultFile;
            }
            else
            {
                this.axWindowsMediaPlayer1.Visible = false;
                ChromiumWebBrowser cefBrowser;
                CefSettings settings = new CefSettings();
                Cef.Initialize(settings);
                cefBrowser = new ChromiumWebBrowser(Properties.Settings.Default.DefaultFile);
                this.Controls.Add(cefBrowser);
                cefBrowser.Dock = DockStyle.Fill;
            }

        }


        private void movie_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.setMode("loop", true);
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
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
            this.axWindowsMediaPlayer1.Visible = true;

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
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder(@"C:\Windows\Web\Wallpaper\Windows");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, (uint)sb.Length, sb.ToString(), SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
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
                CefSettings settings = new CefSettings();
                Cef.Initialize(settings);
                cefBrowser = new ChromiumWebBrowser(ofd.FileName);
                this.Controls.Add(cefBrowser);
                cefBrowser.Dock = DockStyle.Fill;
                
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
    }
}
