using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GlobalMap
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var alt = MapGenerationHelper.GenereateAltitudeMap(1024, 768);
            alt.ToBitmap(new DefaultEarth()).Save("NoiseMap.png");
        }
    }
}