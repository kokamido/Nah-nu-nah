using System;
using System.IO;
using System.Reflection;
using GlobalMap.Weather.Mousture;
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
            try
            {
                Sas();
            }
            catch (Exception e)
            {
                log.Fatal(e);
            }

        }

        private static void Sas()
        {
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
            map.MoistureMap = new DefaultMoisture(map);

            File.WriteAllBytes("Pics/map.json",map.ToJson());
            File.WriteAllBytes("Pics/TempMap.json",map.TemperatureMap.ToJson());
            File.WriteAllBytes("Pics/MoistureMap.json",map.MoistureMap.ToJson());
            log.Info("drawing");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Temperature).Save("Pics/AltAndTempMap.png");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Winds).Save("Pics/AltAndWind.png");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Moisture).Save("Pics/AltAndMoisture.png");
            map.RedrawBitmap(MapLayer.Temperature).Save("Pics/Temp.png");
            map.RedrawBitmap(MapLayer.Winds).Save("Pics/Wind.png");
            map.RedrawBitmap(MapLayer.Moisture).Save("Pics/Moisture.png");
            log.Info("drawing complete");
        }
    }
}