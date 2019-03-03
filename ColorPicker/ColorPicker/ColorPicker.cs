using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColorPicker
{
    public class ColorPicker
    {
        private Color GetColor(String path)
        {
            Bitmap bitmap = new Bitmap(path);
            Int32 width = 20;
            Int32 height = width * bitmap.Height / bitmap.Width;
            Bitmap newBitmap = new Bitmap(bitmap, width, height);
            bitmap.Dispose();

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = newBitmap.LockBits(rect, ImageLockMode.ReadWrite, newBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Int32 bytes = Math.Abs(bmpData.Stride) * height;
            Byte[] sourcePixels = new Byte[bytes];
            Marshal.Copy(ptr, sourcePixels, 0, bytes);
            newBitmap.UnlockBits(bmpData);

            List<Color> pixels = new List<Color>();
            Int32 a, r, g, b;
            Int32 count = 0;
            Color color;

            for (Int32 n = 0; n < sourcePixels.Length / 4; n++)
            {
                b = sourcePixels[count++];
                g = sourcePixels[count++];
                r = sourcePixels[count++];
                a = sourcePixels[count++];
                color = Color.FromArgb(r, g, b);
                if (color.GetBrightness() < 0.8 && color.GetBrightness() > 0.3)
                {
                    pixels.Add(color);
                }
            }

            color = Color.FromArgb(0, 0, 0);
            count = 0;
            foreach (Color selectedColor in pixels)
            {
                Int32 c = pixels.Count(comparedColor => Math.Abs(comparedColor.GetHue() - selectedColor.GetHue()) < 5 && Math.Abs(comparedColor.GetSaturation() - selectedColor.GetSaturation()) < 0.2 && selectedColor.GetSaturation() > 0.2);
                if (c > count)
                {
                    count = c;
                    color = selectedColor;
                }
            }
            count = 0;
            if (color.GetBrightness() == 0)
            {
                foreach (Color selectedColor in pixels)
                {
                    Int32 c = pixels.Count(comparedColor => Math.Abs(comparedColor.GetHue() - selectedColor.GetHue()) < 5);
                    if (c > count)
                    {
                        count = c;
                        color = selectedColor;
                    }
                }
            }

            return color;
        }

        public System.Windows.Media.Color GetMediaColor(String path)
        {
            Color color = GetColor(path);
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}