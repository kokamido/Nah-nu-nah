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
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(2));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.Rivers.Add(map.GetCompatibleRiver(4));
            map.Rivers.Add(map.GetCompatibleRiver(4));
            new Arrow(25,10,Color.Red).DrawOn(map.AsBitmap,Tuple.Create(100,100),0);
            new Arrow(25,10,Color.Red).DrawOn(map.AsBitmap,Tuple.Create(100,200),Math.PI/4);
            new Arrow(25,10,Color.Red).DrawOn(map.AsBitmap,Tuple.Create(100,300),Math.PI/2);
            new Arrow(25,10,Color.Red).DrawOn(map.AsBitmap,Tuple.Create(100,400),Math.PI/3);
            map.AsBitmap.Save("NoiseMap.png");
        }
    }
}