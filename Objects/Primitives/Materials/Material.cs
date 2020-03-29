using System.Runtime.Serialization;
using Objects.Primitives.Tags;

namespace Objects.Primitives.Materials
{
    [DataContract]
    public class Material : TagableEntity
    {
        [DataMember] public double Density; //10000 kg/m^3
        [DataMember] public double HeatCapacity;
        [DataMember] public double WaterAbsorption;
        //всё далььнейшее маппим в [0, 1]
        [DataMember] public double SoundAbsorption;
        [DataMember] public double Transcalency; //Теплопроводность
        [DataMember] public double Durability; //прочность
        [DataMember] public double Hardness; // твёрдость Моос/10 https://ru.wikipedia.org/wiki/%D0%A8%D0%BA%D0%B0%D0%BB%D0%B0_%D0%9C%D0%BE%D0%BE%D1%81%D0%B0
        [DataMember] public double Toughness; // ударная вязкость 0-100 / 100
        [DataMember] public double Plasticity; // пластичность
    }
}