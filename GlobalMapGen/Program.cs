using System.IO;
using System.Reflection;
using GlobalMap.Weather.Temperature;
using GlobalMap.Weather.Wind;
using lib;
using log4net;
using log4net.Config;

namespace GlobalMap
{
    static class GlobalMapGenerator
    {
        private static ILog log = LogManager.GetLogger(typeof(GlobalMapGenerator));
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"log4net.config"));
            log.Info("Here we go");
            var map = MapGenerator.GenerateAltitudeMap(1024, 768, new DefaultEarthAltitude());
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(4));
            map.Rivers.Add(map.GetCompatibleRiver(4));

            map.WindMap = WindMapGenerator.Generate(new CoriolisWind(), map);
            map.TemperatureMap =  new DefaultTemperature(map);

            File.WriteAllBytes("Pics/map.json",map.ToJson());
            log.Info("drawing");
            map.RedrawBitmap(MapDrawingSettings.Altitudes | MapDrawingSettings.Temperature).Save("Pics/AltAndTempMap.png");
            map.RedrawBitmap(MapDrawingSettings.Altitudes | MapDrawingSettings.Winds).Save("Pics/AltAndWind.png");
            map.RedrawBitmap(MapDrawingSettings.Temperature).Save("Pics/Temp.png");
            map.RedrawBitmap(MapDrawingSettings.Winds).Save("Pics/Wind.png");
            log.Info("drawing complete");
        }
    }
}