namespace GlobalMap
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var alt = MapGenerationHelper.GenerateAltitudeMap(1920, 1080);
            alt.ToBitmap(new DefaultEarthAltitude()).Save("NoiseMap.png");
        }
    }
}