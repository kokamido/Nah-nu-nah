using System;
using log4net;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace GlobalMap
{
    public static class MapGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapGenerator));
        
        public static Map GenerateAltitudeMap(int width, int height, AltitudeGenerationSettings settings)
        {
            Log.Info($"Start generating a map with width '{width}', height '{height}' and altitude settings '{settings.GetType()}'");
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
            var res = new Map(image, settings);
            Log.Info("Map has been generated");
            return res;
        }
    }
}