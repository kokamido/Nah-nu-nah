using System;
using System.Collections.Generic;
using lib;
using log4net;

namespace GlobalMap.Weather
{
    public interface WindMapGeneratorSettings
    {
        bool IsCounterClockWise(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords);
        int GetPower(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords);
        Tuple<int, int> GetNexEye(Map map, List<Cyclone> previousCyclones);
        int GetCyclonesCount(Map map);
    }

    public class RandomWind : WindMapGeneratorSettings
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RandomWind));

        public bool IsCounterClockWise(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords)
        {
            var res = SharpHelper.GetUniformRndInt(0, 1) == 0;
            Log.Info($"New cyclone is "+(res ? "counter" : "")+"clockwise");
            return res;
        }

        public int GetPower(Map map, List<Cyclone> previousCyclones, Tuple<int, int> eyeCoords)
        {
            var res =  SharpHelper.GetUniformRndInt(map.Width / 70, map.Width / 40);
            Log.Info($"New cyclone's power is {res}");
            return res;
        }

        public Tuple<int, int> GetNexEye(Map map, List<Cyclone> previousCyclones)
        {

            var res = SharpHelper.GetUniformRndInt(0, map.Width).MakeTupleWith(SharpHelper.GetUniformRndInt(0, map.Height));
            while(!map.IsUnderSeaLevel(res))
                res = SharpHelper.GetUniformRndInt(0, map.Width).MakeTupleWith(SharpHelper.GetUniformRndInt(0, map.Height));
            Log.Info($"New cyclone's eye is located in {res}");
            return res;
        }

        public int GetCyclonesCount(Map map)
        {
            var res = SharpHelper.GetUniformRndInt(4, 6);
            Log.Info($"There will {res} cyclones in our world!");
            return res;
        }
    }
}