using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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
                .Select(p => new ColoredPoint{Color = Color, Coords = p})
                .DrawOn(bitmap);
            return true;
        }
    }

    public class Circle : IDrawable
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

        public bool DrawOn(Bitmap bitmap, Tuple<int, int> point, double angle)
        {
            _pointsAroundZero
                .Select(p => new ColoredPoint{Coords = p.Plus(point),Color=Color})
                .DrawOn(bitmap);
            return true;
        }
    }
}