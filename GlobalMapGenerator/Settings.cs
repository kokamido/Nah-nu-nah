using System;
using System.Collections.Generic;
using SharpNoise.Utilities.Imaging;

namespace GlobalMap
{
    public abstract class GenerationSettings
    {
        private readonly List<Color> _debugColors = new List<Color>
        {
            new Color(0,0,0,Byte.MaxValue),
            new Color(128,0,0,Byte.MaxValue),
            new Color(0,128,0,Byte.MaxValue),
            new Color(0,0,128,Byte.MaxValue),
            new Color(128,128,0,Byte.MaxValue),
            new Color(128,0,128,Byte.MaxValue),
            new Color(0,128,128,Byte.MaxValue), 
            new Color(128,128,128,Byte.MaxValue),
            new Color(255,0,0,Byte.MaxValue),
            new Color(0,255,0,Byte.MaxValue),
            new Color(0,0,255,Byte.MaxValue),
            new Color(255,255,0,Byte.MaxValue),
            new Color(255,0,255,Byte.MaxValue),
            new Color(0,255,255,Byte.MaxValue), 
            new Color(255,255,255,Byte.MaxValue),
        };

        protected abstract Dictionary<(double level, string name), Color> Layers { get;}

        public void Apply(ImageRenderer renderer)
        {
            foreach (var l in Layers)
            {
                renderer.AddGradientPoint(l.Key.level, l.Value);
            }
        }
        
        public void ApplyDebug(ImageRenderer renderer)
        {
            int i = 0;
            foreach (var l in Layers)
            {
                renderer.AddGradientPoint(l.Key.level, _debugColors[i++]);
            }
        }
    }

    public class DefaultEarth : GenerationSettings
    {
        public DefaultEarth()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(-1, "Глубокая вода"), new Color(0, 0, 128, byte.MaxValue)},
                {(-0.2, "Вода"), new Color(32, 64, 128, byte.MaxValue)},
                {(-0.1, "Мелкая вода"), new Color(64, 96, 128, byte.MaxValue)},
                {(-0.09, "Береговая линия"), new Color(192, 192, 128, byte.MaxValue)},
                {(0.1, "Прибрежная зона"), new Color(0, 192, 0, byte.MaxValue)},
                {(0.25, "Равнина"), new Color(192, 192, 0, byte.MaxValue)},
                {(0.6, "Предгорья"), new Color(160, 96, 64, byte.MaxValue)},
                {(0.85, "Горы"), new Color(128, 200, 200, byte.MaxValue)},
                {(0.975, "Вершина мира"), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        protected override Dictionary<(double level, string name), Color> Layers { get; }
    }
    
    public class DryLand : GenerationSettings
    {
        public DryLand()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(-1, "Вода"), new Color(32, 64, 128, byte.MaxValue)},
                {(-0.49, "Береговая линия"), new Color(192, 192, 128, byte.MaxValue)},
                {(-0.5, "Прибрежная зона"), new Color(0, 192, 0, byte.MaxValue)},
                {(-0.4, "Равнина"), new Color(192, 192, 0, byte.MaxValue)},
                {(0.6, "Предгорья"), new Color(160, 96, 64, byte.MaxValue)},
                {(0.8, "Горы"), new Color(128, 200, 200, byte.MaxValue)},
                {(0.99, "Вершина мира"), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        protected override Dictionary<(double level, string name), Color> Layers { get; }
    }
}