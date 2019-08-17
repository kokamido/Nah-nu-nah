using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;
using Settings;

namespace GlobalMap.Weather.Mousture
{
    [KnownType(typeof(DefaultMoisture))]
    [DataContract]
    public abstract class MoistureMap :IDrawable
    {
        protected ILog log = LogManager.GetLogger(typeof(MoistureMap));
        public double[,] Map;
        [DataMember(EmitDefaultValue = false)] protected double[] map;

        protected Color DryColor = Color.Black;
        protected Color MoistColor = Color.White;


        public void DrawOn(Bitmap bitmap)
        {
            log.Info("Drawing moisture map");
            var min = double.MaxValue;
            var max = double.MinValue;
            Map.ForEach((val, xs, ys) =>
            {
                min = Math.Min(val, min);
                max = Math.Max(val, max);
            });
            Map.ForEach((val,x,y)=> bitmap.SetPixel(x,y,GetColor(bitmap.GetPixel(x,y),x,y,min, max)));
        }

        protected Color GetColor(Color bitmapPixel, int x, int y, double min, double max)
        {
            var mul = (Map[x, y] - min) / (max - min);
            return MoistColor.Blend(DryColor, mul).Blend(bitmapPixel, 0.75);
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            Map = map.ToDoubleDimension(Map.GetLength(0), Map.GetLength(1));
        }

        [OnSerializing]
        protected void OnSerializing(StreamingContext context)
        {
            map = Map.ToSingleDimension();
        }
    }

    [DataContract]
    public class DefaultMoisture : MoistureMap
    {
        [DataMember(EmitDefaultValue = false)]private double WindAbsConvectionMultiplier = 15;
        public DefaultMoisture(Map map)
        {
            if(!map.CreatedLayers.HasFlag(MapLayer.Winds) || !map.CreatedLayers.HasFlag(MapLayer.Temperature))
                throw new ArgumentException("Can't create a moisture map without temperature map and windmap, you need to create it first.");
            var width = map.Width;
            var height = map.Height;
            log.Info("Start generating moisture map");
            Map = GetInitialMoisture(map);
            log.Info("Start correcting moisture map by wind");
            var res = new double[width, height];
            Map.ForEach((val, x, y) => res[x, y] = val);
            map.WindMap.WindVectorsMap.ForEachWithShuffle((val, x, y) =>
            {
                if (x % 10 != 0 || y % 10 != 0)
                    return;
                var start = x.MakeTupleWith(y);
                var end = x.MakeTupleWith(y)
                    .Plus(map.WindMap.WindVectorsMap[x, y].Multiply(WindAbsConvectionMultiplier))
                    .ToBorders(width - 1, height - 1);
                MathHelper.GetLineSegment(start, end).ForEach(p =>
                {
                    var airBatch = MathHelper.GetCirclePoints((int) Math.Min(map.WindMap.WindVectorsMap[x, y].GetAbs(), 15));
                    airBatch.ForEach(batch =>
                    {
                        var p1 = p.Plus(batch);
                        var x1 = x + batch.Item1;
                        var y1 = y + batch.Item2;
                        if (!p1.Equals(start) && p1.IsInBorders(width, height) &&
                            x1.MakeTupleWith(y1).IsInBorders(width, height))
                        {
                            var delta = res[x1, y1] - res[p1.Item1, p1.Item2];
                            var diffusionCoeff = (MathHelper.GetNormalRandomNum() * 3).ToBorders(2.5, 5);
                            res[x1, y1] -= delta / diffusionCoeff;
                            res[p1.Item1, p1.Item2] += delta / diffusionCoeff;
                        }
                    });
                });
            });
            Map = res;
            log.Info("First stage of correcting moisture map by wind has been finished");
            log.Info("Start second stage of correcting moisture map. Smoothing");
            res = new double[width, height];
            Map.ForAll((val, x, y) =>
                {
                    var xbuf = x;
                    var ybuf = y;
                    var center = xbuf.MakeTupleWith(ybuf);
                    var pointsToAvg = MathHelper.GetCirclePoints(7).Select(p => p.Plus(center))
                        .Where(p => p.IsInBorders(width, height)).ToArray();
                    res[xbuf, ybuf] = pointsToAvg.Select(p => Map[p.Item1, p.Item2]).Sum() / pointsToAvg.Length;
                }, SystemSettings.AvaliableThreads / 2);
            log.Info("Second stage of correcting temperature map has been finished");
            Map = res;
            log.Info("Moisture map has been generated");
        }

        private double[,] GetInitialMoisture(Map map)
        {
            return map.TemperatureMap.Map.Select((val, x, y) =>
            {
                if (!map.IsUnderSeaLevel(x.MakeTupleWith(y)))
                    return (map.TemperatureMap.Map[x,y]+map.TemperatureMap.TemperatureDeltaFromZero-map.TemperatureMap.TemperatureShift+1)*2;
                return (val-map.TemperatureMap.TemperatureShift+map.TemperatureMap.TemperatureDeltaFromZero) * 10.6;
            });
        }
    }
}