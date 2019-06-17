using System.Drawing.Imaging;
using System.IO;

namespace GlobalMap
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var image = MapGenerationHelper.GenereateMap(1980, 1080, new DefaultEarth());
            using (var fs = File.OpenWrite("NoiseMap.png"))
            {
                image.SaveGdiBitmap(fs, ImageFormat.Png);
            }
        }
    }
}