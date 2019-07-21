using System;
using log4net;
using PerlinWrapper;
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
            var altitudes = PerlinWrap.GenerateAltitudeMap(width, height);
            var res = new Map(altitudes, settings);
            Log.Info("Map has been generated");
            return res;
        }
    }
}