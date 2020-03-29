using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Objects.Life;
using Objects.Primitives.Materials;
using Objects.Primitives.Tags;

namespace Objects.Primitives.Things
{
    [DataContract]
    [KnownType(typeof(Organ))]
    public abstract class Thing : TagableEntity
    {
        [DataMember] public Material Material;
        [DataMember] public double Volume; //m^3
        [DataMember] public MechanicalProperties MechanicalProperties;
        [DataMember] public List<Thing> InnerParts;

        public Thing(Material material, MechanicalProperties mechanicalProperties = null)
        {
            Material = material;
            MechanicalProperties = mechanicalProperties ?? new MechanicalProperties();
        }
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
                       (0.8 + piercing.MechanicalProperties.Sharpness)) * oldProbability;
        }

        public static double ReduceSmashing(this Thing smashed, Thing smashing, double oldProbability)
        {
            if (smashed.Mass / smashing.Mass > 1000)
                return 0;
            return Math.Min(0.6, 1 - (smashed.Material.Toughness - smashing.Material.Hardness)) * oldProbability;
        }
    }
}