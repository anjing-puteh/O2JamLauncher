using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Windows.Forms;
using O2JamLauncher.Ini;
using O2JamLauncher.ResourceMGR;

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
        public string LagFix = "";
        public string WebPort = "";
        public string GamePort = "";

        public string defaultGamePort = "15010";
        public string defaultWebPort = "15000";

        public bool window = true;
        public bool isResize = true;
        public bool DefaultServer = true;
        public bool DefaultProcess = true;
        public bool DefaultPort = true;

        private const UInt32 SWP_NOSIZE = 0x1;
        private const UInt32 SWP_NOMOVE = 0x2;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;
        private const UInt32 SWP_NOZORDER = 0x4;
        public const Int32 HWND_TOPMOST = -0x1;

        public Timer Timer1 { get; set; } = new Timer();
        public IniFile Config;
        public ResourceManager resourceManager = new ResourceManager();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "MonoxJam Launcher";
            Icon = Properties.Resources.Icon3;
            MaximizeBox = false;
            tabPage1.Text = "Home";
            tabPage2.Text = "Settings";
            tabPage3.Text = "About";

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = 5;

            if (!File.Exists(Application.StartupPath + "\\monoxjam.ini"))
            {
                ResourceManager.GenerateConfig();
            }

            Config = new IniFile(Application.StartupPath + "\\monoxjam.ini");

            window = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "WindowMode"));
            isResize = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "IsResize"));
            DefaultServer = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "DefaultServer"));
            DefaultProcess = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "DefaultProcess"));
            DefaultPort = Convert.ToBoolean(Config.IniReadValue("LAUNCHER", "DefaultPort"));
            ProcessName = Config.IniReadValue("LAUNCHER", "ProcessName");
            ServerDomain = Config.IniReadValue("LAUNCHER", "ServerDomain");
            WinX = Config.IniReadValue("LAUNCHER", "WinX");
            WinY = Config.IniReadValue("LAUNCHER", "WinY");
            LagFix = Config.IniReadValue("LAUNCHER", "LAGFIX");
            textBox5.Text = Config.IniReadValue("LAUNCHER", "GamePort");
            textBox6.Text = Config.IniReadValue("LAUNCHER", "WebPort");

            label8.Text = "Config loaded!";
            textBox3.Text = ServerDomain;

            if (DefaultServer)
            {
                ServerDomain = "dnso2.o2jam.com.ph";
                checkBox3.Checked = true;
                textBox3.Enabled = false;
                button3.Enabled = false;
            }

            textBox4.Text = ProcessName;

            if (DefaultProcess)
            {
                textBox4.Enabled = false;
                button4.Enabled = false;
                ProcessName = "O2-JAM";
                checkBox6.Checked = true;
            }

            if (DefaultPort)
            {
                GamePort = defaultGamePort;
                WebPort = defaultWebPort;
            } else
            {
                GamePort = textBox5.Text;
                WebPort = textBox6.Text;
            }

            switch (LagFix)
            {
                case "DDRAW":
                    checkBox4.Checked = true;
                    break;

                case "DXWRAPPER":
                    checkBox7.Checked = true;
                    break;

                case "DDRAWCOMPACT":
                    checkBox5.Checked = true;
                    break;

                default:
                    break;
            };

            checkBox1.Checked = window;
            checkBox2.Checked = isResize;

            if (!window)
            {
                checkBox2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button2.Enabled = false;
            }

            textBox1.Text = WinX;
            textBox2.Text = WinY;

            textBox1.KeyPress += new KeyPressEventHandler(textBox_KeyPress);
            textBox2.KeyPress += new KeyPressEventHandler(textBox_KeyPress);

            textBox1.MaxLength = 5;
            textBox2.MaxLength = 5;

            webBrowser1.Url = new Uri("https://monox.xyz/o2jam");
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Nothing atm
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    ResourceManager.DeleteDLL();

                    ResourceManager.ExtractDDL("DINPUT");
                } catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }

                checkBox2.Enabled = true;
                if (checkBox2.Checked)
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    button2.Enabled = true;
                }

                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox7.Enabled = false;

                Config.IniWriteValue("LAUNCHER", "WindowMode", "true"); 
            } else
            {
                try
                {
                    ResourceManager.DeleteDLL();

                    if (Config.IniReadValue("LAUNCHER", "LagFix") != "")
                    {
                        ResourceManager.ExtractDDL(Config.IniReadValue("LAUNCHER", "LagFix"));
                    }
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }

                checkBox2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button2.Enabled = false;

                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox7.Enabled = true;

                Config.IniWriteValue("LAUNCHER", "WindowMode", "false");
            }
        }

        private bool CheckAdministrator()
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

            if (window & isResize & !CheckAdministrator())
            {
                DialogResult res = MessageBox.Show("You're using Window Resizer and Not running launcher as admin, Window resizer may not working propertly, Continue?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            try
            {
                IPAddress[] addressList = Dns.GetHostEntry(ServerDomain).AddressList;
                ProcessArgs = string.Concat(new string[] { " 1 ", addressList[0].ToString(), " o2jam/patch ", addressList[0].ToString(), ":" + WebPort +" 1 1 1 1 1 1 1 1 ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort + " ", addressList[0].ToString(), " " + GamePort });

                ProcessStartInfo proc = new ProcessStartInfo()
                {
                    FileName = Environment.CurrentDirectory + "\\otwo.exe",
                    Arguments = ProcessArgs,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process.Start(proc);
                label8.Text = "Game running!";
                if (isResize)
                {
                    Timer1.Start();
                }
            } catch (Exception error)
            {
                ProjectData.SetProjectError(error);
                MessageBox.Show(error.ToString(), "Error");
                ProjectData.ClearProjectError();
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd;
            Process[] NoteProc = Process.GetProcessesByName("otwo");
            try
            {
                hWnd = FindWindow(ProcessName, NoteProc[0].MainWindowTitle);
                SetWindowPos(hWnd, (int)(new IntPtr(HWND_TOPMOST)), 0, 0, int.Parse(WinX), int.Parse(WinY), SWP_SHOWWINDOW);
                if (Conversions.ToBoolean(NoteProc[0].MainWindowTitle.Contains(ProcessName).ToString().ToUpper()))
                {
                    Timer1.Enabled = false;
                    label8.Text = "Config loaded!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            WinX = textBox1.Text;
            WinY = textBox2.Text;
            Config.IniWriteValue("LAUNCHER", "WinX", WinX);
            Config.IniWriteValue("LAUNCHER", "WinY", WinY);
            label8.Text = "Resolution Set!";
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                button2.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            } else
            {
                button2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                ServerDomain = "dnso2.o2jam.com.ph";
                textBox3.Enabled = false;
                button3.Enabled = false;

                textBox5.Enabled = false;
                textBox6.Enabled = false;
                button5.Enabled = false;

                Config.IniWriteValue("LAUNCHER", "DefaultServer", "true");
            } else
            {
                ServerDomain = Config.IniReadValue("LAUNCHER", "ServerDomain");
                textBox3.Enabled = true;
                button3.Enabled = true;

                textBox5.Enabled = true;
                textBox6.Enabled = true;
                button5.Enabled = true;

                Config.IniWriteValue("LAUNCHER", "DefaultServer", "false");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ServerDomain = textBox3.Text;
            Config.IniWriteValue("LAUNCHER", "ServerDomain", ServerDomain);
            label8.Text = "Server Domain Set!";
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox5.Checked = false;
                checkBox7.Checked = false;

                LagFix = "DDRAWCOMPACT";
                Config.IniWriteValue("LAUNCHER", "LagFix", LagFix);

                try
                {
                    ResourceManager.DeleteDLL();
                    ResourceManager.ExtractDDL(LagFix);
                } catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }
            } else
            {
                try
                {
                    ResourceManager.DeleteDLL();
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }

                Config.IniWriteValue("LAUNCHER", "LagFix", "");
            }
        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                checkBox4.Checked = false;
                checkBox7.Checked = false;

                LagFix = "DDRAW";
                Config.IniWriteValue("LAUNCHER", "LagFix", LagFix);

                try
                {
                    ResourceManager.DeleteDLL();
                    ResourceManager.ExtractDDL(LagFix);
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                } 
            } else
            {
                try
                {
                    ResourceManager.DeleteDLL();
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }

                Config.IniWriteValue("LAUNCHER", "LagFix", "");
            }
        }

        private void CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                checkBox4.Checked = false;
                checkBox5.Checked = false;

                LagFix = "DXWRAPPER";
                Config.IniWriteValue("LAUNCHER", "LagFix", LagFix);

                try
                {
                    ResourceManager.DeleteDLL();
                    ResourceManager.ExtractDDL(LagFix);
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                } 
            } else
            {
                try
                {
                    ResourceManager.DeleteDLL();
                }
                catch (Exception error)
                {
                    if (error.InnerException is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to change config because missing permission!", "Error");
                        return;
                    }

                    if (error.InnerException is IOException)
                    {
                        MessageBox.Show("Unable to change config because file still in use!", "Error");
                        return;
                    }
                    // silent error;
                }

                Config.IniWriteValue("LAUNCHER", "LagFix", "");
            }
        }

        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                textBox4.Enabled = false;
                button4.Enabled = false;
                ProcessName = "O2-JAM";

                Config.IniWriteValue("LAUNCHER", "DefaultProcess", "true");
            } else
            {
                textBox4.Enabled = true;
                button4.Enabled = true;
                ProcessName = Config.IniReadValue("LAUNCHER", "ProcessName");

                Config.IniWriteValue("LAUNCHER", "DefaultProcess", "false");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ProcessName = textBox4.Text;
            Config.IniWriteValue("LAUNCHER", "ProcessName", ProcessName);
            label8.Text = "ProcessName Set!";
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            GamePort = textBox5.Text;
            WebPort = textBox6.Text;

            Config.IniWriteValue("LAUNCHER", "GamePort", GamePort);
            Config.IniWriteValue("LAUNCHER", "WebPort", WebPort);

            label8.Text = "GamePort Set!";
        }
    }
}
