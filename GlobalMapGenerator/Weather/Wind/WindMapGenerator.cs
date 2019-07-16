using System.Collections.Generic;
using System.Linq;
using lib;
using log4net;
using log4net.Repository.Hierarchy;

namespace GlobalMap.Weather
{
    public static class WindMapGenerator
    {
        private static ILog log = LogManager.GetLogger(typeof(WindMapGenerator));
        public static WindMap Generate(CycloneFabric settings, Map map)
        {
            var cyclones = new List<Cyclone>();
            var cycCount = settings.GetCyclonesCount();
            for (int i = 0; i < cycCount; i++)
            {
                cyclones.Add(settings.GetNewCyclone(map, cyclones));
            }

            var res = new WindMap(cyclones, map.Width, map.Height, map.AltitudeGenerationSettings.HeightWindBlockingThreshold);
            log.Info("Start landscape windmap correction");
            res.ApplyLandscapeCorrection(map.Altitudes);
            log.Info("End landscape windmap correction");
            return res;
        }
    }
}