using System;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using System.Configuration;
using Microsoft.Win32;

namespace BackGroundMovie
{
    public partial class movie : Form
    {
        public movie()
        {
            SystemEvents.PowerModeChanged += (sender, e) =>
            {
                this.axWindowsMediaPlayer1.fullScreen = false;
                this.axWindowsMediaPlayer1.settings.volume = 0;
                this.axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultVideo;
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
            axWindowsMediaPlayer1.URL = Properties.Settings.Default.DefaultVideo;
        }


        private void movie_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.setMode("loop", true);
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

            //OpenFileDialogクラスのインスタンスを作成
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "mp4ファイル(*.mp4)|*.mp4";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.axWindowsMediaPlayer1.URL = ofd.FileName;
            }
            DialogResult result = MessageBox.Show("デフォルトに設定しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.DefaultVideo = ofd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
