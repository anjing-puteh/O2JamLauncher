using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace O2JamLauncher
{
    public partial class Form1 : Form
    {
        // DLLs Load
        [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "FindWindowA", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr FindWindow(ref string lpClassName, ref string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Basic Declaration
        public Timer Timer1 = new Timer();

        // LauncherBasicConfig
        public string ProcessName = "O2-JAM";
        public string WinX = "800";
        public string WinY = "600";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "MonoxJam Launcher";
            tabPage1.Text = "Home";
            tabPage2.Text = "Settings";

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = 0;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\otwo.exe")) {
                MessageBox.Show("otwo.exe not detected in directory nor process!", "Error");
                return;
            }

            Process app = new Process();
            app.StartInfo.FileName = Application.StartupPath + "\\otwo.exe";
            app.StartInfo.Arguments = "";
            app.Start();
            Timer1.Start();
            Hide();

            app.WaitForExit();
            Show();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Process[] processesByName = Process.GetProcessesByName("otwo");
            try
            {
                string mainWindowTitle = processesByName[0].MainWindowTitle;
                IntPtr intPtr1 = FindWindow(ref ProcessName, ref mainWindowTitle);
                SetWindowPos(intPtr1, (int)(new IntPtr(-1)), 0, 0, Conversions.ToInteger(WinX), Conversions.ToInteger(WinY), 2);
                if (Conversions.ToBoolean(processesByName[0].MainWindowTitle.Contains("O2-JAM").ToString().ToUpper()))
                {
                    Timer1.Enabled = false;
                }
            } catch (Exception error)
            {
                ProjectData.SetProjectError(error);
                ProjectData.ClearProjectError();
            }
        }
    }
}
