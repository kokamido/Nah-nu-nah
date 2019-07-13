using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace lib
{
    public class Arrow : IPositionalDrawable
    {
        public readonly Tuple<int, int> A;
        public readonly Tuple<int, int> B;
        public readonly Tuple<int, int> C;
        public readonly Color Color;

        public Arrow(int length, int width, Color color)
        {
            A = Tuple.Create(0, width / 2);
            B = Tuple.Create(0, -width / 2);
            C = Tuple.Create(length, 0);
            Color = color;
        }

        public void DrawOn(Bitmap bitmap, Tuple<int, int> point, double angle)
        {
            angle += Math.PI;
            MathHelper.GetFilledTriangle(A.RotateAroundZero(angle).Plus(point),B.RotateAroundZero(angle).Plus(point),C.RotateAroundZero(angle).Plus(point))
                .Select(p => new ColoredPoint{Color = Color, Coords = p})
                .DrawOn(bitmap);
        }
    }

    public class Circle : IPositionalDrawable
    {
        public readonly int Radius;
        public readonly Color Color;
        private readonly HashSet<Tuple<int, int>> _pointsAroundZero;


        public Circle(int rad, Color color)
        {
            Radius = rad;
            Color = color;
            _pointsAroundZero = GetPoints();
        }

        private HashSet<Tuple<int,int>> GetPoints()
        {
            var rSq = Radius * Radius;
            var res = new HashSet<Tuple<int, int>>();
            for (int xSq = 0; xSq <= rSq; xSq++)
            for (int ySq = 0; ySq < rSq - xSq; ySq++)
            {
                var x = Math.Sqrt(xSq).Round();
                var y = Math.Sqrt(ySq).Round();
                res.Add(x.MakeTupleWith(y));
                res.Add(x.MakeTupleWith(-y));
                res.Add((-x).MakeTupleWith(y));
                res.Add((-x).MakeTupleWith(-y));
            }
            return res;
        }

        public void DrawOn(Bitmap bitmap, Tuple<int, int> point, double angle)
        {
            _pointsAroundZero
                .Select(p => new ColoredPoint{Coords = p.Plus(point),Color=Color})
                .DrawOn(bitmap);
        }
    }
}