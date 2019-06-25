using System;

namespace lib
{
    public static class Itertools
    {
        public static void ForEach<T>(this T[,] arr, Action<T> action, Action<int,int> coordsCallback = null,int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;
            
            for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                action(arr[x, y]);
                coordsCallback?.Invoke(x, y);
            }
        }
        
        public static TOut[,] Select<TIn,TOut>(this TIn[,] arr, Func<TIn,TOut> func, Action<int,int> coordsCallback = null,int width = 0, int height = 0)
        {
            width = width == 0 ? arr.GetLength(0) : width;
            height = height == 0 ? arr.GetLength(1) : height;
            
            var res = new TOut[width,height];
            
            for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[x,y] = func(arr[x, y]);
                coordsCallback?.Invoke(x, y);
            }

            return res;
        }
        
    }
}