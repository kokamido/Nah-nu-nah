using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace GlobalMap
{
    [KnownType(typeof(DefaultEarthAltitude))]
    [KnownType(typeof(DryLandAltitude))]
    [DataContract]
    public abstract class AltitudeGenerationSettings
    {
        [DataMember(EmitDefaultValue = false)] public abstract Dictionary<(double level, string name), Color> Layers { get; set;}
        public Color RiverColor => Color.FromArgb(byte.MaxValue,10,150,255);
        public Color LakeColor => Color.FromArgb(byte.MaxValue,0,200,191);
        [DataMember(EmitDefaultValue = false)] public abstract double SeaAltitude { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double LakeRelativeSquareThreshold { get; set; }
        [DataMember(EmitDefaultValue = false)] public abstract double HeightWindBlockingThreshold { get; set; }
    }

    [DataContract]
    public class DefaultEarthAltitude : AltitudeGenerationSettings
    {
        public DefaultEarthAltitude()
        {
            Layers = new Dictionary<(double level, string name), Color>
            {
                {(0, "Глубокая вода"), Color.FromArgb(byte.MaxValue,0, 0, 128)},
                {(0.35, "Вода"), Color.FromArgb(byte.MaxValue,32, 64, 128)},
                {(0.4, "Мелкая вода"), Color.FromArgb(byte.MaxValue,64, 96, 128)},
                {(0.405, "Береговая линия"), Color.FromArgb(byte.MaxValue,192, 192, 128)},
                {(0.55, "Прибрежная зона"), Color.FromArgb(byte.MaxValue,0, 192, 0)},
                {(0.625, "Равнина"), Color.FromArgb(byte.MaxValue,192, 192, 0)},
                {(0.76, "Предгорья"), Color.FromArgb(byte.MaxValue,160, 96, 64)},
                {(0.925, "Горы"), Color.FromArgb(byte.MaxValue,188, 180, 120)},
                {(0.9875, "Вершина мира"), Color.FromArgb(byte.MaxValue,byte.MaxValue, byte.MaxValue, byte.MaxValue)},
            };
            SeaAltitude = 0.455;
            LakeRelativeSquareThreshold = 0.01;
            HeightWindBlockingThreshold = 0.815;
        }
        public override Dictionary<(double level, string name), Color> Layers { get; set; }
        public override double SeaAltitude { get; set; }
        public override double LakeRelativeSquareThreshold {get; set;}
        public override double HeightWindBlockingThreshold { get; set; }
    }

    [DataContract]
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
            SeaAltitude = 0.255;
            LakeRelativeSquareThreshold = 0.03;
            HeightWindBlockingThreshold = 0.825;
        }
        public override Dictionary<(double level, string name), Color> Layers { get; set; }
        public override double SeaAltitude { get; set; }
        public override double LakeRelativeSquareThreshold {get; set;}
        public override double HeightWindBlockingThreshold { get; set; }
    }
}