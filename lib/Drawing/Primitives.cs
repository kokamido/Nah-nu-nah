using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace lib
{
    public class Arrow : IDrawable
    {
        private List<ColoredPoint> points;
        private Tuple<int, int> nose;

        public Arrow(int length, int width, Color color)
        {
            var a = Tuple.Create(length, width / 2);
            var b = Tuple.Create(length, -width / 2);
            var c = Tuple.Create(0, 0);
            nose = Tuple.Create(0 , 0);
            points = MathHelper.GetFilledTriangle(a, b, c).Select(t => new ColoredPoint {Color = color, Coords = t})
                .ToList();
        }

        public bool DrawOn(Bitmap bitmap, Tuple<int, int> point, double angle)
        {
            var shift = point.Minus(nose);
            points.Select(p => new ColoredPoint {Color = p.Color, Coords = p.Coords.RotateAroundZero(angle).Plus(shift)})
                .Where(p => p.Coords.Item1 >= 0 && p.Coords.Item1 < bitmap.Width && p.Coords.Item2 >= 0 && p.Coords.Item2 < bitmap.Height)
                .DrawOn(bitmap);
            return true;
        }
    }
}