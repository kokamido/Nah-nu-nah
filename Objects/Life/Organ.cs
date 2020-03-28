using System;
using System.Linq;
using lib;
using Objects.Primitives;

namespace Objects.Life
{
    public class Organ : Thing
    {
        public OrganState State;

        public Organ(Material material) : base(material){}
    }

    public enum OrganState
    {
        Healthy,
        Damaged,
        Broken
    }

    public static class OrganHelper
    {
        public static bool GetPiercedPart(this Organ o, out Thing piercedPart)
        {
            piercedPart = null;
            if (o.Parts?.DefaultIfEmpty() == null)
                return false;
            var rnd = MathHelper.GetUniformRndInt(0, o.Parts.Sum(p => p.Volume));
            var totalProcessedVolume = 0;
            foreach (var p in o.Parts)
            {
                totalProcessedVolume += p.Volume;
                if (totalProcessedVolume >= rnd)
                {
                    piercedPart = p;
                    return true;
                }
            }
            throw new Exception("Нас не должно тут быть");
        }
        public static void Hit(this Organ o, Thing piercing, double piercingProbability)
        {
           o.State = o.State == OrganState.Healthy 
                ? OrganState.Damaged 
                : OrganState.Broken;

            if (piercingProbability.CheckProbability() && o.GetPiercedPart(out var p))
            {
                if(p is Organ organ)
                    Hit(organ, piercing, o.ReducePiercing(piercing,piercingProbability));
            }
            
        }
    }
}