using System;
using System.IO;
using System.Windows.Forms;

namespace O2JamLauncher.ResourceMGR
{
    public class ResourceManager
    {
        public static void ExtractDDL(string type)
        {
            switch (type)
            {
                case "DDRAWCOMPACT":
                    File.WriteAllBytes(Application.StartupPath + "\\ddraw.dll", Properties.Resources.DDRAWCOMPACT);
                    break;

                case "DXWRAPPER":
                    File.WriteAllBytes(Application.StartupPath + "\\ddraw.dll", Properties.Resources.DXWRAPPER1);
                    File.WriteAllText(Application.StartupPath + "\\ddraw.ini", Properties.Resources.DXWRAPPER2);
                    File.WriteAllBytes(Application.StartupPath + "\\dxwnd.dll", Properties.Resources.DXWRAPPER3);
                    File.WriteAllText(Application.StartupPath + "\\dxwnd.ini", Properties.Resources.DXWRAPPER4);
                    File.WriteAllBytes(Application.StartupPath + "\\dxwnd.exe", Properties.Resources.DXWRAPPER5);
                    break;

                case "DDRAW":
                    File.WriteAllBytes(Application.StartupPath + "\\ddraw.dll", Properties.Resources.DDRAW1);
                    File.WriteAllText(Application.StartupPath + "\\ddraw.ini", Properties.Resources.DDRAW2);
                    break;

                case "DINPUT":
                    File.WriteAllBytes(Application.StartupPath + "\\dinput.dll", Properties.Resources.dinput);
                    break;

                default:
                    MessageBox.Show("Invalid DLL type " + type + ", Please report this error to Developer", "Error");
                    break;
            }
        }

        public static void DeleteDLL()
        {
            if (File.Exists(Application.StartupPath + "\\ddraw.dll"))
            {
                File.Delete(Application.StartupPath + "\\ddraw.dll");
            }

            if (File.Exists(Application.StartupPath + "\\ddraw.ini"))
            {
                File.Delete(Application.StartupPath + "\\ddraw.ini");
            }

            if (File.Exists(Application.StartupPath + "\\dxwnd.dll"))
            {
                File.Delete(Application.StartupPath + "\\dxwnd.dll");
            }

            if (File.Exists(Application.StartupPath + "\\dxwnd.exe"))
            {
                File.Delete(Application.StartupPath + "\\dxwnd.exe");
            }

            if (File.Exists(Application.StartupPath + "\\dxwnd.ini"))
            {
                File.Delete(Application.StartupPath + "\\dxwnd.ini");
            }

            if (File.Exists(Application.StartupPath + "\\dinput.dll"))
            {
                File.Delete(Application.StartupPath + "\\dinput.dll");
            }
        }

        public static void GenerateConfig()
        {
            File.WriteAllText(Application.StartupPath + "\\monoxjam.ini", Properties.Resources.monoxjam);
        }
    }
}