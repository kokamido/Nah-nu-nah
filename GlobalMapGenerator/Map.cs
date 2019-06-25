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
        public double SeaAltitude => AltitudeGenerationSettings.SeaAltitude;
        public List<River> Rivers = new List<River>();

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
        }

        public Bitmap ToBitmap(bool debug = false)
        {
            var layers = AltitudeGenerationSettings.Layers;
            if (debug)
            {
                layers = layers.Select((l, index) => KeyValuePair.Create(l.Key, AltitudeGenerationSettings.DebugColors[index]))
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