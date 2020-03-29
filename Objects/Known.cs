using System.Collections.Generic;
using System.IO;
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
        private const string pathToMaterials= @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownMaterials";
        private const string pathToOrgans = @"C:\Users\Ква\Desktop\Nah-nu-nah\Objects\KnownOrgans";
        private static ILog Log = LogManager.GetLogger(typeof(Known));
        public static int i = 0;

        static Known()
        {
            LoadOrgans(pathToOrgans);
            LoadMaterials(pathToMaterials);
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