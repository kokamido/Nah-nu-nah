using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;

namespace GlobalMap.Weather.Wind
{
    public abstract class CycloneFabric
    {
        public abstract Cyclone GetNewCyclone(Map map, List<Cyclone> previousCyclones);
        public abstract int GetCyclonesCount();
    }

    public class CoriolisWind : CycloneFabric
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CoriolisWind));

        public override Cyclone GetNewCyclone(Map map, List<Cyclone> previousCyclones)
        {
            var isAntiCyclone = IsAntiCyclone(map);
            var delta = MathHelper.GetNormalRandomNum() * 3;
            delta = (delta > 3 ? 3 : delta < -3 ? -3 : delta) / 3;
            var coord = GetNexEye(isAntiCyclone, map, delta, previousCyclones);
            var power = GetPower(map, delta);
            return new Cyclone
            {
                Eye = coord,
                IsAntiCyclone = isAntiCyclone,
                IsCounterClockwise = IsCounterClockWise(map, coord),
                Power = power
            };
        }

        public override int GetCyclonesCount()
        {
            var res = MathHelper.GetUniformRndInt(15, 20);
            Log.Info($"There will {res} winds in our world!");
            return res;
        }

        private bool IsCounterClockWise(Map map, Tuple<int, int> eyeCoords)
        {
            var res = eyeCoords.Item2 < map.Height / 2;
            Log.Info($"New wind is " + (res ? "counter" : "") + "clockwise");
            return res;
        }

        private bool IsAntiCyclone(Map map)
        {
            var res = MathHelper.GetUniformRndInt(0, 2) == 0;
            Log.Info($"New wind is " + (res ? "AntiCyclone" : "Cyclone"));
            return res;
        }

        private int GetPower(Map map, double delta)
        {
            var res = (int) (MathHelper.GetUniformRndInt(map.Width / 250, map.Width / 130) * (3 - Math.Abs(delta)));
            Log.Info($"New wind's power is {res}");
            return res;
        }

        private Tuple<int, int> GetNexEye(bool isAntiCyclone, Map map, double delta, List<Cyclone> previousCyclones)
        {
            var res = GenerateNewCoord(isAntiCyclone, map, delta);
            while (!map.IsUnderSeaLevel(res) || previousCyclones.Any() &&
                   previousCyclones.Min(c => MathHelper.GetEuclideanRange(res, c.Eye)) < map.Width / 40.0)
                res = GenerateNewCoord(isAntiCyclone, map, delta);
            Log.Info($"New wind's eye is located in {res}");
            return res;
        }

        private Tuple<int, int> GenerateNewCoord(bool isAntiCyclone, Map map, double delta)
        {
            var echelon = isAntiCyclone ? MathHelper.GetUniformRndInt(0, 2) : MathHelper.GetUniformRndInt(0, 3);
            var ye = map.Height / 6;
            var y = (isAntiCyclone ? (2 + 2 * echelon) * ye : (1 + 2 * echelon) * ye) + (int) (delta / 3 * ye);
            var x = MathHelper.GetUniformRndInt(0, map.Width);
            return new Tuple<int, int>(x, y);
        }
    }
}