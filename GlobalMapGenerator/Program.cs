using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using GlobalMap.Weather;
using lib;
using log4net;
using log4net.Config;

namespace GlobalMap
{
    static class GlobalMapGenerator
    {
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

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


            map.AsBitmap.Save("NoiseMap.png");
            var b = new Bitmap(1024, 768);
            var cyc0 = new Cyclone
            {
                Eye = 512.MakeTupleWith(200),
                CounterClockwise = false,
                Power = 25,
                Range = 500
            };

            var cyc1 = new Cyclone
            {
                Eye = 612.MakeTupleWith(400),
                CounterClockwise = false,
                Power = 30,
                Range = 500
            };

            var cyc2 = new Cyclone
            {
                Eye = 412.MakeTupleWith(500),
                CounterClockwise = true,
                Power = 40,
                Range = 700,
            };

            var cyc3 = new Cyclone
            {
                Eye = 100.MakeTupleWith(600),
                CounterClockwise = false,
                Power = 40,
                Range = 700,
            };
            new WindMap(new List<Cyclone>{cyc0,cyc1,cyc2,cyc3},1024,768).DrawOn(b);
            b.Save("WindMap.png");
        }
    }
}