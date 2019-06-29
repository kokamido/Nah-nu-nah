using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace lib
{
    public class Arrow : IDrawable
    {
        private List<ColoredPoint> points;

        public Arrow(Tuple<int,int> start, int length, double angle, Color color)
        {
            
        }

        public Arrow(Tuple<int, int> start, Tuple<int, int> end, Color color)
        {
            var vec = end.Minus(start);
            var len = vec.GetAbs();
            var norm = vec.Get1Normal();
            var a = start.Plus(norm.Multiply(len / 6).Round());
            var b = start.Minus(norm.Multiply(len / 6).Round());
            points = MathHelper.GetFilledTriangle(a, b, end).Select(t => new ColoredPoint {Color = color,Coords = t}).ToList();
        }
        
        public bool DrawOn(Bitmap bitmap, Tuple<int,int> point)
        {
            try
            {
                points.Select(p => new ColoredPoint {Color = p.Color, Coords = p.Coords.Plus(point)}).DrawOn(bitmap);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}