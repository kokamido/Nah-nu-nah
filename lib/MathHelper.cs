using System;
using System.Linq;

namespace lib
{
    public static class MathHelper
    {
        public static double GetQuantile(double[,] arr, double q)
        {
            if (q <= 0 || q > 1)
                throw new ArgumentException($"Incorrect quantile '{q}'");
            return arr
                .Cast<double>()
                .OrderBy(x => x)
                .Skip((int) Math.Floor(q * arr.Length)-1)
                .First();
        }
    }
}