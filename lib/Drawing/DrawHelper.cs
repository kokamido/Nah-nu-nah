using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;

namespace lib
{
    public interface IDrawable
    {
        bool DrawOn(Bitmap bitmap, Tuple<int,int> point, double angle);
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
    }
}