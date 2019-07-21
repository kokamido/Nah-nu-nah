using System;
using log4net;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace SharpNoiseWrapper
{
    public static class PerlinWrap
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PerlinWrap));
        public static double[,] GenerateAltitudeMap(int width, int height)
        {
            Log.Info("Generating noise map.");
            var noiseSource = new Perlin
            {
                Seed = new Random().Next()
            };

            var noiseMap = new NoiseMap();
            var noiseMapBuilder = new PlaneNoiseMapBuilder
            {
                DestNoiseMap = noiseMap,
                SourceModule = noiseSource
            };

            noiseMapBuilder.SetDestSize(width, height);

            noiseMapBuilder.SetBounds(-3, 3, -2, 2);

            noiseMapBuilder.Build();

            var image = new Image();
            var renderer = new ImageRenderer
            {
                SourceNoiseMap = noiseMap,
                DestinationImage = image
            };

            renderer.BuildGrayscaleGradient();
            renderer.Render();
            Log.Info("Noise map has been generated, converting it to our format");
            var res = new double[width,height];
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                res[x, y] = GreyscaleColorToDouble(image[x, y]);
            }
            Log.Info("Map has been generated");
            return res;
        }

        private static double GreyscaleColorToDouble(Color c)
        {
            return (double)c.Red / 255;
        }
    }
}