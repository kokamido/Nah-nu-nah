using System;
using System.Collections.Generic;

namespace Objects.Primitives
{
    public abstract class Thing : TagableEntity
    {
        public readonly Material Material;
        public int Volume;
        public MechanicalProperties MechanicalProperties;
        public List<Thing> Parts;
        public Thing(Material material) => Material = material;
    }

    public static class ThingHelper
    {
        public static double ReducePiercing(this Thing pierced, Thing piercing, double oldPiercing)
        {
            if (pierced.Material.Hardness > piercing.Material.Hardness * 1.5)
                return 0;
            return Math.Min(0.95,
                       piercing.Material.Hardness / pierced.Material.Hardness *
                       (1 + piercing.MechanicalProperties.Sharpness)) * oldPiercing;
        }
    }
}