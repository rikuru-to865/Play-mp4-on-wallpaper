using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace testWinforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChromiumWebBrowser cefBrowser;
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            cefBrowser = new ChromiumWebBrowser("https://yahoo.co.jp");
            this.Controls.Add(cefBrowser);
            cefBrowser.Dock = DockStyle.Fill;
        }
    }
}