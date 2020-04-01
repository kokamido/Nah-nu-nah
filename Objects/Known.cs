using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib;
using log4net;
using Objects.Life;
using Objects.Primitives.Materials;

namespace Objects
{
    public static class Known
    {
        public static Dictionary<OrganType, List<Organ>> Organs;
        public static Dictionary<string, Material> Materials;
        private const string pathToMaterials = @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownMaterials";
        private const string pathToOrgans = @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownOrgans";
        private static ILog Log = LogManager.GetLogger(typeof(Known));
        public static int i = 0;

        static Known()
        {
            LoadMaterials(pathToMaterials);
            LoadOrgans(pathToOrgans); 
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

                foreach (var obj in Enum.GetValues(typeof(OrganType)))
                {
                    var type = (OrganType) obj;
                    if (type == OrganType.None)
                        continue;
                    var toRemove = new List<Organ>();
                    
                    for (int i = 0; i < Organs[type].Count; i++)
                    {
                        var badInnerPartType = Organs[type][i].NecessaryInnerParts
                            .Select(o => o.Type)
                            .Concat(Organs[type][i].PossibleInnerParts.Select(o => o.Type))
                            .FirstOrDefault(t => t >= Organs[type][i].Type);
                        if (badInnerPartType != OrganType.None)
                        {
                            Log.Error(
                                $"Incorrect organ '{Organs[type][i].ToJsonString()}'. It may contains '{badInnerPartType.ToString()}'");
                            toRemove.Add(Organs[type][i]);
                        }
                        else
                        {
                            Log.Debug($"Generating necessary inner parts for '{Organs[type][i].ToJsonString()}'");
                            foreach (var o in Organs[type][i].NecessaryInnerParts)
                            {
                                Log.Debug($"Processing '{o}'");
                                for (int j = 0; j < MathHelper.GetUniformRndInt(o.MinCount, o.MaxCount); j++)
                                {
                                    var newOrgan = Organs[o.Type][MathHelper.GetUniformRndInt(0, Organs[o.Type].Count)];
                                    Log.Debug($"Adding '{newOrgan.ToJsonString()}'");
                                    Organs[type][i].InnerParts.Add(newOrgan);
                                }
                            }

                            Log.Debug($"Generating possible inner parts for '{Organs[type][i].ToJsonString()}'");
                            foreach (var o in Organs[type][i].PossibleInnerParts)
                            {
                                Log.Debug($"Processing '{o}'");
                                for (int j = 0; j < MathHelper.GetUniformRndInt(0, o.MaxCount); j++)
                                {
                                    if (o.Probability.CheckProbability())
                                    {
                                        var newOrgan =
                                            Organs[o.Type][MathHelper.GetUniformRndInt(0, Organs[o.Type].Count)];
                                        Log.Debug($"Adding '{newOrgan.ToJsonString()}'");
                                        Organs[type][i].InnerParts.Add(newOrgan);
                                    }
                                    else
                                        Log.Debug("Probability check failed");
                                }
                            }
                        }
                    }

                    Organs[type]
                        .RemoveAll(o =>
                            toRemove.Contains(o)); //Note Да, можно запилить хэши и сделать О(1). Но пока впадлу.
                }
            }
        }

        private static void LoadMaterials(string pathToData)
        {
            Materials = new Dictionary<string, Material>();
            Log.Debug($"Loading basic materials from '{pathToData}'");
            foreach (var file in Directory.GetFiles(pathToData))
            {
                var materials = JsonHelper.ParseJson<List<Material>>(File.ReadAllText(file));
                Log.Debug($"From '{file}' loaded '{materials.ToJsonString()}'");
                materials.ForEach(m => Materials[m.Name] = m);
            }
        }
    }
}