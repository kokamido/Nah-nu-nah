using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Objects.Life;

namespace Objects.Primitives
{
    [DataContract]
    [KnownType(typeof(Organ))]
    public abstract class Thing : TagableEntity
    {
        [DataMember] public readonly Material Material;
        [DataMember] public int Volume;
        [DataMember] public MechanicalProperties MechanicalProperties;
        [DataMember] public List<Thing> Parts;
        public Thing(Material material) => Material = material;
        public double Mass => Volume * Material.Density;
    }

    public static class ThingHelper
    {
        public static double ReducePiercing(this Thing pierced, Thing piercing, double oldProbability)
        {
            if (pierced.Material.Hardness > piercing.Material.Hardness * 1.5)
                return 0;
            return Math.Min(0.95,
                       piercing.Material.Hardness / pierced.Material.Hardness *
                       (1 + piercing.MechanicalProperties.Sharpness)) * oldProbability;
        }

        public static double ReduceSmashing(this Thing smashed, Thing smashing, double oldProbability)
        {
            if (smashed.Volume / smashing.Volume > 100)
                return 0;
            return Math.Min(0.6, 1 - (smashed.Material.Toughness - smashing.Material.Hardness)) * oldProbability;
        }
    }
}