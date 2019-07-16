using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib;
using log4net;
using log4net.Repository.Hierarchy;
using Settings;

namespace GlobalMap.Weather
{
    public class Cyclone
    {
        public Tuple<int, int> Eye;
        public double Power;
        public double Range => Power * GlobalMapGeneratorMetaSettings.CyclonePowerToRangeMultiplier;
        public bool IsAntiCyclone;
        public bool IsCounterClockwise;

        private static int _ahahaMagicMantiss = 3;
        private static int _ahahaMagicExponent = 100;
        private static int _ahahaMagicNumber => _ahahaMagicMantiss * _ahahaMagicExponent;

        private static double[] tanh = Enumerable.Range(0, _ahahaMagicNumber)
            .Select(x => Math.Tanh(_ahahaMagicMantiss - x / (double) _ahahaMagicExponent)).ToArray();

        public Tuple<double, double> GetVecIn(Tuple<int, int> point)
        {
            var range = MathHelper.EuclideanRange(point, Eye);
            if (!IsCounterClockwise)
            {
                var buf = point.Minus(Eye);
                buf = (-buf.Item1).MakeTupleWith(buf.Item2);
                point = buf.Plus(Eye);
            }

            var res = range > Range
                ? 0.0.MakeTupleWith(0.0)
                : (IsAntiCyclone
                    ? GetAntiCycloneVec(point)
                    : GetCycloneVec(point)).Get1Vec()
                .Multiply(tanh[Math.Min(_ahahaMagicNumber - 1, (range / Range * _ahahaMagicNumber).Round())] * Power);

            return IsCounterClockwise ? res : (-res.Item1).MakeTupleWith(res.Item2);
        }

        private Tuple<double, double> GetCycloneVec(Tuple<int, int> coords)
        {
            var p = coords.Minus(Eye);
            var x = p.Item1;
            var y = p.Item2;
            var power = (Power * GlobalMapGeneratorMetaSettings.CycloneEigenvalueFromPowerMultiplier - 1) / 2.0;
            return (-x + 2 * y).MakeTupleWith(-5 * x - power * y);
        }

        private Tuple<double, double> GetAntiCycloneVec(Tuple<int, int> coords)
        {
            var p = coords.Minus(Eye);
            var power = Power * GlobalMapGeneratorMetaSettings.CycloneEigenvalueFromPowerMultiplier;
            var x = p.Item1;
            var y = p.Item2;
            return (power * x + y).MakeTupleWith(-x + power * y);
        }
    }

    public class WindMap : IDrawable
    {
        public Tuple<double, double>[,] Map;
        public int Width;
        public int Height;
        public ICollection<Cyclone> Cyclones;
        public readonly Color EyeColor = Color.Magenta;
        public readonly Color ArrowColor = Color.Crimson;
        public readonly double HeightThresholdForWindCorrection;

        private ILog _log = LogManager.GetLogger(typeof(WindMap));

        public WindMap(ICollection<Cyclone> cyclones, int width, int height, double heightThresholdForWindCorrection)
        {
            Cyclones = cyclones;
            Map = new Tuple<double, double>[width, height];
            Width = width;
            Height = height;
            HeightThresholdForWindCorrection = heightThresholdForWindCorrection;
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Map[x, y] = cyclones.Select(c => c.GetVecIn(x.MakeTupleWith(y)))
                    .Aggregate(0.0.MakeTupleWith(0.0), (acc, succ) => acc.Plus(succ));
            foreach (var cyclone in cyclones)
            {
                var eyeOfTheStormTiles = MathHelper.GetCirclePoints(cyclone.Power.Round() / 2)
                    .Select(p => p.Plus(cyclone.Eye))
                    .Where(p => p.IsInBorders(Width, Height));
                foreach (var t in eyeOfTheStormTiles)
                {
                    Map[t.Item1, t.Item2] = 0.0.MakeTupleWith(0.0);
                }
            }
        }

        public void ApplyLandscapeCorrection(double[,] altitudes)
        {
            var width = altitudes.GetLength(0);
            var height = altitudes.GetLength(1);
            if (width != Width || height != Height)
                throw new ArgumentException(
                    $"Sizes of windmap[{Width}, {Height}] and altitude map[{width}, {height}] are incompatible!");
            var circle = MathHelper.GetCirclePoints(GlobalMapGeneratorMetaSettings.LandscapeRepulsionRadius);
            for (int x = 0; x < width; x++)
            {
                if (x % 10 == 0)
                    _log.Info($"Windmap landscape correction progress is {x / (double) width}");

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
                                var wind = Map[p.Item1, p.Item2];
                                var fromMountainToWind = currentPoint.Minus(p);
                                var windAngle = wind.GetAngle().AngleTo0_2Pi();
                                var mToWAngle = fromMountainToWind.GetAngle().AngleTo0_2Pi();
                                Map[p.Item1, p.Item2] = Map[p.Item1, p.Item2]
                                    .RotateAroundZero(GlobalMapGeneratorMetaSettings.HeightPointRepulsionAngle *
                                                      GetRotationSign(windAngle, mToWAngle) *
                                                      (GlobalMapGeneratorMetaSettings.LandscapeRepulsionRadius -
                                                       MathHelper.EuclideanRange(currentPoint, p)) /
                                                      GlobalMapGeneratorMetaSettings.LandscapeRepulsionRadius);
                            });
                        Map[x, y] = Map[x, y]
                            .Multiply(GlobalMapGeneratorMetaSettings.HeightPointRepulsionPowerDecrease *
                                      Math.Abs(HeightThresholdForWindCorrection - altitudes[x, y]) /
                                      (1 - HeightThresholdForWindCorrection));
                    }
                    else
                    {
                        var endpoint = currentPoint.Plus(Map[currentPoint.Item1, currentPoint.Item2]
                            .Get1Vec().Multiply(GlobalMapGeneratorMetaSettings.LandscapeTailForWindDecrease)
                            .Round());
                        if (!endpoint.IsInBorders(Width, Height))
                            continue;
                        var seg = MathHelper.GetLineSegment(currentPoint, endpoint);
                        var mul = seg.Count(p => altitudes[p.Item1, p.Item2] >= HeightThresholdForWindCorrection) /
                                  (double) seg.Count;
                        if (mul > 0)
                            Map[x, y] = Map[x, y].Multiply((1 - mul) / 2.0);
                    }
                }
            }
        }

        private int GetRotationSign(double w, double m)
        {
            if (w > m)
                return w - m < Math.PI ? -1 : 1;
            return m - w < Math.PI ? 1 : -1;
        }

        public void DrawOn(Bitmap bitmap)
        {
            foreach (var cyclone in Cyclones)
            {
                var eye = new Circle(cyclone.Power.Round(), EyeColor);
                eye.DrawOn(bitmap, cyclone.Eye, 0);
            }

            for (int x = 0; x < Width; x += 12)
            for (int y = 0; y < Height; y += 12)
            {
                var p = Map[x, y];
                if (p.Equals(0.0.MakeTupleWith(0.0)))
                    continue;
                var abs = p.GetAbs().Round();
                if (abs == 0)
                    continue;
                new Arrow(abs, abs / 3, ArrowColor).DrawOn(bitmap, x.MakeTupleWith(y), p.GetAngle());
            }
        }
    }
}