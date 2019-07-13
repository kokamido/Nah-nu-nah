using System.Collections.Generic;

namespace GlobalMap.Weather
{
    public static class WindMapGenerator
    {
        public static WindMap Generate(WindMapGeneratorSettings settings, Map map)
        {
            var cyclones = new List<Cyclone>();
            var cycCount = settings.GetCyclonesCount(map);
            for (int i = 0; i < cycCount; i++)
            {
                var currentEye = settings.GetNexEye(map, cyclones);
                cyclones.Add(new Cyclone
                {
                    Eye = currentEye,
                    Power = settings.GetPower(map,cyclones,currentEye),
                    CounterClockwise = settings.IsCounterClockWise(map,cyclones,currentEye)
                });
            }
            return new WindMap(cyclones,map.Width,map.Height);
        }
    }
}