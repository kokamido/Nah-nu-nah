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
        private Dictionary<BiomeType, Tuple<int, int>> biomeCenters =
            new Dictionary<BiomeType, Tuple<int,int>>
            {
                [BiomeType.Пустыня] = Tuple.Create(25,30),
                [BiomeType.Саванна] = Tuple.Create(27,100),
                [BiomeType.МуссонныйЛес] = Tuple.Create(27,190),
                [BiomeType.ТропическийЛес] = Tuple.Create(27,350),
                [BiomeType.Степь] = Tuple.Create(9,20),
                [BiomeType.Редколесье] = Tuple.Create(10,50),
                [BiomeType.ШироколиственныйЛес] = Tuple.Create(15,150),
                [BiomeType.УмеренныйТропическийЛес] = Tuple.Create(15,220),
                [BiomeType.Тайга] = Tuple.Create(4,90),
                [BiomeType.Тундра] = Tuple.Create(-4,30),
                [BiomeType.АрктическаяПустыня] = Tuple.Create(-10,10),
            };

        public BiomeType GetBiome(double temperature, double moisture, bool isUnderwater)
        {
            if (isUnderwater)
                return BiomeType.Water;
            var point = temperature.Round().MakeTupleWith(moisture.Round());
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
            [BiomeType.Water] = Color.FromArgb(0,0,250),
            [BiomeType.None] = Color.Black,
            [BiomeType.Тундра] = Color.FromArgb(255,255,0),
            [BiomeType.Тайга] = Color.FromArgb(0,0,128),
            [BiomeType.Степь] = Color.FromArgb(0,128,0),
            [BiomeType.Редколесье] = Color.FromArgb(0,128,128),
            [BiomeType.МуссонныйЛес] = Color.FromArgb(128,128,0),
            [BiomeType.Саванна] =Color.FromArgb(128,128,128),
            [BiomeType.ШироколиственныйЛес] = Color.FromArgb(128,128,255),
            [BiomeType.УмеренныйТропическийЛес] = Color.LimeGreen,
            [BiomeType.ТропическийЛес] = Color.FromArgb(128,255,128),
            [BiomeType.Пустыня] = Color.FromArgb(128,255,255),
            [BiomeType.АрктическаяПустыня] = Color.FromArgb(255,0,0),
        };

        public static Color GetColorOrDefault(this BiomeType type) =>
            BiomesToColor.ContainsKey(type) ? BiomesToColor[type] : DefaultColor;
    }
}