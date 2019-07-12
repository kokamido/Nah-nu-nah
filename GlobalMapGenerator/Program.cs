using System;
using System.Drawing;
using System.IO;
using System.Reflection;
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
            /* map.Rivers.Add(map.GetCompatibleRiver(2));
             map.Rivers.Add(map.GetCompatibleRiver(2));
             map.Rivers.Add(map.GetCompatibleRiver(2));
             map.Rivers.Add(map.GetCompatibleRiver(3));
             map.Rivers.Add(map.GetCompatibleRiver(3));
             map.Rivers.Add(map.GetCompatibleRiver(3));
             map.Rivers.Add(map.GetCompatibleRiver(3));
             map.Rivers.Add(map.GetCompatibleRiver(4));
             map.Rivers.Add(map.GetCompatibleRiver(4));*/
            var arrow = new Arrow(100, 30, Color.Red);
            arrow.DrawOn(map.AsBitmap, Tuple.Create(100, 150), 0);
            arrow.DrawOn(map.AsBitmap, Tuple.Create(200, 200), Math.PI / 4);
            arrow.DrawOn(map.AsBitmap, Tuple.Create(300, 250), Math.PI / 3);
            arrow.DrawOn(map.AsBitmap, Tuple.Create(400, 300), Math.PI / 2);
            map.AsBitmap.Save("NoiseMap.png");
        }
    }
}