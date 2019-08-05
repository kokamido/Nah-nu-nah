using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;

namespace GlobalMap.Weather.Temperature
{
    [KnownType(typeof(DefaultTemperature))]
    [DataContract]
    public abstract class TemperatureMap : IDrawable
    {
        [IgnoreDataMember] public abstract double[,] Map { get; set; }
        [DataMember(EmitDefaultValue = false)] private double[] map;
        [DataMember(EmitDefaultValue = false)] protected abstract int Цidth { get; set; }
        [DataMember(EmitDefaultValue = false)] protected abstract int Рeight { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double TemperatureDeltaFromZero { get; set; }
        public Color ColdColor => Color.Blue;
        public Color WarmColor => Color.OrangeRed;

        protected static ILog Log = LogManager.GetLogger(typeof(TemperatureMap));

        public void DrawOn(Bitmap bitmap)
        {
            Log.Info("Drawing temperature");
            for (int x = 0; x < bitmap.Width; x++)
            for (int y = 0; y < bitmap.Height; y++)
            {
                var color = GetColor(bitmap.GetPixel(x, y), Map[x, y]);
                bitmap.SetPixel(x, y, color);
            }
        }

        private Color GetColor(Color bitmapPixel, double temperature)
        {
            var mul = (temperature / TemperatureDeltaFromZero + 1)/2;
            return WarmColor.Blend(ColdColor,1- mul).Blend(bitmapPixel, 0.6);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Map = map.ToDoubleDimension(Цidth, Рeight);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            map = Map.ToSingleDimension();
        }
    }

    [DataContract]
    public sealed class DefaultTemperature : TemperatureMap
    {
        private const int HeatPointOnEquatorCount = 7;
        private const double RelativeDeltaFromEquatorForHeatPointSpawn = 0.05;
        private const int ColdPointOnEachPoleCount = 4;
        private const double RelativeDeltaFromPoleLineForColdPointSpawn = 0.05;

        public override double TemperatureDeltaFromZero { get; set; }
        public override double[,] Map { get; set; }
        protected override int Цidth { get; set; }
        protected override int Рeight { get; set; }

        public DefaultTemperature(Map map)
        {
            Log.Info($"Creating new temperature map with width '{map.Width}' and height '{map.Height}'.");
            Map = new double[map.Width, map.Height];
            Цidth = map.Width;
            Рeight = map.Height;
            TemperatureDeltaFromZero = 50;
            var coldPointsNord = Enumerable.Range(0, ColdPointOnEachPoleCount)
                .Select(x => GenerateColdPoint(map, true));
            var coldPointsSouth = Enumerable.Range(0, ColdPointOnEachPoleCount)
                .Select(x => GenerateColdPoint(map, false));
            var coldPoints = coldPointsNord.Concat(coldPointsSouth).ToList();
            Log.Info($"Cold points {coldPoints.PrettyPrint()}");
            var heatPoints = Enumerable.Range(0, HeatPointOnEquatorCount)
                .Select(x => GenerateHeatPoint(map))
                .ToList();

            Log.Info($"Heat points {heatPoints.PrettyPrint()}");

            Map.ForEach((_,x, y) => Map[x, y] = GetTemperature(x, y, heatPoints, coldPoints));
            Log.Info("Temperature map has been created.");
        }

        private Tuple<int, int> GenerateHeatPoint(Map map)
        {
            var width = MathHelper.GetUniformRndInt(0,map.Width);
            var height = MathHelper.GetUniformRndInt(
                (map.Height * (0.5 - RelativeDeltaFromEquatorForHeatPointSpawn)).Round(),
                (map.Height * (0.5 + RelativeDeltaFromEquatorForHeatPointSpawn)).Round());
            return width.MakeTupleWith(height);
        }

        private Tuple<int, int> GenerateColdPoint(Map map, bool isNordPole)
        {
            var lowerBound = isNordPole
                ? 0
                : (map.Height * (1 - RelativeDeltaFromPoleLineForColdPointSpawn)).Round();

            var upperBound = isNordPole
                ? (map.Height * RelativeDeltaFromPoleLineForColdPointSpawn).Round()
                : map.Height;
            var width =  MathHelper.GetUniformRndInt(0,map.Width);
            var height = MathHelper.GetUniformRndInt(lowerBound, upperBound);
            return width.MakeTupleWith(height);
        }

        private double GetTemperature(int x, int y, IEnumerable<Tuple<int, int>> heatPoints,
            IEnumerable<Tuple<int, int>> coldPoints)
        {
            var avgRangeToHeatPoint = heatPoints
                                          .Select(p => MathHelper.GetEuclideanRange(p, x.MakeTupleWith(y)))
                                          .OrderBy(t => t)
                                          .Take(3)
                                          .Sum() / 3;
            var avgRangeToColdPoint = coldPoints
                                          .Select(p => MathHelper.GetEuclideanRange(p, x.MakeTupleWith(y)))
                                          .OrderBy(t => t)
                                          .Take(3)
                                          .Sum() / 3;
            var multiplier = avgRangeToColdPoint >= avgRangeToHeatPoint
                ? -1 + avgRangeToHeatPoint / avgRangeToColdPoint
                : 1 - avgRangeToColdPoint / avgRangeToHeatPoint;
            return TemperatureDeltaFromZero * multiplier;
        }
    }
}