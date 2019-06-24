namespace GlobalMap
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var alt = MapGenerationHelper.GenerateAltitudeMap(1920, 1080, new DefaultEarthAltitude());
            alt.ToBitmap().Save("NoiseMap.png");
        }
    }
}