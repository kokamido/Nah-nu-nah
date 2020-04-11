using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using lib;
using log4net;
using Objects.Primitives.Materials;
using Objects.Primitives.Tags;
using Objects.Primitives.Things;

namespace Objects.Life
{
    public enum OrganType
    {
        None,
        Fluid,
        Blood,
        BloodVessel,
        Muscle,
        Viscera,
        Bone,
        Sensory,
        NotVital,
        BodyPartAndLimb,
        Container
    }
    
    [DataContract]
    public class Organ : Thing
    {
        [IgnoreDataMember] public OrganState State;
        [IgnoreDataMember] public OrganType Type;
        [DataMember(Name = "KnownBasicMaterial", EmitDefaultValue = false)] private string knownBasicMaterial;
        [DataMember(Order = 1, EmitDefaultValue = false)] private string state;
        [DataMember(Name = "Type", Order = 2, EmitDefaultValue = false)] private string type;
        [DataMember(Order = 3, EmitDefaultValue = false)] public List<NecessaryInnerPartTemplate<OrganType>> NecessaryInnerParts;
        [DataMember(Order = 4, EmitDefaultValue = false)] public List<PossibleInnerPartTemplate<OrganType>> PossibleInnerParts;
        
        public Organ(Material material) : base(material){}

        public void EraseFieldsAfterGeneration()
        {
            NecessaryInnerParts = null;
            PossibleInnerParts = null;
            knownBasicMaterial = null;
        }
        
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            state = State.ToString();
            type = Type.ToString();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)            
        {
            Enum.TryParse(state, true, out State);
            Enum.TryParse(type, true, out Type);
            if (!string.IsNullOrEmpty(knownBasicMaterial))
                Material = Known.Materials[knownBasicMaterial];
        }
    }

    public static class OrganHelper
    {
        private static ILog Log = LogManager.GetLogger(typeof(OrganHelper));
             
        [SensitiveToTag(typeof(PhysicsTags), PhysicsTags.Intangible)]
        public static void Smash(this Organ o, Thing smashing, double smashingProbability)
        {
            Log.Debug($"Smashing\n\t'{o.GetType()} - {o.ToJsonString()}'\n\tWith '{smashing.GetType()} - {smashing.ToJsonString()}'\n\tProbability is '{smashingProbability}'");
            double reducedProbability = smashingProbability;
           
            if (!o.ContainsTag(PhysicsTags.Intangible))
            {
                if (!smashingProbability.CheckProbability())
                {
                    Log.Debug("Not smashed");
                    return;
                }
            
                o.State = o.State == OrganState.Healthy 
                    ? OrganState.Damaged 
                    : OrganState.Broken;
                
                reducedProbability = o.ReduceSmashing(smashing, smashingProbability);
            }
            else
                Log.Debug($"{o.ToJsonString()} is intangible!");
            

            if (o.InnerParts?.DefaultIfEmpty() == null)
            {
                Log.Debug("There is no inner part to smash");
                return;
            }
                     
            Log.Debug($"Smashing probability has been reduced from '{smashingProbability}' to '{reducedProbability}'");
            foreach (var p in o.InnerParts)
            {
                if(p is Organ organ)
                    organ.Smash(smashing, reducedProbability);
            }
        }
        
        [SensitiveToTag(typeof(PhysicsTags), PhysicsTags.Intangible)]
        public static void Pierce(this Organ o, Thing piercing, double piercingProbability)
        {
            Log.Debug($"Piercing\n\t'{o.GetType()} - {o.ToJsonString()}'\n\tWith '{piercing.GetType()} - {piercing.ToJsonString()}'\n\tProbability is '{piercingProbability}'");
            double reducedProbability = piercingProbability;
            if (!o.ContainsTag(PhysicsTags.Intangible))
            {
                if (!piercingProbability.CheckProbability())
                {
                    Log.Debug("Not pierced");
                    return;
                }

                o.State = o.State == OrganState.Healthy
                    ? OrganState.Damaged
                    : OrganState.Broken;
                reducedProbability = o.ReducePiercing(piercing, piercingProbability);
            }
            else
                Log.Debug($"{o.ToJsonString()} is intangible!");
            

            if (o.GetPiercedPart(out var p) && p is Organ organ)
                Pierce(organ, piercing,reducedProbability);
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
    
    public enum OrganState
    {
        Healthy,
        Damaged,
        Broken
    }
}