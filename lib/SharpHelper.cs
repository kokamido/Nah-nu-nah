using System;
using System.Collections.Generic;
using System.Drawing;

namespace lib
{
    public static class SharpHelper
    {
        [ThreadStatic] private static Random rnd;

        public static int GetUniformRndInt(int left, int right)
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

        public static string PrettyPrint<T>(this ICollection<T> src, string sep = ", ")
        {
            return string.Join(sep, src);
        }

        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte) ((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte) ((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte) ((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }
    }
}