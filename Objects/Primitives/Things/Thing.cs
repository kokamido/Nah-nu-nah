using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using Objects.Life;
using Objects.Primitives.Materials;
using Objects.Primitives.Tags;

namespace Objects.Primitives.Things
{
    [DataContract]
    [KnownType(typeof(Organ))]
    public class Thing : TagableEntity
    {
        [DataMember(EmitDefaultValue = false)] public Material Material;
        [DataMember(EmitDefaultValue = false)] public double AdditionalVolumeInCm3; //cm^3
        [DataMember(EmitDefaultValue = false)] public MechanicalProperties MechanicalProperties;
        [DataMember(EmitDefaultValue = false)] public List<Thing> InnerParts;
        public CreatureSize Size { get; private set; }

        protected Thing(Material material, MechanicalProperties mechanicalProperties = null)
        {
            Material = material;
            MechanicalProperties = mechanicalProperties ?? new MechanicalProperties();
        }
        
        public void RecursiveApply<T>(Action<T> action) where T: Thing
        {
            action(this as T);
            if(InnerParts == null)
                return;
            foreach (var part in InnerParts)
                part.RecursiveApply(action);
        }
        
        public void ResizeTo(CreatureSize newSize)
        {
            var multiplier = newSize.ToDouble() / Size.ToDouble();
            AdditionalVolumeInCm3 *= multiplier;
            (InnerParts ?? Enumerable.Empty<Thing>()).ForEach(p => p.ResizeTo(newSize));
        }

        public T DeepCopyAs<T>() => JsonHelper.ParseJson<T>(this.ToJsonString());

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Size = CreatureSize.Cat;
        

        public double Volume => (InnerParts ?? Enumerable.Empty<Thing>()).Sum(part => part.Volume) + AdditionalVolumeInCm3;
        public double Mass =>  (InnerParts ?? Enumerable.Empty<Thing>()).Sum(part => part.Mass) + AdditionalVolumeInCm3 * Material.Density;
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