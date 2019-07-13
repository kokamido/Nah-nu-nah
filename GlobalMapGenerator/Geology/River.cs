using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;

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

        public static River GetCompatibleRiver(this Map map, int width = 0)
        {
            Log.Info("Start creating a river");
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
            
            var riverMainstream = new List<Tuple<int, int>>();
            var source = possibleRiverSource[SharpHelper.GetUniformRndInt(0, possibleRiverSource.Count)];
            var currentTile = source;
            while (!map.IsUnderSeaLevel(currentTile))
            {
                Log.Debug($"Tile ({currentTile.Item1},{currentTile.Item2}) added to river");
                riverMainstream.Add(currentTile);
                currentTile = map.Altitudes.GetNeighbourByMinCriteria(currentTile,
                    Comparer<double>.Create((d, d1) => Math.Sign(d - d1)),riverMainstream,false);
            }
            Log.Debug($"Final river mainstream tiles {string.Join(", ",riverMainstream.Select(t => $"({t.Item1}, {t.Item2})"))}");
            Log.Info($"River mainstream has been created, final length is {riverMainstream.Count}.");
            var extendedRiver = new HashSet<Tuple<int,int>>(riverMainstream);
            if (width > 1)
            {
                Log.Info($"Extending river mainstream up to width {width}");
                double currentNum = 0;
                foreach (var t in riverMainstream)
                {
                    currentNum++;
                    extendedRiver.UnionWith(ExtendRiverTile(map,t,(int)Math.Ceiling(width*(currentNum/riverMainstream.Count))));
                }
                Log.Debug($"Extended river is {string.Join(", ",extendedRiver)}");
                Log.Info($"River has been extended. Done!");
            }
            
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