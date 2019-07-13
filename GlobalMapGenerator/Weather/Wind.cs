using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib;

namespace GlobalMap.Weather
{
    public class Cyclone
    {
        public Tuple<int, int> Eye;
        public double Power;
        public double Range;
        public bool CounterClockwise;

        private static int _ahahaMagicMantiss = 3;
        private static int _ahahaMagicExponent = 100;
        private static int _ahahaMagicNumber => _ahahaMagicMantiss * _ahahaMagicExponent;

        private static double[] tanh = Enumerable.Range(0, _ahahaMagicNumber)
            .Select(x => Math.Tanh(_ahahaMagicMantiss - x / (double) _ahahaMagicExponent)).ToArray();

        public Tuple<double, double> GetVecIn(Tuple<int, int> point)
        {
            var range = MathHelper.EuclideanRange(Eye, point);
            var vec = point.Minus(Eye);
            var norm = vec.Get1Normal();
            if (CounterClockwise)
                    norm = 0.0.MakeTupleWith(0.0).Minus(norm);
            return range > Range
                ? Tuple.Create(0.0, 0.0)
                : norm.Multiply(tanh[Math.Min(_ahahaMagicNumber - 1, (range / Range * _ahahaMagicNumber).Round())] * Power);
        }
    }

    public class WindMap : IDrawable
    {
        public Tuple<double, double>[,] Map;
        public int Width;
        public int Height;
        public ICollection<Cyclone> Cyclones;

        public WindMap(ICollection<Cyclone> cyclones, int width, int height)
        {
            Cyclones = cyclones;
            Map = new Tuple<double, double>[width, height];
            Width = width;
            Height = height;
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Map[x, y] = cyclones.Select(c => c.GetVecIn(x.MakeTupleWith(y)))
                    .Aggregate(0.0.MakeTupleWith(0.0), (acc, succ) => acc.Plus(succ));
        }

        public void DrawOn(Bitmap bitmap)
        {
            var eye = new Circle(bitmap.Width / 100, Color.Magenta);
            foreach (var cyclone in Cyclones)
            {
                eye.DrawOn(bitmap, cyclone.Eye, 0);
            }

            for (int x = 0; x < Width; x += 25)
            for (int y = 0; y < Height; y += 25)
            {
                var p = Map[x, y];
                if (p.Equals(0.0.MakeTupleWith(0.0)))
                    continue;
                var abs = p.GetAbs().Round();
                if (abs < 10)
                    continue;
                new Arrow(abs, abs / 3, Color.Crimson).DrawOn(bitmap, x.MakeTupleWith(y),p.GetAngle());
            }
        }
    }
}