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

            /*var map = MapGenerator.GenerateAltitudeMap(1024, 768, new DefaultEarthAltitude());
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(4));
            map.Rivers.Add(map.GetCompatibleRiver(4));


            var arrow = new Arrow(10, 6, Color.Crimson);

            var center = Tuple.Create(512, 384);
            var circle = new Circle(25,Color.BlueViolet);
            circle.DrawOn(map.AsBitmap, center, 0);
            for (int r = 100; r <= 200; r += 25)
            {
                for (double phi = 1; phi <= 50; phi++)
                {
                    var angle = Math.PI * 2 * phi / 50;
                    var point = Tuple.Create((int) (r * Math.Cos(angle)), (int) (r * Math.Sin(angle)))
                        .Plus(center);
                    arrow.DrawOn(map.AsBitmap, point, angle + Math.PI / 2);
                }
            }
            map.AsBitmap.Save("NoiseMap.png");*/
            var b = new Bitmap(1024, 768);
            var cyc0 = new Cyclone
            {
                Eye = 512.MakeTupleWith(200),
                CounterClockwise = false,
                Power = 30,
                Range = 200
            };

            var cyc1 = new Cyclone
            {
                Eye = 612.MakeTupleWith(400),
                CounterClockwise = false,
                Power = 30,
                Range = 200
            };

            var cyc2 = new Cyclone
            {
                Eye = 412.MakeTupleWith(500),
                CounterClockwise = true,
                Power = 20,
                Range = 400,
            };
            new WindMap(new List<Cyclone>{cyc0,cyc1,cyc2},1024,768).DrawOn(b);
            b.Save("WindMap.png");
        }
    }
}