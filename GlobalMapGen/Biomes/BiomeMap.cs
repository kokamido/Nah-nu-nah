using System;
using System.Drawing;
using System.Runtime.Serialization;
using lib;
using log4net;

namespace GlobalMap.Weather.Biomes
{
    [KnownType(typeof(DefaultBiomeMap))]
    [DataContract]
    public abstract class BiomeMap : IDrawable
    {
        public abstract IBiomeSettings Settings { get; }
        protected static ILog Log = LogManager.GetLogger(typeof(BiomeMap));
        [IgnoreDataMember] public abstract BiomeType[,] Map { get; set; }
        [DataMember(EmitDefaultValue = false)] protected BiomeType[] map;

        public void DrawOn(Bitmap bitmap, bool needBlend)
        {
            Log.Info("Drawing biomes");
            Map.ForEach((val, x, y) =>
            {
                bitmap.SetPixel(x, y, needBlend ? val.GetColorOrDefault().Blend(bitmap.GetPixel(x, y), 0.75) : val.GetColorOrDefault());
            });
            var yForText = 0;
            foreach (var kvp in BiomeHelper.BiomesToColor)
            {
                bitmap.DrawText(kvp.Key.ToString(),kvp.Value,new RectangleF(0,yForText,100,yForText+50));
                yForText += 52;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Map = map.ToDoubleDimension(Map.GetLength(0), Map.GetLength(1));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            map = Map.ToSingleDimension();
        }
    }

    [DataContract]
    public class DefaultBiomeMap : BiomeMap
    {
        public override IBiomeSettings Settings { get; } = new DefaultBiomes();

        public override BiomeType[,] Map { get; set; }

        public DefaultBiomeMap(Map map)
        {
            if(!map.CreatedLayers.HasFlag(MapLayer.Temperature) || !map.CreatedLayers.HasFlag(MapLayer.Moisture))
                throw new ArgumentException("Can't create a biome map without temperature map and moisture map, you need to create it first.");
            Map = map.TemperatureMap.Map.Select((val, x, y) => Settings.GetBiome(val, map.MoistureMap.Map[x, y], map.IsWater(x.MakeTupleWith(y))));
        }
    }
}