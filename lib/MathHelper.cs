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
            return Tuple.Create((int) Math.Round(t.Item1), (int) Math.Round(t.Item2));
        }

        public static Tuple<double, double> GetRadVec(double angle, double length)
        {
            return Tuple.Create(Math.Cos(angle), Math.Sin(angle)).Multiply(length);
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
            if (radVec.Item2 >= 0)
                return Math.Atan(radVec.Item2 / (double) radVec.Item1);
            if (radVec.Item1 < 0)
                return Math.PI + Math.Atan(radVec.Item2 / (double) radVec.Item1);
            return 2*Math.PI + Math.Atan(radVec.Item2 / (double) radVec.Item1);
        }

        public static Tuple<double, double> Get1Normal(this Tuple<int, int> vec)
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

        public static Tuple<int, int> RotateAroundZero(this Tuple<int, int> point, double angle)
        {
            var newAngle = point.GetAngle() + angle;
            var abs = point.GetAbs();
            return Tuple.Create((int) Math.Round(abs * Math.Cos(newAngle)), (int) Math.Round(abs * Math.Sin(newAngle)));
        }

        public static Tuple<int, int> ToPositiveQuad(this Tuple<int, int> t)
        {
            return Tuple.Create(Math.Abs(t.Item1),Math.Abs(t.Item2));
        }

        public static IEnumerable<Tuple<int, int>> SortPoints(this IEnumerable<Tuple<int, int>> src) =>
            src.OrderBy(p => p.Item1).ThenBy(p => p.Item2);

        public static List<Tuple<int, int>> GetLineSegment(Tuple<int, int> fst, Tuple<int, int> snd)
        {
            var start = fst.Item1 < snd.Item1 ? fst : snd;
            var end = fst.Item1 >= snd.Item1 ? fst : snd;
            var xDelta = Math.Abs(end.Item1 - start.Item1);
            var yDelta = end.Item2 - start.Item2;
            var multiplier = yDelta / (double) xDelta;
            var add = start.Item2 - start.Item1 * multiplier;
            var startToEnd = end.Minus(start);
            var shift = start.Minus(start.ToPositiveQuad());
            start = start.ToPositiveQuad();
            end = start.Plus(startToEnd);

            if (xDelta != 0 && yDelta != 0)
            {
                return new[] {start}.Concat(Enumerable.Range(1, xDelta)
                        .Select(n =>
                            Tuple.Create(start.Item1 + n, (int) Math.Round((start.Item1 + n) * multiplier + add)))
                        .Concat(new[] {end}))
                    .Distinct()
                    .Select(t => t.Plus(shift))
                    .SortPoints()
                    .ToList();
            }

            if (xDelta == 0 && yDelta == 0)
                return new List<Tuple<int, int>> {start};
            if (xDelta == 0)
            {
                var kokoko = Math.Min(start.Item2, end.Item2);
                return new[] {start}
                    .Concat(Enumerable.Range(1, Math.Abs(yDelta)).Select(y => Tuple.Create(start.Item1, kokoko + y)))
                    .Concat(new[] {end})
                    .Distinct()
                    .SortPoints()
                    .ToList();
            }
            return Enumerable.Range(0, xDelta + 1).Select(x => Tuple.Create(start.Item1 + x, start.Item2)).SortPoints().ToList();;
        }

        public static List<Tuple<int, int>> GetFilledTriangle(Tuple<int, int> a, Tuple<int, int> b, Tuple<int, int> c)
        {
            Console.WriteLine($"a {a}, b {b}, c{c}");
            var shiftX = new[]{a.Item1, b.Item1, c.Item1}.Min();
            var shiftY = new[]{a.Item2, b.Item2, c.Item2}.Min();
            var shift = Tuple.Create(shiftX, shiftY);
            Console.WriteLine("SHIFT "+shift);
            var sorted = new[] {a.Minus(shift), b.Minus(shift), c.Minus(shift)}.SortPoints().ToArray();

            var ab = GetLineSegment(sorted[0], sorted[1]);
            var ac = GetLineSegment(sorted[0], sorted[2]);
            Console.WriteLine("SORTED "+sorted.PrettyPrint());
            var small = ab.Count <= ac.Count ? ab : ac;
            var big = ab.Count > ac.Count ? ab : ac;

            Console.WriteLine("SMOL "+small.PrettyPrint());
            Console.WriteLine("BIG" + big.PrettyPrint());
            HashSet<Tuple<int, int>> result = new HashSet<Tuple<int, int>>();
            int i = 0;
            for (; i < small.Count; i++)
                result.UnionWith(GetLineSegment(small[i], big[i]));

            for (; i < big.Count; i++)
                result.UnionWith(GetLineSegment(small.Last(), big[i]));
            return result.Select(p => p.Plus(shift)).OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
        }
    }
}