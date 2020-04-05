using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Linq;
using GlobalMap;
using GlobalMap.Weather.Biomes;
using GlobalMap.Weather.Mousture;
using GlobalMap.Weather.Temperature;
using GlobalMap.Weather.Wind;
using lib;
using log4net;
using log4net.Config;
using Objects;
using Objects.Life;
using Objects.Primitives.Materials;

namespace TestProject
{
    internal class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(@"C:\Users\Ква\Desktop\Nah-nu-nah\TestProject\log4net.config"));
            log.Info("Here we go");
            try
            {
                LifeTest();
                //MapGenTest();
            }
            catch (Exception e)
            {
                log.Fatal(e);
            }
        }

        private static void LifeTest()
        {
            var organ = Known.Organ(OrganType.BloodVessel);
            organ.ResizeTo(CreatureSize.Human);
            log.Info($"{organ.Name} has been resized to '{CreatureSize.Human}' and total volume '{organ.Volume}':\n{organ.ToJsonString()}\n");
        }

        private static void MapGenTest()
        {
            var map = MapGenerator.GenerateAltitudeMap(300, 300, new DefaultEarthAltitude());
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
            map.TemperatureMap = new DefaultTemperature(map);
            map.MoistureMap = new DefaultMoisture(map);
            map.BiomeMap = new DefaultBiomeMap(map);

            /*File.WriteAllBytes("Pics/map.pozhato", map.ToJson().Compress());
            File.WriteAllBytes("Pics/map.json", map.ToJson());*/
            log.Info($"Total biomes: {map.BiomeMap.Map.ToSingleDimension().Distinct().Count()}");
            log.Info("drawing");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Temperature).Save("Pics/AltAndTempMap.png");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Winds).Save("Pics/AltAndWind.png");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Moisture).Save("Pics/AltAndMoisture.png");
            map.RedrawBitmap(MapLayer.Altitudes | MapLayer.Biome).Save("Pics/AltAndBiome.png");
            map.RedrawBitmap(MapLayer.Temperature, false).Save("Pics/Temp.png");
            map.RedrawBitmap(MapLayer.Winds, false).Save("Pics/Wind.png");
            map.RedrawBitmap(MapLayer.Moisture, false).Save("Pics/Moisture.png");
            map.RedrawBitmap(MapLayer.Biome, false).Save("Pics/Biome.png");
            map.RedrawBitmap(MapLayer.Altitudes, false).Save("Pics/Landscape.png");
            log.Info("drawing complete");
        }
    }
}