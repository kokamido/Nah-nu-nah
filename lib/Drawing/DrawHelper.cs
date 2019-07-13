using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;

namespace lib
{
    public interface IPositionalDrawable
    {
        void DrawOn(Bitmap bitmap, Tuple<int,int> point, double angle);
    }

    public interface IDrawable
    {
        void DrawOn(Bitmap bitmap);
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
                if(p.Coords.IsInBorders(bitmap.Width,bitmap.Height))
                    bitmap.SetPixel(p.Coords.Item1, p.Coords.Item2, p.Color);
        }
    }
}