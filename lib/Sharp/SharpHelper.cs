using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using log4net;

namespace lib
{
    public static class SharpHelper
    {
        private static ILog log = LogManager.GetLogger(typeof(SharpHelper));

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEach<T>(this T[,] arr, Action<T,int,int> action,int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                action?.Invoke(arr[x, y],x,y);
            }
        }

        public static void ForAll<T>(this T[,] arr, Action<T,int,int> action, int degreeOfParallelism, int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;

            var ys = Enumerable.Range(0, height);
            Enumerable.Range(0,width).SelectMany(x => ys.Select(y => x.MakeTupleWith(y)))
                .AsParallel()
                .WithDegreeOfParallelism(degreeOfParallelism)
                .ForAll(t =>
                {
                    action?.Invoke(arr[t.Item1, t.Item2],t.Item1,t.Item2);
                });
        }

        public static TOut[,] Select<TIn, TOut>(this TIn[,] arr, Func<TIn, int, int, TOut> func, int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;

            var res = new TOut[width, height];

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[x, y] = func(arr[x, y],x,y);
            }
            return res;
        }

        public static string PrettyPrint<T>(this ICollection<T> src, string sep = ", ")
        {
            return string.Join(sep, src);
        }

        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte) (color.R * amount + backColor.R * (1 - amount));
            byte g = (byte) (color.G * amount + backColor.G * (1 - amount));
            byte b = (byte) (color.B * amount + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        public static T[] ToSingleDimension<T>(this T[,] arr)
        {
            var width = arr.GetLength(0);
            var height = arr.GetLength(1);
            var res = new T[width*height];
            for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[y * width + x] = arr[x,y];
            }
            return res;
        }

        public static T[,] ToDoubleDimension<T>(this T[] arr, int width, int height)
        {
            var res = new T[width,height];
            for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[x,y] = arr[y * width + x];
            }
            return res;
        }

        public static byte[] Compress(this byte[] data)
        {
            log.Info($"Compressing byte array of size {data.Length}");
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(this byte[] data)
        {
            log.Info($"Decompressing byte array. Compressed size is {data.Length}");
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src.Distinct());
    }
}