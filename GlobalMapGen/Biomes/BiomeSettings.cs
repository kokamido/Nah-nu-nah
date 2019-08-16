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
        private Dictionary<BiomeType, List<Tuple<int, int>>> biomeBorders =
            new Dictionary<BiomeType, List<Tuple<int, int>>>
            {
                [BiomeType.Тундра] = new List<Tuple<int, int>>
                    {(-10).MakeTupleWith(0), (0).MakeTupleWith(100), (-1).MakeTupleWith(25), (-3).MakeTupleWith(0)},
                [BiomeType.Тайга] = new List<Tuple<int, int>>
                    {(-1).MakeTupleWith(25), (0).MakeTupleWith(100), (7).MakeTupleWith(200), (6).MakeTupleWith(60),},
                [BiomeType.Степь] = new List<Tuple<int, int>>
                    {(-3).MakeTupleWith(0), (-1).MakeTupleWith(15), (21).MakeTupleWith(50), (18).MakeTupleWith(0)},
                [BiomeType.Редколесье] = new List<Tuple<int, int>>
                    {(-1).MakeTupleWith(25), (22).MakeTupleWith(125), (21).MakeTupleWith(50)},
                [BiomeType.Саванна] = new List<Tuple<int, int>>
                    {(21).MakeTupleWith(50), (22).MakeTupleWith(125), (32).MakeTupleWith(150), (33).MakeTupleWith(100)},
                [BiomeType.ШироколиственныйЛес] = new List<Tuple<int, int>>
                    {(6).MakeTupleWith(60), (7).MakeTupleWith(180), (22).MakeTupleWith(220), (22).MakeTupleWith(120)},
                [BiomeType.УмеренныйТропическийЛес] = new List<Tuple<int, int>>
                    {(6).MakeTupleWith(175), (7).MakeTupleWith(200), (23).MakeTupleWith(330), (22).MakeTupleWith(230)},
                [BiomeType.МуссонныйЛес] = new List<Tuple<int, int>>
                    {(22).MakeTupleWith(125), (22).MakeTupleWith(230), (32).MakeTupleWith(280), (32).MakeTupleWith(150)},
                [BiomeType.ТропическийЛес] = new List<Tuple<int, int>>
                    {(22).MakeTupleWith(230), (23).MakeTupleWith(330), (26).MakeTupleWith(450), 32.MakeTupleWith(280)},
                [BiomeType.Пустыня] = new List<Tuple<int, int>>
                    {(18).MakeTupleWith(0), (21).MakeTupleWith(50), (33).MakeTupleWith(100), (34).MakeTupleWith(0)},
            };

        public BiomeType GetBiome(double temperature, double moisture, bool isUnderwater)
        {
            if (isUnderwater)
                return BiomeType.Water;
            var point = temperature.Round().MakeTupleWith(moisture.Round());
            return biomeBorders
                       .Select(kvp => Tuple.Create(kvp.Key, point.IsPointInsidePolygon(kvp.Value)))
                       .FirstOrDefault(t => t.Item2)?.Item1 ?? BiomeType.None;
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
    }

    public static class BiomeHelper
    {
        private static readonly Color DefaultColor = Color.Black;

        public static readonly Dictionary<BiomeType, Color> BiomesToColor = new Dictionary<BiomeType, Color>
        {
            [BiomeType.Water] = Color.DodgerBlue,
            [BiomeType.None] = DefaultColor,
            [BiomeType.Тундра] = Color.SlateGray,
            [BiomeType.Тайга] = Color.Green,
            [BiomeType.Степь] = Color.Goldenrod,
            [BiomeType.Редколесье] = Color.YellowGreen,
            [BiomeType.МуссонныйЛес] = Color.PaleGoldenrod,
            [BiomeType.Саванна] = Color.Gold,
            [BiomeType.ШироколиственныйЛес] = Color.PaleGreen,
            [BiomeType.УмеренныйТропическийЛес] = Color.LimeGreen,
            [BiomeType.ТропическийЛес] = Color.Lime,
            [BiomeType.Пустыня] = Color.Wheat,
        };

        public static Color GetColorOrDefault(this BiomeType type) =>
            BiomesToColor.ContainsKey(type) ? BiomesToColor[type] : DefaultColor;
    }
}