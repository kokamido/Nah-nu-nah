using System;
using System.Collections.Generic;
using System.Linq;
using lib;

namespace GlobalMap
{
    public class River
    {
        public Tuple<int, int>[] RiverTiles;
        public Tuple<int, int> Source => RiverTiles?.FirstOrDefault();

        public River(Tuple<int, int>[] tiles)
        {
            if (tiles == null || tiles.Length == 0)
                throw new ArgumentException(
                    $"Incorrect river tiles collection, it can't be '{tiles}' with len '{tiles?.Length}'");
            RiverTiles = tiles;
        }
    }

    public static class RiverCreator
    {
        public static River GetСompatibleRiver(this Map map)
        {
            var altitudeThreshold = MathHelper.GetQuantile(map.Altitudes, 0.75);
            var possibleRiverSource = new List<Tuple<int, int>>();
            map.Altitudes.ForEach(_ => { }, (x, y) =>
            {
                if (map.Altitudes[x, y] >= altitudeThreshold)
                    possibleRiverSource.Add(Tuple.Create(x, y));
            }, map.Width, map.Height);
            return null;
        }
    }
}