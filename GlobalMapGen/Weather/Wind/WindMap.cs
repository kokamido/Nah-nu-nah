using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;
using Settings;

namespace GlobalMap.Weather.Wind
{
    [DataContract]
    public class WindMap : IDrawable
    {
        [IgnoreDataMember] public Tuple<double, double>[,] WindVectorsMap;
        [DataMember(EmitDefaultValue = false)] private Tuple<double, double>[] windVectorsMap;

        [IgnoreDataMember] public double[,] AtmospherePressureMap;
        [DataMember(EmitDefaultValue = false)] private double[] atmospherePressureMap;
        [DataMember(EmitDefaultValue = false)] public int Width;
        [DataMember(EmitDefaultValue = false)] public int Height;
        [DataMember(EmitDefaultValue = false)] public ICollection<Cyclone> Cyclones;
        [DataMember(EmitDefaultValue = false)] public readonly Color EyeColor = Color.Magenta;
        [DataMember(EmitDefaultValue = false)] public readonly Color ArrowColor = Color.Crimson;
        [DataMember(EmitDefaultValue = false)] public readonly double HeightThresholdForWindCorrection;

        private static ILog log = LogManager.GetLogger(typeof(WindMap));

        public WindMap(ICollection<Cyclone> cyclones, int width, int height, double heightThresholdForWindCorrection,
            Map map)
        {
            Cyclones = cyclones;
            WindVectorsMap = new Tuple<double, double>[width, height];
            Width = width;
            Height = height;
            HeightThresholdForWindCorrection = heightThresholdForWindCorrection;
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                WindVectorsMap[x, y] = cyclones.Select(c => c.GetVecIn(x.MakeTupleWith(y)))
                    .Aggregate(0.0.MakeTupleWith(0.0), (acc, succ) => acc.Plus(succ));
            foreach (var cyclone in cyclones)
            {
                var eyeOfTheStormTiles = MathHelper.GetCirclePoints(cyclone.Power.Round() / 2)
                    .Select(p => p.Plus(cyclone.Eye))
                    .Where(p => p.IsInBorders(Width, Height));
                foreach (var t in eyeOfTheStormTiles)
                {
                    WindVectorsMap[t.Item1, t.Item2] = 0.0.MakeTupleWith(0.0);
                }
            }

            ApplyLandscapeCorrection(map.Altitudes);
            GeneratePressureMap();
        }

        private void ApplyLandscapeCorrection(double[,] altitudes)
        {
            log.Info("Start landscape windmap correction");
            var width = altitudes.GetLength(0);
            var height = altitudes.GetLength(1);
            if (width != Width || height != Height)
                throw new ArgumentException(
                    $"Sizes of windmap[{Width}, {Height}] and altitude map[{width}, {height}] are incompatible!");
            var circle = MathHelper.GetCirclePoints(CycloneMetaSettings.LandscapeRepulsionRadius);
            for (int x = 0; x < width; x++)
            {
                if (x % 100 == 0)
                    log.Info($"Windmap landscape correction progress is {x / (double) width}");

                for (int y = 0; y < height; y++)
                {
                    var currentPoint = x.MakeTupleWith(y);
                    if (altitudes[x, y] >= HeightThresholdForWindCorrection)
                    {
                        circle
                            .AsParallel()
                            .WithDegreeOfParallelism(Math.Min(SystemSettings.AvaliableThreads, 2))
                            .Select(p => p.Plus(currentPoint))
                            .Where(p => p.IsInBorders(width, height))
                            .ForAll(p =>
                            {
                                var wind = WindVectorsMap[p.Item1, p.Item2];
                                var fromMountainToWind = currentPoint.Minus(p);
                                var windAngle = wind.GetAngle().AngleTo0_2Pi();
                                var mToWAngle = fromMountainToWind.GetAngle().AngleTo0_2Pi();
                                WindVectorsMap[p.Item1, p.Item2] = WindVectorsMap[p.Item1, p.Item2]
                                    .RotateAroundZero(CycloneMetaSettings.HeightPointRepulsionAngle *
                                                      GetRotationSign(windAngle, mToWAngle) *
                                                      (CycloneMetaSettings.LandscapeRepulsionRadius -
                                                       MathHelper.GetEuclideanRange(currentPoint, p)) /
                                                      CycloneMetaSettings.LandscapeRepulsionRadius);
                            });
                        WindVectorsMap[x, y] = WindVectorsMap[x, y]
                            .Multiply(CycloneMetaSettings.HeightPointRepulsionPowerDecrease *
                                      Math.Abs(HeightThresholdForWindCorrection - altitudes[x, y]) /
                                      (1 - HeightThresholdForWindCorrection));
                    }
                    else
                    {
                        var endpoint = currentPoint.Plus(WindVectorsMap[currentPoint.Item1, currentPoint.Item2]
                            .Get1Vec().Multiply(CycloneMetaSettings.LandscapeTailForWindDecrease)
                            .Round());
                        if (!endpoint.IsInBorders(Width, Height))
                            continue;
                        var seg = MathHelper.GetLineSegment(currentPoint, endpoint);
                        var mul = seg.Count(p => altitudes[p.Item1, p.Item2] >= HeightThresholdForWindCorrection) /
                                  (double) seg.Count;
                        if (mul > 0)
                            WindVectorsMap[x, y] = WindVectorsMap[x, y].Multiply((1 - mul) / 2.0);
                    }
                }
            }
            log.Info("End landscape windmap correction");
            log.Info("Start windmap avg correction");
            var res = new Tuple<double, double>[Width,Height];
            WindVectorsMap.ForAll((val, x, y) =>
                {
                    var xbuf = x;
                    var ybuf = y;
                    var center = xbuf.MakeTupleWith(ybuf);
                    var pointsToAvg = MathHelper.GetCirclePoints(20).Select(p => p.Plus(center)).Where(p => p.IsInBorders(Width,Height)).ToArray();
                    res[xbuf, ybuf] = pointsToAvg.Select(p => WindVectorsMap[p.Item1, p.Item2]).Aggregate((acc, curr)=>acc.Plus(curr)).Divide(pointsToAvg.Length);
                }, SystemSettings.AvaliableThreads/2);
            WindVectorsMap = res;
            log.Info("End windmap avg correction");
        }

        private void GeneratePressureMap()
        {
            log.Info("Generating atmospheric pressure map");
            var initMap = new double[Width, Height];
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var angle = WindVectorsMap[x, y].GetAngle();
                var abs = WindVectorsMap[x, y].GetAbs();
                int xDestination = -1;
                int yDestination = -1;
                if (angle >= -Math.PI / 8 && angle < Math.PI / 8)
                {
                    xDestination = x + 1;
                    yDestination = y;
                }

                if (angle >= Math.PI / 8 && angle < 3 * Math.PI / 8)
                {
                    xDestination = x + 1;
                    yDestination = y + 1;
                }

                if (angle >= 3 * Math.PI / 8 && angle < 5 * Math.PI / 8)
                {
                    xDestination = x;
                    yDestination = y + 1;
                }

                if (angle >= 5 * Math.PI / 8 && angle < 7 * Math.PI / 8)
                {
                    xDestination = x - 1;
                    yDestination = y + 1;
                }

                if (Math.Abs(angle) >= 7 * Math.PI / 8)
                {
                    xDestination = x - 1;
                    yDestination = y;
                }

                if (angle >= -7 * Math.PI / 8 && angle < -5 * Math.PI / 8)
                {
                    xDestination = x - 1;
                    yDestination = y - 1;
                }

                if (angle >= -5 * Math.PI / 8 && angle < -3 * Math.PI / 8)
                {
                    xDestination = x;
                    yDestination = y - 1;
                }

                if (angle >= -3 * Math.PI / 8 && angle < -Math.PI / 8)
                {
                    xDestination = x + 1;
                    yDestination = y - 1;
                }

                if (xDestination >= 0 && xDestination < Width && yDestination > 0 && yDestination < Height)
                    initMap[xDestination, yDestination] += abs;
            }

            AtmospherePressureMap = initMap;

            log.Info("Atmospheric pressure map has been generated");
        }

        public void DrawOn(Bitmap bitmap)
        {
            log.Info("Drawing winds");
            foreach (var cyclone in Cyclones)
            {
                var eye = new Circle(cyclone.Power.Round(), EyeColor);
                eye.DrawOn(bitmap, cyclone.Eye, 0);
            }

            for (int x = 0; x < Width; x += 12)
            for (int y = 0; y < Height; y += 12)
            {
                var p = WindVectorsMap[x, y];
                if (p.Equals(0.0.MakeTupleWith(0.0)))
                    continue;
                var abs = p.GetAbs().Round();
                if (abs == 0)
                    continue;
                new Arrow(abs, abs / 3, ArrowColor).DrawOn(bitmap, x.MakeTupleWith(y), p.GetAngle());
            }
        }

        public Bitmap DrawPressureMap()
        {
            var maxPressure = 0.0;
            AtmospherePressureMap.ForEach((val,x,y)=> maxPressure = Math.Max(maxPressure,val));
            var res = new Bitmap(Width,Height);
            AtmospherePressureMap.ForEach((value, x, y) =>
            {
                unchecked
                {
                    var brightness = (byte) (255 * value / maxPressure);
                    res.SetPixel(x,y,Color.FromArgb(brightness,brightness,brightness));
                }
            });
            return res;
        }

        private int GetRotationSign(double w, double m)
        {
            if (w > m)
                return w - m < Math.PI ? -1 : 1;
            return m - w < Math.PI ? 1 : -1;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            WindVectorsMap = windVectorsMap.ToDoubleDimension(Width, Height);
            AtmospherePressureMap = atmospherePressureMap.ToDoubleDimension(Width, Height);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            windVectorsMap = WindVectorsMap.ToSingleDimension();
            atmospherePressureMap = AtmospherePressureMap.ToSingleDimension();
        }
    }
}