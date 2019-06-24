using System;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace GlobalMap
{
    public static class MapGenerationHelper
    {
        public static Map GenerateAltitudeMap(int width, int height, AltitudeGenerationSettings settings)
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
            return new Map(image, settings);
        }
    }
}