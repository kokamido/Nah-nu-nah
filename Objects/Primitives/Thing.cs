using System.Collections.Generic;

namespace Objects.Primitives
{
    public class Thing<T> : Entity where T : Entity
    {
        public readonly Material Material;
        public List<T> Parts;
        public Thing(Material material) => Material = material;
    }
}