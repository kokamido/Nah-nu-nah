using System.Runtime.Serialization;

namespace Objects.Primitives
{
    /// <summary>
    /// All properties must be in [0, 1]
    /// </summary>
    [DataContract]
    public class MechanicalProperties
    {
        [DataMember] public double Sharpness;
    }
}