using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Drawing;
using GlobalMap.Weather.Temperature;
using SharpNoiseColor = SharpNoise.Utilities.Imaging.Color;
using SharpNoiseImage = SharpNoise.Utilities.Imaging.Image;
using GlobalMap.Weather.Wind;

namespace GlobalMap
{
    public class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double[,] Altitudes { get; private set; }
        public AltitudeGenerationSettings AltitudeGenerationSettings { get; private set; }

        public WindMap WindMap;
        public TemperatureMap TemperatureMap;

        public List<River> Rivers = new List<River>();
        public List<WaterArea> Lakes;
        public List<WaterArea> Seas;

        public bool IsUnderSeaLevel(Tuple<int, int> t) => Altitudes[t.Item1, t.Item2] < AltitudeGenerationSettings.SeaAltitude;
        public bool IsRiverTile(Tuple<int, int> t) => Rivers.Any(r => r.RiverTiles.Contains(t));
        public bool IsLakeTile(Tuple<int, int> t) => Lakes.Any(r => r.Tiles.Contains(t));
        public int Square => Width*Height;

        private Bitmap bitmap;
        public Bitmap RedrawBitmap(MapDrawingSettings settings, bool debugAltitude=false) => bitmap = ToBitmap(settings, debugAltitude);
        public Bitmap GetBitmap() => bitmap ?? (bitmap = ToBitmap(MapDrawingSettings.Altitudes| MapDrawingSettings.Winds, false));

        private ILog log = LogManager.GetLogger(typeof(Map));

        public Map(SharpNoiseImage imgGreyscale,AltitudeGenerationSettings altitudeColorSettings)
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

        private Bitmap ToBitmap(MapDrawingSettings settings, bool debug = false)
        {
            log.Info("Drawing map");
            bitmap = new Bitmap(Width, Height);

            if (settings.HasFlag(MapDrawingSettings.Altitudes))
            {
                log.Info("Drawing altitudes");
                var layers = AltitudeGenerationSettings.Layers;
                if (debug)
                {
                    layers = layers.Select((l, index) => new KeyValuePair<(double,string), Color>(l.Key, AltitudeGenerationSettings.DebugColors[index]))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }

                var layersArray = layers.ToArray();

                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    var color = GetColor(layersArray, Altitudes[x, y]);
                    bitmap.SetPixel(x, y, color);
                }

                foreach (var river in Rivers)
                {
                    foreach (var tile in river.RiverTiles)
                    {
                        bitmap.SetPixel(tile.Item1, tile.Item2, AltitudeGenerationSettings.RiverColor);
                    }
                }

                foreach (var lake in Lakes)
                {
                    foreach (var tile in lake.Tiles)
                    {
                        bitmap.SetPixel(tile.Item1, tile.Item2, AltitudeGenerationSettings.LakeColor);
                    }
                }
            }

            if (settings.HasFlag(MapDrawingSettings.Winds))
            {
                log.Info("Drawing winds");
                WindMap.DrawOn(bitmap);
            }

            if (settings.HasFlag(MapDrawingSettings.Temperature))
            {
                log.Info("Drawing temperature");
                TemperatureMap.DrawOn(bitmap);
            }

            return bitmap;
        }

        private Color GetColor(KeyValuePair<(double level, string name), Color>[] layers, double value)
        {
            for (int i = 0; i < layers.Length-1; i++)
            {
                if (layers[i + 1].Key.level > value)
                    return layers[i].Value;
            }

            return layers[layers.Length - 1].Value;
        }

        private double GreyscaleColorToDouble(SharpNoiseColor c)
        {
            return (double)c.Red / 255;
        }
    }

    [Flags]
    public enum MapDrawingSettings
    {
        Nothing = 0,
        Altitudes = 1,
        Winds = 1 << 1,
        Temperature = 1 << 2,
    }
}