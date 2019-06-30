using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = SharpNoise.Utilities.Imaging.Color;
using ColorDr = System.Drawing.Color;
using Image = SharpNoise.Utilities.Imaging.Image;

namespace GlobalMap
{
    public class Map
    {
        public readonly int Width;
        public readonly int Height;
        public readonly double[,] Altitudes;
        public readonly AltitudeGenerationSettings AltitudeGenerationSettings;
        
        public List<River> Rivers = new List<River>();
        public List<WaterArea> Lakes;
        public List<WaterArea> Seas;
        
        public bool IsUnderSeaLevel(Tuple<int, int> t) => Altitudes[t.Item1, t.Item2] < AltitudeGenerationSettings.SeaAltitude;
        public bool IsRiverTile(Tuple<int, int> t) => Rivers.Any(r => r.RiverTiles.Contains(t));
        public bool IsLakeTile(Tuple<int, int> t) => Lakes.Any(r => r.Tiles.Contains(t));
        public int Square => Width*Height;

        private Bitmap bitmap;
        public Bitmap AsBitmap => bitmap ?? (bitmap = ToBitmap());

        public Map(Image imgGreyscale,AltitudeGenerationSettings altitudeColorSettings)
        {
            AltitudeGenerationSettings = altitudeColorSettings;
            Width = imgGreyscale.Width;
            Height = imgGreyscale.Height;
            Altitudes = new double[Width, Height];
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                Altitudes[x, y] = GreyscaleColorToDouble(imgGreyscale[x, y]);
            }

            this.ClassifyWaterAreas();
        }

        private Bitmap ToBitmap(bool debug = false)
        {
            var layers = AltitudeGenerationSettings.Layers;
            if (debug)
            {
                layers = layers.Select((l, index) => new KeyValuePair<(double,string), ColorDr>(l.Key, AltitudeGenerationSettings.DebugColors[index]))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            var layersArray = layers.ToArray();
            var res = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var color = GetColor(layersArray, Altitudes[x, y]);
                res.SetPixel(x, y, color);
            }

            foreach (var river in Rivers)
            {
                foreach (var tile in river.RiverTiles)
                {
                    res.SetPixel(tile.Item1, tile.Item2, AltitudeGenerationSettings.RiverColor);
                }
            }
            
            foreach (var lake in Lakes)
            {
                foreach (var tile in lake.Tiles)
                {
                    res.SetPixel(tile.Item1, tile.Item2, AltitudeGenerationSettings.LakeColor);
                }
            }

            return res;
        }

        private ColorDr GetColor(KeyValuePair<(double level, string name), ColorDr>[] layers, double value)
        {
            for (int i = 0; i < layers.Length-1; i++)
            {
                if (layers[i + 1].Key.level > value)
                    return layers[i].Value;
            }

            return layers[layers.Length - 1].Value;
        }
        
        private double GreyscaleColorToDouble(Color c)
        {
            return (double)c.Red / 255;
        }
    }
}