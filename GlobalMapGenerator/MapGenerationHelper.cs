using System;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace GlobalMap
{
    public static class MapGenerationHelper
    {
        public static Image GenereateMap(int width, int height, GenerationSettings settings, bool debug = false)
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

            renderer.ClearGradient();
            settings.Apply(renderer);

            renderer.Render();

            return image;
            
        }
    }
}