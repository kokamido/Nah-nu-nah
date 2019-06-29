using System;
using System.Collections.Generic;
using System.Drawing;

namespace lib
{
    public interface IDrawable
    {
        bool DrawOn(Bitmap bitmap, Tuple<int,int> point);
    }

    public class ColoredPoint
    {
        public Tuple<int, int> Coords;
        public Color Color;
    }
    
    public static class DrawHelper
    {
        public static void Draw(IEnumerable<ColoredPoint> points, Bitmap bitmap)
        {
            foreach (var p in points)
                bitmap.SetPixel(p.Coords.Item1, p.Coords.Item2, p.Color);
        }
    }
}