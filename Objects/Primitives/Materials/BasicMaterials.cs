using System.Collections.Generic;
using System.IO;
using lib;
using log4net;

namespace Objects.Primitives.Materials
{
    public class BasicMaterials
    {
        public Dictionary<string, Material> Materials;
        private static ILog Log = LogManager.GetLogger(typeof(BasicMaterials));

        public BasicMaterials(string pathToData)
        {
            Log.Debug($"Loading basic materials from '{pathToData}'");
            foreach (var file in Directory.GetFiles(pathToData))
            {
                var material = JsonHelper.ParseJson<Material>(File.ReadAllText(file));
                Log.Debug($"From '{file}' loaded '{material.ToJsonString()}'");
                Materials[material.Name] = material;
            }
        }
    }
}