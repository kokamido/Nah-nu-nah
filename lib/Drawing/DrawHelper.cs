using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace lib
{
    public interface IPositionalDrawable
    {
        void DrawOn(Bitmap bitmap, Tuple<int,int> point, double angle);
    }

    public interface IDrawable
    {
        void DrawOn(Bitmap bitmap, bool needBlend);
    }

    public class ColoredPoint
    {
        public Tuple<int, int> Coords;
        public Color Color;

        public override string ToString() => $"({Coords.Item1}, {Coords.Item2}); {Color}";

    }

    public static class DrawHelper
    {
        public static void DrawOn(this IEnumerable<ColoredPoint> points, Bitmap bitmap)
        {
            foreach (var p in points)
                bitmap.SetPixel(p.Coords.Item1, p.Coords.Item2, p.Color);
        }

        public static void DrawText(this Bitmap bmp, string text, Color c, RectangleF rect)
        {
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.FillRectangle(Brushes.WhiteSmoke, new Rectangle((int)rect.X,(int)rect.Y,(int)rect.Width,(int)rect.Height));
            g.DrawString(text, new Font("Tahoma",10,FontStyle.Bold), new SolidBrush(c), rect);
            g.Flush();
        }
    }
}