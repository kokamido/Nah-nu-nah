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
        private static int zaloopa = 10;
        private Dictionary<BiomeType, Tuple<int, int>> biomeCenters =
            new Dictionary<BiomeType, Tuple<int,int>>
            {
                [BiomeType.Пустыня] = Tuple.Create(25*zaloopa,30),
                [BiomeType.Саванна] = Tuple.Create(24*zaloopa,100),
                [BiomeType.МуссонныйЛес] = Tuple.Create(21*zaloopa,160),
                [BiomeType.ТропическийЛес] = Tuple.Create(24*zaloopa,220),
                [BiomeType.Степь] = Tuple.Create(9*zaloopa,20),
                [BiomeType.Редколесье] = Tuple.Create(10*zaloopa,50),
                [BiomeType.ШироколиственныйЛес] = Tuple.Create(15*zaloopa,170),
                [BiomeType.УмеренныйТропическийЛес] = Tuple.Create(17*zaloopa,200),
                [BiomeType.Тайга] = Tuple.Create(4*zaloopa,55),
                [BiomeType.Тундра] = Tuple.Create(-6*zaloopa,20),
                [BiomeType.АрктическаяПустыня] = Tuple.Create(-10*zaloopa,10),
            };

        public BiomeType GetBiome(double temperature, double moisture, bool isUnderwater)
        {
            if (isUnderwater)
                return BiomeType.Water;
            var point = (temperature*zaloopa).Round().MakeTupleWith((moisture).Round());

            return biomeCenters
                       .Select(kvp => Tuple.Create(kvp.Key, MathHelper.GetEuclideanRange(point,kvp.Value)))
                       .OrderBy(t => t.Item2)
                       .First().Item1;
        }
    }

    public enum BiomeType
    {
        Water = -1,
        None = 0,
        Тундра,
        Тайга,
        Степь,
        Редколесье,
        Саванна,
        ШироколиственныйЛес,
        УмеренныйТропическийЛес,
        МуссонныйЛес,
        ТропическийЛес,
        Пустыня,
        АрктическаяПустыня,
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
            [BiomeType.Редколесье] = Color.FromArgb(186,215,138),
            [BiomeType.МуссонныйЛес] = Color.FromArgb(151,174,218),
            [BiomeType.Саванна] =Color.FromArgb(202,215,136),
            [BiomeType.ШироколиственныйЛес] = Color.FromArgb(122,213,96),
            [BiomeType.УмеренныйТропическийЛес] = Color.FromArgb(205,107,0),
            [BiomeType.ТропическийЛес] = Color.FromArgb(0,196,186),
            [BiomeType.Пустыня] = Color.FromArgb(255,203,159),
            [BiomeType.АрктическаяПустыня] = Color.FromArgb(239,255,255),
        };

        public static Color GetColorOrDefault(this BiomeType type) =>
            BiomesToColor.ContainsKey(type) ? BiomesToColor[type] : DefaultColor;
    }
}