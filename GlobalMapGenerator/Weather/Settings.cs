using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;

namespace GlobalMap.Weather
{
    public abstract class WindMapGeneratorSettings
    {
        public abstract bool IsCounterClockWise(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords);
        public abstract bool IsAntiCyclone(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords);
        public abstract int GetPower(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords);
        public abstract Tuple<int, int> GetNexEye(Map map, List<Cyclone> previousCyclones);
        public abstract int GetCyclonesCount(Map map);
    }

    public class RandomWind : WindMapGeneratorSettings
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RandomWind));

        public override bool IsAntiCyclone(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords)
        {
            var res = SharpHelper.GetUniformRndInt(0, 2) == 0;
            Log.Info($"New wind is "+(res ? "Cyclone" : "AntiCyclone"));
            return res;
        }

        public override bool IsCounterClockWise(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords)
        {
            var res = SharpHelper.GetUniformRndInt(0, 2) == 0;
            Log.Info($"New wind is "+(res ? "counter" : "")+"clockwise");
            return res;
        }

        public override int GetPower(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords)
        {
            var res =  SharpHelper.GetUniformRndInt(map.Width / 70, map.Width / 40);
            Log.Info($"New wind's power is {res}");
            return res;
        }

        public override Tuple<int, int> GetNexEye(Map map, List<Cyclone> previousCyclones)
        {

            var res = SharpHelper.GetUniformRndInt(0, map.Width).MakeTupleWith(SharpHelper.GetUniformRndInt(0, map.Height));
            while(!map.IsUnderSeaLevel(res) || previousCyclones.Any() && previousCyclones.Min(c => MathHelper.EuclideanRange(res,c.Eye))< map.Width/4.0)
                res = SharpHelper.GetUniformRndInt(0, map.Width).MakeTupleWith(SharpHelper.GetUniformRndInt(0, map.Height));
            Log.Info($"New wind's eye is located in {res}");
            return res;
        }

        public override int GetCyclonesCount(Map map)
        {
            var res = SharpHelper.GetUniformRndInt(4, 6);
            Log.Info($"There will {res} winds in our world!");
            return res;
        }
    }
}