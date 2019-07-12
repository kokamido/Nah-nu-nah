using System;
using System.Collections.Generic;
using System.Linq;
using lib;

namespace ConsoleApplication1.lib
{
    public static class TestHelper
    {
        public static bool IsEqualAsSet<T>(this ICollection<T> fst, ICollection<T> snd)
        {
            if (fst.Count != snd.Count)
            {
                Console.WriteLine($"Sets aren't equal!\n'{fst.PrettyPrint()}' \n '{snd.PrettyPrint()}'");
                return false;
            }

            foreach (var t in fst)
                snd.Remove(t);

            if (snd.Any())
            {
                Console.WriteLine($"delta isn't empty! '{snd.PrettyPrint()}'");
                return false;
            }

            return true;
        }
    }
}