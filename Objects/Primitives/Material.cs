using System.Runtime.Serialization;

namespace Objects.Primitives
{
    /// <summary>
    /// All properties must be in [0, 1]
    /// </summary>
    [DataContract]
    public class Material : TagableEntity
    {
        [DataMember] public readonly double Density;
        [DataMember] public readonly double Transcalency;
        [DataMember] public readonly double HeatCapacity;
        [DataMember] public readonly double SoundAbsorption;
        [DataMember] public readonly double WaterAbsorption;
        [DataMember] public readonly double Durability; //прочность
        [DataMember] public readonly double Hardness; // твёрдость
        [DataMember] public readonly double Toughness; // ударная вязкость
        [DataMember] public readonly double Plasticity; // пластичность
    }
}