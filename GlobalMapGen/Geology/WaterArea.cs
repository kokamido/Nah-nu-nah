using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;

namespace GlobalMap
{
    [DataContract]
    public class WaterArea
    {
        [DataMember(EmitDefaultValue = false)] public HashSet<Tuple<int, int>> Tiles;

        public WaterArea(HashSet<Tuple<int, int>> tiles)
        {
            Tiles = tiles;
        }
    }

    public static class WaterAreaHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WaterAreaHelper));

        public static void ClassifyWaterAreas(this Map map, double squareThreshold = 0.05)
        {
            Log.Info($"Classifying water areas");
            var lakes = new HashSet<WaterArea>();
            var seas = new HashSet<WaterArea>();
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
                    var waterArea = GetBasin(map, current);
                    lock (tilesToProcess)
                        tilesToProcess.ExceptWith(waterArea);
                    if(waterArea.Count <= thresholdAbsolute)
                        lock(lakes)
                            lakes.Add(new WaterArea(waterArea));
                    else
                        lock(seas)
                            seas.Add(new WaterArea(waterArea));
                }
            }

            var a = new Func<HashSet<Tuple<int,int>>,Tuple<int,int>>[] {t => t.FirstOrDefault(), t => t.LastOrDefault()};
            a.AsParallel().ForAll(Iter);

            Log.Info($"Lakes has been found, total {lakes.Count} water areas");
            Log.Info($"Seas has been found, total {seas.Count} water areas");
            map.Lakes = lakes.ToList();
            map.Seas = seas.ToList();
        }

        private static HashSet<Tuple<int, int>> GetBasin(Map map, Tuple<int, int> tile)
        {
            var basin = new HashSet<Tuple<int, int>> {tile};
            var toProcess = new HashSet<Tuple<int, int>> {tile};
            while (toProcess.Any())
            {
                Log.Debug($"Classifying water area from tile {tile}, current basin volume is {basin.Count}");
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