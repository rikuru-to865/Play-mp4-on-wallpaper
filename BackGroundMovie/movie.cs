using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.IO;

namespace BackGroundMovie
{
    public partial class movie : Form
    {
        public movie()
        {
            if (File.Exists("./miku.mp4") != true)
            {
                byte[] miku = Properties.Resources.miku;
                using (BinaryWriter mi = new BinaryWriter(new FileStream("./miku.mp4", FileMode.Create)))
                {
                    mi.Write(miku, 0, miku.Length);
                }
            }
            InitializeComponent();
        }

        private void movie_Load(object sender, EventArgs e)
        {
            
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.setMode("loop", true);
        }
    }
}
