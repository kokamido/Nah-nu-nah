using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;
using Settings;

namespace GlobalMap
{
    public class Lake
    {
        public HashSet<Tuple<int, int>> Tiles;

        public Lake(HashSet<Tuple<int, int>> tiles)
        {
            Tiles = tiles;
        }
    }

    public static class LakeHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LakeHelper));

        public static List<Lake> FindLakes(this Map map, double squareThreshold = 0.05)
        {
            Log.Info($"Finding lakes");
            var res = new List<Lake>();
            var thresholdAbsolute = map.Square * map.AltitudeGenerationSettings.LakeRelativeSquareThreshold;

            var tilesToProcess = new HashSet<Tuple<int, int>>(Enumerable.Range(0, map.Width)
                .SelectMany(x => Enumerable.Range(0, map.Height).Select(y => Tuple.Create(x, y)))
                .Where(map.IsUnderSeaLevel));

            void Iter(Func<HashSet<Tuple<int,int>>,Tuple<int,int>> selector)
            {
                while (true)
                {
                    Tuple<int, int> current;
                    lock (tilesToProcess)
                        current = selector.Invoke(tilesToProcess);
                    if (current == null)
                        break;
                    var lake = GetBasin(map, current);
                    lock (tilesToProcess)
                        tilesToProcess.ExceptWith(lake);
                    if(lake.Count <= thresholdAbsolute)
                        lock(res)
                            res.Add(new Lake(lake));
                }
            }

            var a = new Func<HashSet<Tuple<int,int>>,Tuple<int,int>>[] {t => t.FirstOrDefault(), t => t.LastOrDefault()};
            a.AsParallel().ForAll(Iter);
            
            Log.Info($"Lakes has been found, total {res.Count} water areas");
            return res;
        }

        private static HashSet<Tuple<int, int>> GetBasin(Map map, Tuple<int, int> tile)
        {
            var basin = new HashSet<Tuple<int, int>> {tile};
            var toProcess = new HashSet<Tuple<int, int>> {tile};
            while (toProcess.Any())
            {
                Log.Debug($"Finding lake from tile {tile}, current basin volume is {basin.Count}");
                var buf = new HashSet<Tuple<int, int>>();
                basin.UnionWith(toProcess);
                foreach (var t in toProcess)
                    buf.UnionWith(map.Altitudes.GetNeighboursIndexes(t).Where(map.IsUnderSeaLevel));
                buf.ExceptWith(basin);
                toProcess = buf;
            }

            return basin;
        }
    }
}