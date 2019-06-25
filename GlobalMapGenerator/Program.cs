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
            var alt = MapGenerator.GenerateAltitudeMap(1920, 1080, new DefaultEarthAltitude());
            alt.ToBitmap().Save("NoiseMap.png");
        }
    }
}