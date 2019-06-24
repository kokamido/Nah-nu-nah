using System.Collections.Generic;
using System.Drawing;

namespace GlobalMap
{
    public abstract class GenerationSettings
    {
        public readonly Color[] DebugColors = {
            Color.FromArgb(byte.MaxValue,0,0,0),
            Color.FromArgb(byte.MaxValue,128,0,0),
            Color.FromArgb(byte.MaxValue,0,128,0),
            Color.FromArgb(byte.MaxValue,0,0,128),
            Color.FromArgb(byte.MaxValue,128,128,0),
            Color.FromArgb(byte.MaxValue,128,0,128),
            Color.FromArgb(byte.MaxValue,0,128,128), 
            Color.FromArgb(byte.MaxValue,128,128,128),
            Color.FromArgb(byte.MaxValue,255,0,0),
            Color.FromArgb(byte.MaxValue,0,255,0),
            Color.FromArgb(byte.MaxValue,0,0,255),
            Color.FromArgb(byte.MaxValue,255,255,0),
            Color.FromArgb(byte.MaxValue,255,0,255),
            Color.FromArgb(byte.MaxValue,0,255,255), 
            Color.FromArgb(byte.MaxValue,255,255,255),
        };

        public abstract Dictionary<(double level, string name), Color> Layers { get;}
    }

    public class DefaultEarth : GenerationSettings
    {
        public DefaultEarth()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(0, "Глубокая вода"), Color.FromArgb(byte.MaxValue,0, 0, 128)},
                {(0.4, "Вода"), Color.FromArgb(byte.MaxValue,32, 64, 128)},
                {(0.45, "Мелкая вода"), Color.FromArgb(byte.MaxValue,64, 96, 128)},
                {(0.455, "Береговая линия"), Color.FromArgb(byte.MaxValue,192, 192, 128)},
                {(0.55, "Прибрежная зона"), Color.FromArgb(byte.MaxValue,0, 192, 0)},
                {(0.625, "Равнина"), Color.FromArgb(byte.MaxValue,192, 192, 0)},
                {(0.8, "Предгорья"), Color.FromArgb(byte.MaxValue,160, 96, 64)},
                {(0.925, "Горы"), Color.FromArgb(byte.MaxValue,128, 200, 200)},
                {(0.9875, "Вершина мира"), Color.FromArgb(byte.MaxValue,byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        public override Dictionary<(double level, string name), Color> Layers { get; }
    }
    
    public class DryLand : GenerationSettings
    {
        public DryLand()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(-1, "Вода"), Color.FromArgb(byte.MaxValue,32, 64, 128)},
                {(-0.49, "Береговая линия"), Color.FromArgb(byte.MaxValue,192, 192, 128)},
                {(-0.5, "Прибрежная зона"), Color.FromArgb(byte.MaxValue,0, 192, 0)},
                {(-0.4, "Равнина"), Color.FromArgb(byte.MaxValue,192, 192, 0)},
                {(0.6, "Предгорья"), Color.FromArgb(byte.MaxValue,160, 96, 64)},
                {(0.8, "Горы"), Color.FromArgb(byte.MaxValue,128, 200, 200)},
                {(0.99, "Вершина мира"), Color.FromArgb(byte.MaxValue,byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        public override Dictionary<(double level, string name), Color> Layers { get; }
    }
}