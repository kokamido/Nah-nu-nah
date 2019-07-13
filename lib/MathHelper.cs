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
            return 2 * Math.PI + Math.Atan(radVec.Item2 / (double) radVec.Item1);
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
            if (angle == 0)
                return point;
            var newAngle = point.GetAngle() + angle;
            var abs = point.GetAbs();
            return Tuple.Create((abs * Math.Cos(newAngle)).Round(), (abs * Math.Sin(newAngle)).Round());
        }

        public static IEnumerable<Tuple<int, int>> SortPoints(this IEnumerable<Tuple<int, int>> src) =>
            src.OrderBy(p => p.Item1).ThenBy(p => p.Item2);

        public static int Abs(this int a) => Math.Abs(a);
        public static double Abs(this double a) => Math.Abs(a);
        public static int Min(params int[] ints) => ints.Min();
        public static double Min(params double[] doubles) => doubles.Min();
        public static int Max(params int[] ints) => ints.Max();
        public static double Max(params double[] doubles) => doubles.Max();
        public static int Round(this double d) => (int) Math.Round(d);
        public static Tuple<int,int> MakeTupleWith(this int x, int y) => Tuple.Create(x,y);

        public static IEnumerable<Tuple<int, int>> GetNeighbours(this Tuple<int, int> p)
        {
            for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    yield return p.Plus(Tuple.Create(i, j));
        }

        public static List<Tuple<int, int>> GetLineSegment(Tuple<int, int> fst, Tuple<int, int> snd, int width = 1)
        {
            var subtract = snd.Item1 > fst.Item1 ? snd.Minus(fst) : fst.Minus(snd);

            if (fst.Item1 == snd.Item1)
                return Enumerable.Range(Min(fst.Item2, snd.Item2), subtract.Item2.Abs() + 1)
                    .Select(y => Tuple.Create(fst.Item1, y))
                    .ToList();

            if (fst.Item2 == snd.Item2)
                return Enumerable.Range(Min(fst.Item1, snd.Item1), subtract.Item1.Abs() + 1)
                    .Select(x => Tuple.Create(x, fst.Item2))
                    .ToList();

            bool xIsPrimal = subtract.Item1.Abs() >= subtract.Item2.Abs();
            double xDelta = subtract.Item1.Abs();
            var yDelta = subtract.Item2;
            var multiplier = yDelta / xDelta;
            var addition = fst.Item2 - fst.Item1 * multiplier;
            var res = xIsPrimal
                ? Enumerable.Range(Min(fst.Item1, snd.Item1), subtract.Item1.Abs() + 1)
                    .Select(x => Tuple.Create(x, (int) Math.Round(x * multiplier + addition)))
                    .ToHashSet()
                : Enumerable.Range(Min(fst.Item2, snd.Item2), subtract.Item2.Abs() + 1)
                    .Select(y => Tuple.Create((int) Math.Round((y - addition) / multiplier), y))
                    .ToHashSet();
            for (int i = 1; i < width; i++)
                res.UnionWith(res.SelectMany(p => p.GetNeighbours()).ToList());
            return res.ToList();
        }

        public static List<Tuple<int, int>> GetFilledTriangle(Tuple<int, int> a, Tuple<int, int> b, Tuple<int, int> c)
        {
            var sorted = new[]
                {
                    Tuple.Create(EuclideanRange(a, b), GetLineSegment(a, b)),
                    Tuple.Create(EuclideanRange(a, c), GetLineSegment(a, c)),
                    Tuple.Create(EuclideanRange(b, c), GetLineSegment(b, c)),
                }
                .OrderBy(t => t.Item1)
                .ToArray();
            Console.WriteLine(sorted.Select(p => p.Item1).ToList().PrettyPrint());
            var side1 = sorted[1].Item2;
            var side2 = sorted[2].Item2;
            if (EuclideanRange(side1.First(), side2.First()) > EuclideanRange(side1.First(), side2.Last()))
                side2.Reverse();
            var small = side1.Count <= side2.Count ? side1 : side2;
            var big = side1.Count > side2.Count ? side1 : side2;
            HashSet<Tuple<int, int>> result = new HashSet<Tuple<int, int>>();
            int i = 0;
            for (; i < small.Count; i++)
                result.UnionWith(GetLineSegment(small[i], big[i],2));
            for (; i < big.Count; i++)
                result.UnionWith(GetLineSegment(small.Last(), big[i],2));
            var rs = result.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
            return rs;
        }
    }
}