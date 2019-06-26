using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public static class SharpHelper
    {
        [ThreadStatic] private static Random rnd;

        public static int GetRndInt(int left, int right)
        {
            if(rnd == null)
                rnd = new Random();
            return rnd.Next(left, right);
        }
        public static void ForEach<T>(this T[,] arr, Action<T> action, Action<int, int> coordsCallback = null,
            int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                action(arr[x, y]);
                coordsCallback?.Invoke(x, y);
            }
        }

        public static TOut[,] Select<TIn, TOut>(this TIn[,] arr, Func<TIn, TOut> func,
            Action<int, int> coordsCallback = null, int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;

            var res = new TOut[width, height];

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[x, y] = func(arr[x, y]);
                coordsCallback?.Invoke(x, y);
            }

            return res;
        }

        public static HashSet<Tuple<int, int>> GetNeighboursIndexes<T>(this T[,] arr, Tuple<int, int> index, bool disableDiag = false)
        {
            var width = arr.GetLength(0) - 1;
            var height = arr.GetLength(1) - 1;
            var neighbours = new HashSet<Tuple<T, Tuple<int, int>>>();

            for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++)
            {
                if(disableDiag && Math.Abs(x)+Math.Abs(y)==2)
                    continue;
                var newX = Math.Min(Math.Max(index.Item1 + x, 0), width);
                var newY = Math.Min(Math.Max(index.Item2 + y, 0), height);
                neighbours.Add(Tuple.Create(arr[newX, newY], Tuple.Create(newX, newY)));
            }

            return new HashSet<Tuple<int, int>>(neighbours.Select(t => t.Item2));
        }

        public static Tuple<int, int> GetNeighbourByMinCriteria<T>(this T[,] arr, Tuple<int, int> index,
            IComparer<T> comparer, ICollection<Tuple<int, int>> exceptions = null, bool disableDiag = false)
        {
            var neighbours = GetNeighboursIndexes(arr, index,disableDiag)
                .Select(n => Tuple.Create(arr[n.Item1, n.Item2], Tuple.Create(n.Item1, n.Item2)))
                .ToList();

            return neighbours
                       .Where(n => !exceptions?.Contains(n.Item2) ?? true)
                       .OrderBy(x => x.Item1, comparer)
                       .FirstOrDefault()?.Item2
                   ?? neighbours[GetRndInt(0,neighbours.Count)].Item2;
        }
    }
}