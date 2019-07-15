using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;

namespace GlobalMap.Weather
{
    public abstract class WindMapGeneratorSettings
    {
        public abstract Cyclone GetNewCyclone(Map map, List<Cyclone> previosCyclones);
        public abstract int GetCyclonesCount(Map map);
    }

    public class CoriolisWind : WindMapGeneratorSettings
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CoriolisWind));
        private static readonly NormalRandom NormalRandom = new NormalRandom(0, 3);
        
        private bool IsCounterClockWise(Map map, Tuple<int, int> eyeCoords)
        {
            var res = eyeCoords.Item2 < map.Height / 2;
            Log.Info($"New wind is "+(res ? "counter" : "")+"clockwise");
            return res;
        }

        private bool IsAntiCyclone(Map map)
        {
            var res = SharpHelper.GetUniformRndInt(0, 2) == 0;
            Log.Info($"New wind is "+(res ? "AntiCyclone" : "Cyclone"));
            return res;
        }

        private int GetPower(Map map,  double delta)
        {
            var res =  (int)(SharpHelper.GetUniformRndInt(map.Width / 250, map.Width / 130) * (3- Math.Abs(delta)));
            Log.Info($"New wind's power is {res}");
            return res ;
        }

        private Tuple<int, int> GetNexEye(bool isAntiCyclone, Map map, double delta, List<Cyclone> previousCyclones)
        {
            var res = GenerateNewCoord(isAntiCyclone, map, delta);
            while(!map.IsUnderSeaLevel(res) || previousCyclones.Any() && previousCyclones.Min(c => MathHelper.EuclideanRange(res,c.Eye))< map.Width/40.0)
                res = GenerateNewCoord(isAntiCyclone, map, delta);
            Log.Info($"New wind's eye is located in {res}");
            return res;
        }

        private Tuple<int, int> GenerateNewCoord(bool isAntiCyclone, Map map, double delta)
        {
            var echelon = isAntiCyclone ? SharpHelper.GetUniformRndInt(0, 2): SharpHelper.GetUniformRndInt(0, 3);
            var ye = map.Height / 6;
            var y = (isAntiCyclone? (2+2*echelon)*ye:(1+2*echelon)*ye )+ (int)(delta/3 * ye);
            
            var x = SharpHelper.GetUniformRndInt(0, map.Width);
            return new Tuple<int,int>(x,y);
        }
        
        

        public override Cyclone GetNewCyclone(Map map, List<Cyclone> previosCyclones)
        {
            var isAntiCyclone = IsAntiCyclone(map);
            var delta = NormalRandom.Next();
            delta = (delta > 3 ? 3 : delta < -3 ? -3 : delta) / 3;
            var coord = GetNexEye(isAntiCyclone, map, delta, previosCyclones);
            var power = GetPower(map, delta);
            return new Cyclone
            {
                Eye = coord,
                IsAntiCyclone = isAntiCyclone,
                IsCounterClockwise = IsCounterClockWise(map, coord),
                Power = power
            };
        }
       

        public override int GetCyclonesCount(Map map)
        {
            var res = SharpHelper.GetUniformRndInt(15, 20);
            Log.Info($"There will {res} winds in our world!");
            return res;
        }
    }
    internal class NormalRandom
    {
        private readonly Random _randi;
        private readonly double m;
        private readonly double t;
        private double? _buffer;

        public NormalRandom(double m, double t, int? seed = null)
        {
            _randi = seed.HasValue? new Random(seed.Value): new Random();
            this.m = m;
            this.t = t;
            _buffer = null;
        }

        public double Next()
        {
            if (_buffer.HasValue)
            {
                var rs = _buffer.Value;
                _buffer = null;
                return rs;
            }
            var (z1, z2) = GetNormalRandom(_randi.NextDouble(), _randi.NextDouble());
            _buffer = m + t * z2;
            return m + t * z1;

        }

        private (double, double) GetNormalRandom(double r, double t)
        {
            return (Math.Sqrt(-2*Math.Log(r)) * Math.Sin(2*Math.PI*t), Math.Sqrt(-2*Math.Log(r)) * Math.Cos(2*Math.PI*t));
        }
    }
}