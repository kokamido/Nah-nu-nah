using System.IO;
using System.Reflection;
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
            map.Rivers.Add(map.GetCompatibleRiver(3));
            map.ToBitmap().Save("NoiseMap.png");
        }
    }
}