using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public static class MathHelper
    {
        public static Tuple<int, int> Minus(this Tuple<int, int> left, Tuple<int, int> right)
        {
            return Tuple.Create(left.Item1 - right.Item1, left.Item2 - right.Item2);
        }
        
        public static Tuple<int, int> Plus(this Tuple<int, int> left, Tuple<int, int> right)
        {
            return Tuple.Create(left.Item1 + right.Item1, left.Item2 + right.Item2);
        }

        public static Tuple<double, double> Multiply(this Tuple<double, double> vec, double multiplier)
        {
            return Tuple.Create(vec.Item1 * multiplier, vec.Item2 * multiplier);
        }
        
        public static Tuple<int, int> Multiply(this Tuple<int, int> vec, int multiplier)
        {
            return Tuple.Create(vec.Item1 * multiplier, vec.Item2 * multiplier);
        }

        public static Tuple<int, int> Round(this Tuple<double, double> t)
        {
            return Tuple.Create((int)Math.Round(t.Item1), (int)Math.Round(t.Item2));
        }
        
        public static double GetQuantile(double[,] arr, double q)
        {
            if (q <= 0 || q > 1)
                throw new ArgumentException($"Incorrect quantile '{q}'");
            return arr
                .Cast<double>()
                .OrderBy(x => x)
                .Skip((int) Math.Floor(q * arr.Length) - 1)
                .First();
        }

        public static double GetAngle(this Tuple<int, int> radVec)
        {
            return Math.Atan(radVec.Item2 / (double) radVec.Item1);
        }
        
        public static Tuple<double,double> Get1Normal(this Tuple<int, int> vec)
        {
            var normal = Tuple.Create(-vec.Item2, vec.Item1);
            var abs = normal.GetAbs();
            return Tuple.Create(normal.Item1 / abs, normal.Item2 / abs);
        }

        public static double EuclideanRange(Tuple<int, int> p0, Tuple<int, int> p1)
        {
            return Math.Sqrt(Math.Pow(p0.Item1 - p1.Item1, 2) + Math.Pow(p0.Item2 - p1.Item2, 2));
        }

        public static double GetAbs(this Tuple<int, int> p)
        {
            return EuclideanRange(Tuple.Create(0, 0), p);
        }

        public static Tuple<int, int> RotateAroundZero(Tuple<int, int> point, double angle)
        {
            var newAngle = point.GetAngle() + angle;
            var abs = point.GetAbs();
            return Tuple.Create((int) Math.Round(abs * Math.Cos(newAngle)), (int) Math.Round(abs * Math.Sin(newAngle)));
        }

        public static List<Tuple<int, int>> GetLineSegment(Tuple<int, int> fst, Tuple<int, int> snd)
        {
            var start = fst.Item1 < snd.Item1 ? fst : snd;
            var end = fst.Item1 >= snd.Item1 ? fst : snd;
            var xDelta = Math.Abs(end.Item1 - start.Item1);
            var yDelta = end.Item2 - start.Item2;
            if (xDelta != 0 && yDelta != 0)
            {
                var multiplier = yDelta / (double) xDelta;
                var add =  start.Item1 - start.Item1 * multiplier;
                return new[]{start}.Concat(Enumerable.Range(0, xDelta)
                    .Select(n => Tuple.Create(start.Item1 + n, (int) Math.Round((start.Item1 + n) * multiplier+add)))
                    .Concat(new[]{end}))
                    .Distinct()
                    .ToList();
            }

            if (xDelta == 0 && yDelta == 0)
                return new List<Tuple<int, int>> {start};
            if (xDelta == 0)
            {
                var kokoko = Math.Min(start.Item2, end.Item2);
                return Enumerable.Range(0, Math.Abs(yDelta)+1).Select(y => Tuple.Create(start.Item1, kokoko + y)).ToList();
            }
            return Enumerable.Range(0, xDelta+1).Select(x => Tuple.Create(start.Item1 + x, start.Item2)).ToList();
        }

        public static List<Tuple<int, int>> GetFilledTriangle(Tuple<int, int> a, Tuple<int, int> b, Tuple<int, int> c)
        {
            var sorted = new[] {a, b, c}.OrderBy(p => p.Item1).ThenBy(p => p.Item2).ToArray();
            var ab = GetLineSegment(sorted[0], sorted[1]);
            var ac = GetLineSegment(sorted[0], sorted[2]);
            
            var small = ab.Count <= ac.Count ? ab : ac;
            var big = ab.Count > ac.Count ? ab : ac;
            HashSet<Tuple<int, int>> result = new HashSet<Tuple<int, int>>();

            int i = 0;
            for (; i < small.Count; i++)
                result.UnionWith(GetLineSegment(small[i], big[i]));

            for (; i < big.Count; i++)
                result.UnionWith(GetLineSegment(small.Last(), big[i]));
            return result.ToList();
        }
    }
}