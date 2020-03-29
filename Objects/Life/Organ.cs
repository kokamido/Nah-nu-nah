using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;
using Objects.Primitives;

namespace Objects.Life
{
    [DataContract]
    public class Organ : Thing
    {
        [DataMember] public OrganState State;

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
        private static ILog Log = LogManager.GetLogger(typeof(OrganHelper));
        public static void Smash(this Organ o, Thing smashing, double smashingProbability)
        {
            Log.Debug($"Smashing\n\t'{o.GetType()} - {o.ToJsonString()}'\n\tWith '{smashing.GetType()} - {smashing.ToJsonString()}'\n\tProbability is '{smashingProbability}'");
            if (!smashingProbability.CheckProbability())
            {
                Log.Debug("Not smashed");
                return;
            }
            
            o.State = o.State == OrganState.Healthy 
                ? OrganState.Damaged 
                : OrganState.Broken;

            if (o.InnerParts?.DefaultIfEmpty() == null)
            {
                Log.Debug("There is no inner part to smash");
                return;
            }
           
            var reducedProbability = o.ReduceSmashing(smashing, smashingProbability);
            Log.Debug($"Smashing probability has been reduced from '{smashingProbability}' to '{reducedProbability}'");
            foreach (var p in o.InnerParts)
            {
                if(p is Organ organ)
                    organ.Smash(smashing,reducedProbability);
            }
        }
        
        public static void Pierce(this Organ o, Thing piercing, double piercingProbability)
        {
            Log.Debug($"Piercing\n\t'{o.GetType()} - {o.ToJsonString()}'\n\tWith '{piercing.GetType()} - {piercing.ToJsonString()}'\n\tProbability is '{piercingProbability}'");
            if (!piercingProbability.CheckProbability())
            {
                Log.Debug("Not pierced");
                return;
            }

            o.State = o.State == OrganState.Healthy
                ? OrganState.Damaged
                : OrganState.Broken;

            if (o.GetPiercedPart(out var p) && p is Organ organ)
                Pierce(organ, piercing, o.ReducePiercing(piercing, piercingProbability));
        }

        private static bool GetPiercedPart(this Organ o, out Thing piercedPart)
        {
            Log.Debug($"Getting inner part of '{o.GetType()} - {o.ToJsonString()}' to pierce");
            piercedPart = null;
            if (o.InnerParts?.DefaultIfEmpty() == null)
            {
                Log.Debug($"There is no inner part to pierce");
                return false;
            }
            var rnd = MathHelper.GetUniformRndDouble(0, o.InnerParts.Sum(p => p.Volume));
            var totalProcessedVolume = 0.0;
            foreach (var p in o.InnerParts)
            {
                if (!(p is Organ))
                {
                    Log.Debug($"'{p.GetType()} - {p.ToJsonString()}' is not Organ, skipping it");
                    continue;
                }
                
                totalProcessedVolume += p.Volume;
                if (totalProcessedVolume >= rnd)
                {
                    Log.Debug($"Part to new pierce is '{p.GetType()} - {p.ToJsonString()}'");
                    piercedPart = p;
                    return true;
                }
            }
            Log.Debug("No one part has been pierced");
            return false;
        }
        
    }
}