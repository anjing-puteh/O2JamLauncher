using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Windows.Forms;
using O2JamLauncher.Ini;

namespace O2JamLauncher
{
    public partial class Form1 : Form
    {
        // DLLs Load
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        // LauncherBasicConfig

        public string ProcessName = "O2-JAM";
        public string ProcessArgs = "";
        public string ServerDomain = "samojams3.servegame.com";
        public string WinX = "1366";
        public string WinY = "768";

        public bool window = true;
        public bool isResize = true;

        private const UInt32 SWP_NOSIZE = 0x1;
        private const UInt32 SWP_NOMOVE = 0x2;
        private const UInt32 SWP_NOZORDER = 0x4;
        public const Int32 HWND_TOPMOST = -0x1;

        public Timer Timer1 { get; set; } = new Timer();
        public IniFile Config;

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
            Timer1.Interval = 5;

            if (!File.Exists(Application.StartupPath + "\\monoxjam.ini"))
            {
                File.WriteAllText(Application.StartupPath + "\\monoxjam.ini", Properties.Resources.monoxjam);
            }

            Config = new IniFile(Application.StartupPath + "\\monoxjam.ini");

            window = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "WindowMode"));
            isResize = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "IsResize"));
            ProcessName = Config.IniReadValue("LAUNCHER", "ProcessName");
            ServerDomain = Config.IniReadValue("LAUNCHER", "ServerDomain");
            WinX = Config.IniReadValue("LAUNCHER", "WinX");
            WinY = Config.IniReadValue("LAUNCHER", "WinY");

            checkBox1.Checked = window;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool checkAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\otwo.exe")) {
                MessageBox.Show("otwo.exe not detected in directory nor process!", "Error");
                return;
            }

            if (window & isResize & !checkAdministrator())
            {
                DialogResult res = MessageBox.Show("This launcher not running in Administrator, and Window Resizer may not working properly continue?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            try
            {
                IPAddress[] addressList = Dns.GetHostEntry(ServerDomain).AddressList;
                ProcessArgs = string.Concat(new string[] { " 1 ", addressList[0].ToString(), " o2jam/patch ", addressList[0].ToString(), ":15000 1 1 1 1 1 1 1 1 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010 ", addressList[0].ToString(), " 15010" });

                ProcessStartInfo proc = new ProcessStartInfo()
                {
                    FileName = Environment.CurrentDirectory + "\\otwo.exe",
                    Arguments = ProcessArgs,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process.Start(proc);
                Timer1.Start();
            } catch (Exception error)
            {
                ProjectData.SetProjectError(error);
                MessageBox.Show(error.ToString(), "Error");
                ProjectData.ClearProjectError();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd;
            Process[] NoteProc = Process.GetProcessesByName("otwo"); // Get Notepad's handle
            try
            {
                hWnd = FindWindow("O2-JAM", NoteProc[0].MainWindowTitle);
                SetWindowPos(hWnd, (int)(new IntPtr(HWND_TOPMOST)), 0, 0, int.Parse(WinX), int.Parse(WinY), SWP_NOMOVE);
                if (Conversions.ToBoolean(NoteProc[0].MainWindowTitle.Contains("O2-JAM").ToString().ToUpper()))
                {
                    Timer1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
    }
}
