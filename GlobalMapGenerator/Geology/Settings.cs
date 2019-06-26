using System.Collections.Generic;
using System.Drawing;

namespace GlobalMap
{
    public abstract class AltitudeGenerationSettings
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
        public Color RiverColor => Color.FromArgb(byte.MaxValue,10,150,255);
        public Color LakeColor => Color.FromArgb(byte.MaxValue,0,200,191);
        public abstract double SeaAltitude { get; }
        public abstract double LakeRelativeSquareThreshold { get; }
    }

    public class DefaultEarthAltitude : AltitudeGenerationSettings
    {
        public DefaultEarthAltitude()
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
                {(0.925, "Горы"), Color.FromArgb(byte.MaxValue,188, 180, 120)},
                {(0.9875, "Вершина мира"), Color.FromArgb(byte.MaxValue,byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        public override Dictionary<(double level, string name), Color> Layers { get; }
        public override double SeaAltitude => 0.455;
        public override double LakeRelativeSquareThreshold => 0.01;
    }
    
    public class DryLandAltitude : AltitudeGenerationSettings
    {
        public DryLandAltitude()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(0, "Вода"), Color.FromArgb(byte.MaxValue,32, 64, 128)},
                {(0.255, "Береговая линия"), Color.FromArgb(byte.MaxValue,192, 192, 128)},
                {(0.25, "Прибрежная зона"), Color.FromArgb(byte.MaxValue,0, 192, 0)},
                {(0.3, "Равнина"), Color.FromArgb(byte.MaxValue,192, 192, 0)},
                {(0.8, "Предгорья"), Color.FromArgb(byte.MaxValue,160, 96, 64)},
                {(0.9, "Горы"), Color.FromArgb(byte.MaxValue,128, 200, 200)},
                {(0.995, "Вершина мира"), Color.FromArgb(byte.MaxValue,byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };    
        }
        public override Dictionary<(double level, string name), Color> Layers { get; }
        public override double SeaAltitude => 0.255;
        public override double LakeRelativeSquareThreshold => 0.03;
    }
}