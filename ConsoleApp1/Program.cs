using System;
using System.Drawing.Imaging;
using System.IO;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
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

            noiseMapBuilder.SetDestSize(1920, 1080);

            noiseMapBuilder.SetBounds(-3, 3, -2, 2);

            noiseMapBuilder.Build();

            for (int i = 0; i < noiseMap.Width; i++)
            {
                for (int j = 0; j < noiseMap.Height; j++)
                {
                    Console.Write($"{noiseMap[i,j]} ");
                }
                Console.WriteLine();
            }

            var image = new Image();
            var renderer = new ImageRenderer
            {
                SourceNoiseMap = noiseMap,
                DestinationImage = image
            };

            renderer.ClearGradient();
            new DefaultEarth().Apply(renderer);

            renderer.Render();

            using (var fs = File.OpenWrite("NoiseMap.png"))
            {
                image.SaveGdiBitmap(fs, ImageFormat.Png);
            }
        }
    }
}