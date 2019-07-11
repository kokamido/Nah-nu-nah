using System;
using System.Drawing;
using System.Linq;

namespace lib
{
    public class Arrow : IDrawable
    {
        public readonly Tuple<int, int> A;
        public readonly  Tuple<int, int> B;
        public readonly Color Color;

        public Arrow(int length, int width, Color color)
        {
            A = Tuple.Create(length, width / 2);
            B = Tuple.Create(length, -width / 2);
            Color = color;
        }

        public bool DrawOn(Bitmap bitmap, Tuple<int, int> point, double angle)
        {
            MathHelper.GetFilledTriangle(A.RotateAroundZero(angle).Plus(point),B.RotateAroundZero(angle).Plus(point),point)
                .Where(p => p.Item1 >= 0 && p.Item1 < bitmap.Width && p.Item2 >= 0 && p.Item2 < bitmap.Height)
                .Select(p => new ColoredPoint{Color = Color, Coords = p})
                .DrawOn(bitmap);
            return true;
        }
    }
}