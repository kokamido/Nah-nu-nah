using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Drawing;
using System.Runtime.Serialization;
using GlobalMap.Weather.Biomes;
using GlobalMap.Weather.Mousture;
using GlobalMap.Weather.Temperature;
using GlobalMap.Weather.Wind;
using lib;

namespace GlobalMap
{
    [DataContract]
    public class Map : IDrawable
    {
        [DataMember(EmitDefaultValue = false)] public int Width { get; private set; }
        [DataMember(EmitDefaultValue = false)] public int Height { get; private set; }
        [IgnoreDataMember] public double[,] Altitudes { get; private set; }
        [DataMember(EmitDefaultValue = false)] private double[] altitudes;
        [DataMember(EmitDefaultValue = false)] public AltitudeGenerationSettings AltitudeGenerationSettings { get; private set; }

        [DataMember(EmitDefaultValue = false)] public WindMap WindMap;
        [DataMember(EmitDefaultValue = false)] public TemperatureMap TemperatureMap;
        [DataMember(EmitDefaultValue = false)] public MoistureMap MoistureMap;
        [DataMember(EmitDefaultValue = false)] public BiomeMap BiomeMap;

        [DataMember(EmitDefaultValue = false)] public List<River> Rivers = new List<River>();
        [DataMember(EmitDefaultValue = false)] public List<WaterArea> Lakes;
        [DataMember(EmitDefaultValue = false)] public List<WaterArea> Seas;

        [DataMember(EmitDefaultValue = false)] public MapLayer CreatedLayers
        {
            get
            {
                var res = MapLayer.Nothing;
                if (Altitudes != null)
                    res |= MapLayer.Altitudes;
                if (WindMap != null)
                    res |= MapLayer.Winds;
                if (TemperatureMap != null)
                    res |= MapLayer.Temperature;
                if (MoistureMap != null)
                    res |= MapLayer.Moisture;
                if (BiomeMap != null)
                    res |= MapLayer.Biome;
                return res;
            }
        }

        public bool IsUnderSeaLevel(Tuple<int, int> t) => Altitudes[t.Item1, t.Item2] < AltitudeGenerationSettings.SeaAltitude;
        public bool IsRiverTile(Tuple<int, int> t) => Rivers.Any(r => r.RiverTiles.Contains(t));
        public bool IsLakeTile(Tuple<int, int> t) => Lakes.Any(r => r.Tiles.Contains(t));
        public int Square => Width*Height;

        [IgnoreDataMember] private Bitmap bitmap;
        public Bitmap RedrawBitmap(MapLayer settings, bool debugAltitude=false) => bitmap = ToBitmap(settings, debugAltitude);
        public Bitmap GetBitmap() => bitmap ?? (bitmap = ToBitmap(MapLayer.Altitudes| MapLayer.Winds, false));

        private static ILog log = LogManager.GetLogger(typeof(Map));

        public Map(double[,] altitudes,AltitudeGenerationSettings altitudeColorSettings)
        {
            AltitudeGenerationSettings = altitudeColorSettings;
            Altitudes = altitudes;
            Width = Altitudes.GetLength(0);
            Height = Altitudes.GetLength(1);
            this.ClassifyWaterAreas();
        }

        private Bitmap ToBitmap(MapLayer settings, bool debug = false)
        {
            log.Info("Drawing map");
            bitmap = new Bitmap(Width, Height);

            if (settings.HasFlag(MapLayer.Altitudes))
                DrawOn(bitmap);

            if (settings.HasFlag(MapLayer.Winds))
                WindMap.DrawOn(bitmap);

            if (settings.HasFlag(MapLayer.Temperature))
                TemperatureMap.DrawOn(bitmap);

            if (settings.HasFlag(MapLayer.Moisture))
                MoistureMap.DrawOn(bitmap);

            if (settings.HasFlag(MapLayer.Biome))
                BiomeMap.DrawOn(bitmap);

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

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Altitudes = altitudes.ToDoubleDimension(Width, Height);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            altitudes = Altitudes.ToSingleDimension();
        }

        public void DrawOn(Bitmap bitmap)
        {
            log.Info("Drawing altitudes");
            var layers = AltitudeGenerationSettings.Layers;
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
    }

    [Flags]
    public enum MapLayer
    {
        Nothing = 0,
        Altitudes = 1,
        Winds = 1 << 1,
        Temperature = 1 << 2,
        Moisture = 1 << 3,
        Biome = 1 << 4,
    }
}