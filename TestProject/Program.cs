using System.Collections.Generic;
using System.IO;
using lib;
using Objects.Life;
using Objects.Primitives;

namespace TestProject
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var rhinoSkin = new Material
            {
                Name = "RhinoSkin",
                Hardness = 0.2,
                Toughness = 0.2,
                Density = 1200
            };
            var skin = new Organ(rhinoSkin)
            {
                Name = "RhinoSkin",
                Volume = 0.5
            };
            var rhino = new Organism
            {
                Name = "Rhino",
                Organs = new List<Organ>{skin}
            };
            
            File.WriteAllText("Organism\\Rhino",rhino.ToJsonString());
           
        }
    }
}