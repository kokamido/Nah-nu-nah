using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using lib;
using log4net;
using log4net.Repository.Hierarchy;

namespace GlobalMap
{
    public class River
    {
        public HashSet<Tuple<int, int>> RiverTiles;
        public Tuple<int, int> Source;

        public River(HashSet<Tuple<int, int>> tiles, Tuple<int, int> source)
        {
            if (tiles == null || tiles.Count == 0)
                throw new ArgumentException(
                    $"Incorrect river tiles collection, it can't be '{tiles}' with len '{tiles?.Count}'");
            RiverTiles = tiles;
            Source = source;
        }
    }

    public static class RiverGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RiverGenerator));
        [ThreadStatic] private static Random rnd;
       
        public static River GetCompatibleRiver(this Map map, int width = 0)
        {
            Log.Info($"Start creating a river");
            if(rnd == null)
                rnd = new Random();
            
            var altitudeThreshold = MathHelper.GetQuantile(map.Altitudes, 0.9);
            var possibleRiverSource = new List<Tuple<int, int>>();
            map.Altitudes.ForEach(
                _ => { },
                (x, y) =>
                {
                    if (map.Altitudes[x, y] >= altitudeThreshold)
                        possibleRiverSource.Add(Tuple.Create(x, y));
                },
                map.Width, 
                map.Height);
            
            var riverTiles = new HashSet<Tuple<int, int>>();
            var source = possibleRiverSource[rnd.Next(0, possibleRiverSource.Count)];
            var currentTile = source;
            while (map.Altitudes[currentTile.Item1,currentTile.Item2] > map.SeaAltitude)
            {
                Log.Debug($"Tile ({currentTile.Item1},{currentTile.Item2}) added to river");
                riverTiles.Add(currentTile);
                currentTile = map.Altitudes.GetNeighbourByMinCriteria(currentTile,
                    Comparer<double>.Create((d, d1) => Math.Sign(d - d1)),riverTiles,false);
            }
            Log.Debug($"Final river mainstream tiles {string.Join(", ",riverTiles.Select(t => $"({t.Item1}, {t.Item2})"))}");
            Log.Info($"River mainstream has been created, final length is {riverTiles.Count}. Start extension to width {width}");
            var extendedRiver = new HashSet<Tuple<int,int>>();
            foreach (var t in riverTiles)
            {
                extendedRiver.UnionWith(ExtendRiverTile(map,t,width));
            }
            Log.Debug($"Extended river is {string.Join(", ",extendedRiver)}");
            Log.Info($"River has been extended. Done!");
            return new River(extendedRiver, source);
        }

        private static HashSet<Tuple<int, int>> ExtendRiverTile(Map map, Tuple<int, int> tile, int width)
        {
            Log.Debug($"Extending river tile {tile} with width {width}");
            var res = new HashSet<Tuple<int,int>>{tile};
            for (int i = 0; i < width; i++)
            {
                var buf = new HashSet<Tuple<int,int>>();
                foreach (var t in res)
                    buf.UnionWith(map.Altitudes.GetNeighboursIndexes(t));
                res.UnionWith(buf);
            }
            Log.Debug($"River tile {tile} has been extended to {string.Join(", ",res)}");
            return res;
        }
    }
}