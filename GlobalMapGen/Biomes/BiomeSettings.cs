using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using lib;

namespace GlobalMap.Weather.Biomes
{
    public interface IBiomeSettings
    {
        BiomeType GetBiome(double temperature, double moisture, bool IsUnderwater);
    }

    public class DefaultBiomes : IBiomeSettings
    {
        private Dictionary<BiomeType, List<Tuple<int, int>>> biomePolygons =
            new Dictionary<BiomeType, List<Tuple<int,int>>>
            {
                [BiomeType.АрктическаяПустыня] = new List<Tuple<int, int>>{Tuple.Create(-16,18),Tuple.Create(-5,18),Tuple.Create(0,0),Tuple.Create(-20,0),Tuple.Create(-20,450),Tuple.Create(0,450)},
                [BiomeType.Тундра] = new List<Tuple<int, int>>{Tuple.Create(-16,18),Tuple.Create(0,450),Tuple.Create(-5,18)},
                [BiomeType.Тайга] = new List<Tuple<int, int>>{Tuple.Create(0,450),Tuple.Create(4,450),Tuple.Create(4,45),Tuple.Create(-5,45)},
                [BiomeType.УмеренныйВлажныйЛес] = new List<Tuple<int, int>>{Tuple.Create(4,450),Tuple.Create(20,450),Tuple.Create(20,215),Tuple.Create(4,145)},
                [BiomeType.ТропическийЛес] = new List<Tuple<int, int>>{Tuple.Create(20,450),Tuple.Create(32,450),Tuple.Create(32,235),Tuple.Create(20,215)},
                [BiomeType.УмеренныйШироколиственныйЛес] = new List<Tuple<int, int>>{Tuple.Create(4,145),Tuple.Create(20,215),Tuple.Create(20,135),Tuple.Create(4,45)},
                [BiomeType.ТропическийМуссонныйЛес] = new List<Tuple<int, int>>{Tuple.Create(20,215),Tuple.Create(32,235),Tuple.Create(31,135),Tuple.Create(20,135)},
                [BiomeType.Лесостепь] = new List<Tuple<int, int>>{Tuple.Create(4,45),Tuple.Create(20,135),Tuple.Create(20,90)},
                [BiomeType.ЖестколиственныйЛес] = new List<Tuple<int, int>>{Tuple.Create(20,135),Tuple.Create(31,135),Tuple.Create(20,90)},
                [BiomeType.Степь] = new List<Tuple<int, int>>{Tuple.Create(-5,18),Tuple.Create(-5,45),Tuple.Create(4,45),Tuple.Create(20,90),Tuple.Create(20,36)},
                [BiomeType.Саванна] = new List<Tuple<int, int>>{Tuple.Create(20,90),Tuple.Create(31,135),Tuple.Create(30,45),Tuple.Create(20,36)},
                [BiomeType.Пустыня] = new List<Tuple<int, int>>{Tuple.Create(0,0),Tuple.Create(-5,18),Tuple.Create(20,36),Tuple.Create(30,45),Tuple.Create(31,135),Tuple.Create(32,235),Tuple.Create(32,450),Tuple.Create(35,450),Tuple.Create(35,0)},
            };

        public BiomeType GetBiome(double temperature, double moisture, bool isUnderwater)
        {
            if (isUnderwater)
                return BiomeType.Water;
            var point = (temperature).Round().MakeTupleWith((moisture).Round());

            return biomePolygons
                       .Where(kvp => point.IsPointInsidePolygon(kvp.Value))
                       .Select(kvp => kvp.Key)
                       .FirstOrDefault();
        }
    }

    public enum BiomeType
    {
        Water = -1,
        None = 0,
        АрктическаяПустыня,
        Тундра,
        Тайга,
        УмеренныйВлажныйЛес,
        ТропическийЛес,
        УмеренныйШироколиственныйЛес,
        ТропическийМуссонныйЛес,
        Лесостепь,
        ЖестколиственныйЛес,
        Степь,
        Саванна,
        Пустыня,
    }

    public static class BiomeHelper
    {
        private static readonly Color DefaultColor = Color.Black;

        public static readonly Dictionary<BiomeType, Color> BiomesToColor = new Dictionary<BiomeType, Color>
        {
            [BiomeType.Water] = Color.FromArgb(192,236,247),
            [BiomeType.None] = Color.Black,
            [BiomeType.Тундра] = Color.FromArgb(187,161,208),
            [BiomeType.Тайга] = Color.FromArgb(2,190,105),
            [BiomeType.Степь] = Color.FromArgb(254,255,175),
            [BiomeType.Лесостепь] = Color.FromArgb(186,215,138),
            [BiomeType.ТропическийМуссонныйЛес] = Color.FromArgb(151,174,218),
            [BiomeType.Саванна] =Color.FromArgb(202,215,136),
            [BiomeType.ЖестколиственныйЛес] =Color.FromArgb(205,107,0),
            [BiomeType.УмеренныйШироколиственныйЛес] = Color.FromArgb(122,213,96),
            [BiomeType.УмеренныйВлажныйЛес] = Color.FromArgb(151,218,212),
            [BiomeType.ТропическийЛес] = Color.FromArgb(0,196,186),
            [BiomeType.Пустыня] = Color.FromArgb(255,203,159),
            [BiomeType.АрктическаяПустыня] = Color.FromArgb(239,255,255),
        };

        public static Color GetColorOrDefault(this BiomeType type) =>
            BiomesToColor.ContainsKey(type) ? BiomesToColor[type] : DefaultColor;
    }
}