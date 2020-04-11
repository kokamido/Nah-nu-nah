using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib;
using log4net;
using Objects.Life;
using Objects.Primitives.Materials;
using Objects.Primitives.Things;

namespace Objects
{
    public static class Known
    {
        private static Dictionary<OrganType, List<Organ>> Organs;
        public static Dictionary<string, Material> Materials;
        private const string pathToMaterials = @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownMaterials";
        private const string pathToOrgans = @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownOrgans";
        private static ILog Log = LogManager.GetLogger(typeof(Known));

        static Known()
        {
            LoadMaterials(pathToMaterials);
            LoadOrgans(pathToOrgans);
        }

        public static Organ Organ(OrganType type)
        {
            Log.Debug($"Generating organ of type '{type.ToString()}'");
            var organ = JsonHelper.ParseJson<Organ>(Organs[type][MathHelper.GetUniformRndInt(0, Organs[type].Count - 1)]
                .ToJsonString()); //Note Будет лагать - напишу норм копирование. ИМХО для генерации на старте и так сойдёт
            Log.Debug($"Generating necessary inner parts for '{organ.ToJsonString()}'");
            foreach (var o in organ.NecessaryInnerParts ?? Enumerable.Empty<NecessaryInnerPartTemplate<OrganType>>())
            {
                Log.Debug($"Processing '{o}'");
                for (int j = 0; j < MathHelper.GetUniformRndInt(o.MinCount, o.MaxCount); j++)
                    AddInnerOrgan(organ, o.Type, o.KnownName);
            }

            Log.Debug($"Generating possible inner parts for '{organ.ToJsonString()}'");
            foreach (var o in organ.PossibleInnerParts ?? Enumerable.Empty<PossibleInnerPartTemplate<OrganType>>())
            {
                Log.Debug($"Processing '{o}'");
                for (int j = 0; j < MathHelper.GetUniformRndInt(0, o.MaxCount); j++)
                {
                    if (o.Probability.CheckProbability())
                        AddInnerOrgan(organ, o.Type, o.KnownName);
                    else
                        Log.Debug("Probability check failed");
                }
            }

            organ.EraseFieldsAfterGeneration();
            Log.Debug($"{organ.Name} has been generated with volume '{organ.Volume}' for {CreatureSize.Cat}:\n{organ.ToJsonString()}\n");
            return organ;
        }

        private static void AddInnerOrgan(Organ baseOrgan, OrganType innerOrganType, string innerOrganKnownName)
        {
            Organ newOrgan = null;
            if (string.IsNullOrWhiteSpace(innerOrganKnownName))
                newOrgan = Organ(innerOrganType);
            else
            {
                newOrgan = Organs[innerOrganType].FirstOrDefault(org => string.Equals(org.Name, innerOrganKnownName, StringComparison.InvariantCultureIgnoreCase));
                if (newOrgan == null)
                {
                    Log.Error($"Can't generate organ with type'{innerOrganType}'. Can't resolve KnownName '{innerOrganKnownName}'. Skipping it.");
                    return;
                }
                newOrgan = JsonHelper.ParseJson<Organ>(newOrgan.ToJsonString()); //Note Будет лагать - напишу норм копирование. ИМХО для генерации на старте и так сойдёт
            }
            Log.Debug($"Adding '{newOrgan.ToJsonString()}'");
            if (baseOrgan.InnerParts == null)
                baseOrgan.InnerParts = new List<Thing>();
            baseOrgan.InnerParts.Add(newOrgan);
        }
        
        private static void LoadOrgans(string pathToData)
        {
            Organs = new Dictionary<OrganType, List<Organ>>();
            Log.Debug($"Loading basic organs from '{pathToData}'");
            foreach (var file in Directory.GetFiles(pathToData))
            {
                var organs = JsonHelper.ParseJson<List<Organ>>(File.ReadAllText(file));
                Log.Debug($"From '{file}' loaded '{organs.ToJsonString()}'");
                foreach (var organ in organs)
                {
                    if (Organs.TryGetValue(organ.Type, out var list))
                        list.Add(organ);
                    else
                        Organs[organ.Type] = new List<Organ> {organ};
                }
            }

            foreach (var obj in Enum.GetValues(typeof(OrganType)))
            {
                var type = (OrganType) obj;
                if (type == OrganType.None)
                    continue;
                var toRemove = new List<Organ>();

                if (!Organs.ContainsKey(type))
                    continue;
                for (int i = 0; i < Organs[type].Count; i++)
                {
                    var typesToProcess = Enumerable.Empty<OrganType>();

                    if (Organs[type][i].NecessaryInnerParts != null)
                        typesToProcess = typesToProcess.Concat(Organs[type][i].NecessaryInnerParts.Select(o => o.Type));

                    if (Organs[type][i].PossibleInnerParts != null)
                        typesToProcess = typesToProcess.Concat(Organs[type][i].PossibleInnerParts.Select(o => o.Type));

                    var badInnerPartType = typesToProcess.FirstOrDefault(t => t >= Organs[type][i].Type);

                    if (badInnerPartType != OrganType.None)
                    {
                        Log.Error($"Incorrect organ '{Organs[type][i].ToJsonString()}'. It may contains '{badInnerPartType.ToString()}'. It will be removed");
                        toRemove.Add(Organs[type][i]);
                    }
                }

                Organs[type].RemoveAll(o => toRemove.Contains(o)); //Note Да, можно запилить хэши и сделать О(1). Но пока впадлу.
            }
        }

        private static void LoadMaterials(string pathToData)
        {
            Materials = new Dictionary<string, Material>();
            foreach (var file in Directory.GetFiles(pathToData))
            {
                Log.Debug($"Loading basic materials from '{file}'");
                var materials = JsonHelper.ParseJson<List<Material>>(File.ReadAllText(file));
                Log.Debug($"From '{file}' loaded '{materials.ToJsonString()}'");
                materials.ForEach(m => Materials[m.Name] = m);
            }
        }
    }
}