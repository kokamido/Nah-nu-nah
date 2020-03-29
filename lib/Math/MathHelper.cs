using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public static class MathHelper
    {
        private static NormalRandom normalRandom = new NormalRandom(0, 1);
        [ThreadStatic] private static Random rnd;

        public static int GetUniformRndInt(int left, int right)
        {
            if (rnd == null)
                rnd = new Random();
            lock (rnd)
               return rnd.Next(left, right);
        }
        
        public static double GetUniformRndDouble(double left, double right)
        {
            if (rnd == null)
                rnd = new Random();
            lock (rnd)
                return rnd.NextDouble()*(right-left)+left;
        }

        public static double GetNormalRandomNum()
        {
            lock (normalRandom)
                return normalRandom.Next();
        }

        public static bool CheckProbability(this double p) => GetUniformRndInt(0, 100000) <= p * 100000;
        

        public static Tuple<int, int> Minus(this Tuple<int, int> left, Tuple<int, int> right)
            => Tuple.Create(left.Item1 - right.Item1, left.Item2 - right.Item2);

        public static Tuple<double, double> Minus(this Tuple<double, double> left, Tuple<double, double> right)
            => Tuple.Create(left.Item1 - right.Item1, left.Item2 - right.Item2);

        public static Tuple<double, double> Minus(this Tuple<int, int> left, Tuple<double, double> right)
            => Tuple.Create(left.Item1 - right.Item1, left.Item2 - right.Item2);

        public static Tuple<double, double> Minus(this Tuple<double, double> left, Tuple<int, int> right)
            => Tuple.Create(left.Item1 - right.Item1, left.Item2 - right.Item2);

        public static Tuple<int, int> Plus(this Tuple<int, int> left, Tuple<int, int> right)
            => Tuple.Create(left.Item1 + right.Item1, left.Item2 + right.Item2);

        public static Tuple<double, double> Plus(this Tuple<int, int> left, Tuple<double, double> right)
            => Tuple.Create(left.Item1 + right.Item1, left.Item2 + right.Item2);

        public static Tuple<double, double> Plus(this Tuple<double, double> left, Tuple<int, int> right)
            => Tuple.Create(left.Item1 + right.Item1, left.Item2 + right.Item2);

        public static Tuple<double, double> Plus(this Tuple<double, double> left, Tuple<double, double> right)
            => Tuple.Create(left.Item1 + right.Item1, left.Item2 + right.Item2);

        public static Tuple<double, double> Multiply(this Tuple<double, double> vec, double multiplier)
            => Tuple.Create(vec.Item1 * multiplier, vec.Item2 * multiplier);

        public static Tuple<int, int> Multiply(this Tuple<int, int> vec, int multiplier)
            => Tuple.Create(vec.Item1 * multiplier, vec.Item2 * multiplier);

        public static Tuple<double, double> Multiply(this Tuple<int, int> vec, double multiplier)
            => Tuple.Create(vec.Item1 * multiplier, vec.Item2 * multiplier);

        public static Tuple<double, double> Divide(this Tuple<int, int> vec, double divider)
            => Tuple.Create(vec.Item1 / divider, vec.Item2 / divider);

        public static Tuple<double, double> Divide(this Tuple<double, double> vec, double divider)
            => Tuple.Create(vec.Item1 / divider, vec.Item2 / divider);

        public static Tuple<int, int> Round(this Tuple<double, double> t)
            => Tuple.Create((int) Math.Round(t.Item1), (int) Math.Round(t.Item2));

        public static Tuple<double, double> GetRadVec(double angle, double length)
            => Tuple.Create(Math.Cos(angle), Math.Sin(angle)).Multiply(length);

        public static double ScalProd(this Tuple<double, double> fst, Tuple<double, double> snd)
            => fst.Item1 * snd.Item1 + fst.Item2 * snd.Item2;

        public static Tuple<double, double> Get1Vec(this Tuple<int, int> v) => v.Divide(v.GetAbs());
        public static Tuple<double, double> Get1Vec(this Tuple<double, double> v) => v.Divide(v.GetAbs());

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
            return Math.Atan2(radVec.Item2, radVec.Item1);
        }

        public static double GetAngle(this Tuple<double, double> radVec)
        {
            return Math.Atan2(radVec.Item2, radVec.Item1);
        }

        public static Tuple<double, double> Get1Normal(this Tuple<int, int> vec)
        {
            var normal = vec.RotateAroundZero(Math.PI / 2);
            var abs = normal.GetAbs();
            return normal.Divide(abs);
        }

        public static double GetEuclideanRange(Tuple<int, int> p0, Tuple<int, int> p1)
        {
            return Math.Sqrt(Math.Pow(p0.Item1 - p1.Item1, 2) + Math.Pow(p0.Item2 - p1.Item2, 2));
        }

        public static double GetEuclideanRange(Tuple<double, double> p0, Tuple<double, double> p1)
        {
            return Math.Sqrt(Math.Pow(p0.Item1 - p1.Item1, 2) + Math.Pow(p0.Item2 - p1.Item2, 2));
        }

        public static double GetAbs(this Tuple<int, int> p)
        {
            return GetEuclideanRange(Tuple.Create(0, 0), p);
        }

        public static double GetAbs(this Tuple<double, double> p)
        {
            return GetEuclideanRange(Tuple.Create(0.0, 0.0), p);
        }

        public static Tuple<int, int> RotateAroundZero(this Tuple<int, int> point, double angle)
        {
            if (angle == 0)
                return point;
            var newAngle = (point.GetAngle() + angle) % (2 * Math.PI);
            var abs = point.GetAbs();
            return Tuple.Create((abs * Math.Cos(newAngle)).Round(), (abs * Math.Sin(newAngle)).Round());
        }

        public static Tuple<double, double> RotateAroundZero(this Tuple<double, double> point, double angle)
        {
            if (angle == 0)
                return point;
            var newAngle = (point.GetAngle() + angle) % (2 * Math.PI);
            var abs = point.GetAbs();
            return Tuple.Create((abs * Math.Cos(newAngle)), (abs * Math.Sin(newAngle)));
        }

        public static IEnumerable<Tuple<int, int>> SortPoints(this IEnumerable<Tuple<int, int>> src) =>
            src.OrderBy(p => p.Item1).ThenBy(p => p.Item2);

        public static double AngleTo0_2Pi(this double angle) => angle >= 0 ? angle : angle + 2 * Math.PI;
        public static int Abs(this int a) => Math.Abs(a);
        public static double Abs(this double a) => Math.Abs(a);
        public static int Min(params int[] ints) => ints.Min();
        public static double Min(params double[] doubles) => doubles.Min();
        public static int Max(params int[] ints) => ints.Max();
        public static double Max(params double[] doubles) => doubles.Max();
        public static int Round(this double d) => (int) Math.Round(d);
        public static Tuple<int, int> MakeTupleWith(this int x, int y) => Tuple.Create(x, y);
        public static Tuple<double, double> MakeTupleWith(this double x, double y) => Tuple.Create(x, y);
        public static Tuple<double, double> MakeTupleWith(this int x, double y) => Tuple.Create((double) x, y);
        public static Tuple<double, double> MakeTupleWith(this double x, int y) => Tuple.Create(x, (double) y);

        public static bool IsInBorders(this Tuple<int, int> point, int width, int height)
            => point.Item1 >= 0 && point.Item1 < width && point.Item2 >= 0 && point.Item2 < height;

        public static bool IsInBorders(this Tuple<double, double> point, int width, int height)
            => point.Item1 >= 0 && point.Item1 < width && point.Item2 >= 0 && point.Item2 < height;

        public static Tuple<int, int> ToBorders(this Tuple<double, double> point, int width, int height)
            => point.Item1.ToBorders(0, width).MakeTupleWith(point.Item2.ToBorders(0, height));

        public static int ToBorders(this double val, int min, int max) => (int) Math.Min(max, Math.Max(min, val));

        public static double ToBorders(this double val, double min, double max) =>
            (int) Math.Min(max, Math.Max(min, val));

        public static IEnumerable<Tuple<int, int>> GetNeighboursIndexes(this Tuple<int, int> p)
        {
            for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    yield return p.Plus(Tuple.Create(i, j));
        }

        public static HashSet<Tuple<int, int>> GetNeighboursIndexesInBorders<T>(this T[,] arr, Tuple<int, int> index,
            bool disableDiag = false)
        {
            var width = arr.GetLength(0) - 1;
            var height = arr.GetLength(1) - 1;
            var neighbours = new HashSet<Tuple<int, int>>();

            for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++)
            {
                if (disableDiag && Math.Abs(x) + Math.Abs(y) == 2)
                    continue;
                var newX = Math.Min(Math.Max(index.Item1 + x, 0), width);
                var newY = Math.Min(Math.Max(index.Item2 + y, 0), height);
                neighbours.Add(Tuple.Create(newX, newY));
            }

            return neighbours;
        }

        public static Tuple<int, int> GetNeighbourByMinCriteria<T>(this T[,] arr, Tuple<int, int> index,
            IComparer<T> comparer, ICollection<Tuple<int, int>> exceptions = null, bool disableDiag = false)
        {
            var neighbours = GetNeighboursIndexesInBorders(arr, index, disableDiag)
                .Select(n => Tuple.Create(arr[n.Item1, n.Item2], Tuple.Create(n.Item1, n.Item2)))
                .ToList();

            return neighbours
                       .Where(n => !exceptions?.Contains(n.Item2) ?? true)
                       .OrderBy(x => x.Item1, comparer)
                       .FirstOrDefault()?.Item2
                   ?? neighbours[GetUniformRndInt(0, neighbours.Count)].Item2;
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
                res.UnionWith(res.SelectMany(p => p.GetNeighboursIndexes()).ToList());
            return res.ToList();
        }

        public static HashSet<Tuple<int, int>> GetFilledTriangle(Tuple<int, int> a, Tuple<int, int> b,
            Tuple<int, int> c)
        {
            var sideLengths = new[] {GetEuclideanRange(a, b), GetEuclideanRange(a, c), GetEuclideanRange(b, c)};
            double quant = 1.0 / sideLengths.Max();
            var res = new HashSet<Tuple<int, int>>();
            for (double i = 0; i <= 1; i += quant)
            for (double j = 0; j <= 1 - i; j += quant)
                res.Add(a.Multiply(i).Plus(b.Multiply(j)).Plus(c.Multiply(1 - i - j)).Round());
            return res;
        }

        public static HashSet<Tuple<int, int>> GetCirclePoints(int radius)
        {
            var rSq = radius * radius;
            var res = new HashSet<Tuple<int, int>>();
            for (int x = 0; x <= radius; x++)
            for (int y = 0; y < Math.Sqrt(rSq - x * x).Round(); y++)
            {
                res.Add(x.MakeTupleWith(y));
                res.Add(x.MakeTupleWith(-y));
                res.Add((-x).MakeTupleWith(y));
                res.Add((-x).MakeTupleWith(-y));
            }

            return res;
        }

        private static Func<double, double> GetLineEquation(Tuple<int, int> fst, Tuple<int, int> snd)
        {
            if (snd.Item2 == fst.Item2)
                throw new ArgumentException($"Can't create the equation for points {fst} and {snd}");
            var k = (snd.Item2 - fst.Item2) / (snd.Item1 - fst.Item1);
            var b = fst.Item2 - k * fst.Item1;
            return x => k * x + b;
        }

        public static bool IsPointInsidePolygon(this Tuple<int, int> point, List<Tuple<int, int>> polygon)
        {
            int res = 1;
            for (int i = 0; i < polygon.Count - 1; i++)
                res *= ZaloopaFromHabr(polygon[i], polygon[i + 1], point);
            return res * ZaloopaFromHabr(polygon.Last(), polygon.First(), point) < 1;
        }

        private static int ZaloopaFromHabr(Tuple<int, int> a, Tuple<int, int> b, Tuple<int, int> middle)
        {
            long ax = a.Item1 - middle.Item1;
            long ay = a.Item2 - middle.Item2;
            long bx = b.Item1 - middle.Item1;
            long by = b.Item2 - middle.Item2;
            int s = Math.Sign(ax * by - ay * bx);
            if (s == 0 && (ay == 0 || by == 0) && ax * bx <= 0)
                return 0;
            if (ay < 0 ^ by < 0)
                return by < 0 ? s : -s;
            return 1;
        }
    }
}