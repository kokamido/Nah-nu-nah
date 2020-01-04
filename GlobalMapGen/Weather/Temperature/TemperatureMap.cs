using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;
using Settings;

namespace GlobalMap.Weather.Temperature
{
    [KnownType(typeof(DefaultTemperature))]
    [DataContract]
    public abstract class TemperatureMap : IDrawable
    {
        [IgnoreDataMember] public abstract double[,] Map { get; set; }
        [DataMember(EmitDefaultValue = false)] protected double[] map;
        [DataMember(EmitDefaultValue = false)] protected abstract int Width { get; set; }
        [DataMember(EmitDefaultValue = false)] protected abstract int Height { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double TemperatureDeltaFromZero { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double TemperatureShift { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double WindAbsConvectionMultiplier { get; set; }
        public Color ColdColor => Color.Blue;
        public Color MidColor => Color.Red;
        public Color WarmColor => Color.White;

        protected static ILog Log = LogManager.GetLogger(typeof(TemperatureMap));

        public void DrawOn(Bitmap bitmap, bool needBlend)
        {
            Log.Info("Drawing temperature");
            for (int x = 0; x < bitmap.Width; x++)
            for (int y = 0; y < bitmap.Height; y++)
            {
                var color = GetColor(bitmap.GetPixel(x, y), Map[x, y],needBlend);
                bitmap.SetPixel(x, y, color);
            }
        }

        protected Color GetColor(Color bitmapPixel, double temperature, bool needBlend)
        {
            var mul = ((temperature-TemperatureShift) / TemperatureDeltaFromZero + 1) / 2;
            var res = mul >= 0.5
                ? WarmColor.Blend(MidColor, (mul - 0.5) * 2)
                : MidColor.Blend(ColdColor, mul * 2);
            return needBlend ? res.Blend(bitmapPixel, 0.85) : res;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Map = map.ToDoubleDimension(Width, Height);
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
        public override double TemperatureDeltaFromZero { get; set; }
        public override double WindAbsConvectionMultiplier { get; set; }
        public override double[,] Map { get; set; }
        protected override int Width { get; set; }
        protected override int Height { get; set; }
        public override double TemperatureShift { get; set; }

        public DefaultTemperature(Map map)
        {
            if(!map.CreatedLayers.HasFlag(MapLayer.Winds))
                throw new ArgumentException("Can't create a temperature map without windmap, you need to create it first.");
            Log.Info($"Creating new temperature map with width '{map.Width}' and height '{map.Height}'.");
            Map = new double[map.Width, map.Height];
            Width = map.Width;
            Height = map.Height;
            TemperatureDeltaFromZero = 20;
            WindAbsConvectionMultiplier = 12;
            TemperatureShift = 6.5;

            Log.Info("Generating temperature map");
            Map.ForEach((_, x, y) => Map[x, y] = GetTemperature(x, y, map.Altitudes[x, y]));
            CorrectTemperatureByWind(map);
            Log.Info("Temperature map has been created.");
        }

        private void CorrectTemperatureByWind(Map globalMap)
        {
            var map = globalMap.WindMap;
            Log.Info("Start correcting temperature map by wind");
            var res = new double[Width, Height];
            Map.ForEach((val, x, y) => res[x, y] = val);
            map.WindVectorsMap.ForEach((val, x, y) =>
            {
                if (x % 15 != 0 || y % 15 != 0)
                    return;
                var start = x.MakeTupleWith(y);
                var end = x.MakeTupleWith(y)
                    .Plus(map.WindVectorsMap[x, y].Multiply(WindAbsConvectionMultiplier))
                    .ToBorders(Width - 1, Height - 1);
                MathHelper.GetLineSegment(start, end).ForEach(p =>
                {
                    var airBatch = MathHelper.GetCirclePoints((int) Math.Min(map.WindVectorsMap[x, y].GetAbs(), 20));
                    airBatch.ForEach(batch =>
                    {
                        var p1 = p.Plus(batch);
                        var x1 = x + batch.Item1;
                        var y1 = y + batch.Item2;
                        if (!p1.Equals(start) && p1.IsInBorders(Width, Height) &&
                            x1.MakeTupleWith(y1).IsInBorders(Width, Height))
                        {
                            var delta = res[x1, y1] - res[p1.Item1, p1.Item2];
                            var diffusionCoeff = (MathHelper.GetNormalRandomNum() * 4).ToBorders(3, 6);
                            res[x1, y1] -= delta / diffusionCoeff;
                            res[p1.Item1, p1.Item2] += delta / diffusionCoeff;
                        }
                    });
                });
            });
            Map = res;
            Log.Info("First stage of correcting temperature map by wind has been finished");
            Log.Info("Start second stage of correcting temperature map. Smoothing");
            res = new double[Width, Height];
            Map.ForAll((val, x, y) =>
                {
                    var xbuf = x;
                    var ybuf = y;
                    var center = xbuf.MakeTupleWith(ybuf);
                    var pointsToAvg = MathHelper.GetCirclePoints(15).Select(p => p.Plus(center))
                        .Where(p => p.IsInBorders(Width, Height)).ToArray();
                    res[xbuf, ybuf] = pointsToAvg.Select(p => Map[p.Item1, p.Item2]).Sum() / pointsToAvg.Length;
                }, SystemSettings.AvaliableThreads / 2);
            Log.Info("Second stage of correcting temperature map has been finished");
            Map = res;
            Log.Info($"Start third stage of correcting temperature map. Shifting dealing with anomalies, shifting tamp by {TemperatureShift}");
            Map = Map.Select((t, x, y) =>
            {
                var relativeLatitude = Math.Abs(y / (Height / 2.0) - 1);
                var latitudeCorrection = relativeLatitude / 2 + 0.3;
                var temperature = (t*latitudeCorrection + GetTemperature(x, y, globalMap.Altitudes[x, y])*(1-latitudeCorrection));
                return Math.Min(TemperatureShift+TemperatureDeltaFromZero, Math.Max(-TemperatureDeltaFromZero + TemperatureShift, temperature));
            });
            Log.Info("Third stage of correcting temperature map has been finished");
        }

        private double GetTemperature(int x, int y, double altitude)
        {
            var relativeLatitude = Math.Abs(y / (Height / 2.0) - 1);
            var solarActivity = relativeLatitude < 0.233 ? 1
                 : relativeLatitude < 0.433 ? (Math.Cos((relativeLatitude - 0.233) / 0.2 * Math.PI) / 4.0 + 0.75)
                 : relativeLatitude < 0.7 ? 0.5
                 : Math.Cos((relativeLatitude - 0.7) / 0.6 * Math.PI) / 2.0;
            var relativeAltitude = (altitude - 0.455) / 0.545;
            var altitudeCorrection = -(altitude > 0.455 ? relativeAltitude * relativeAltitude * 35 : 0);
            return ( solarActivity) * (2* TemperatureDeltaFromZero + TemperatureShift) - TemperatureDeltaFromZero + altitudeCorrection;
        }
    }
}