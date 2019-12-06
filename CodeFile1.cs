using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace O2JamLauncher.tmp
{
    class Test
    {
        public static dynamic GetImage(int frame_index, int frame_num, int width, int height, int Startpos, string Files)
        {
            if (frame_index < 0 || frame_index >= frame_num)
            {
                MessageBox.Show("frame_index out of range", "Error");
                return null;
            }

            if (!File.Exists(Files))
            {
                MessageBox.Show("File is not found", "Error");
                return null;
            }

            FileStream fs = new FileStream(Files, FileMode.Open);
            BinaryReader rdr = new BinaryReader(fs);
            int t = Startpos;

            int range = (int)new FileInfo(Files).Length - Startpos;

            Bitmap tmp_bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb555);
            Rectangle rect = new Rectangle(0, 0, tmp_bitmap.Width, tmp_bitmap.Height);
            BitmapData bmpData = tmp_bitmap.LockBits(rect, ImageLockMode.ReadWrite, tmp_bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int plus = (int)ptr;
            int bytes = width * height * 2;
            Byte[] b = rdr.ReadBytes(range);
            plus -= 28;
            ptr = (IntPtr)plus;
            Marshal.Copy(b, 0, ptr, b.Length);
            tmp_bitmap.UnlockBits(bmpData);

            rdr.Close();
            fs.Dispose();
            fs.Close();
            return bmpData;
        }
    }
}