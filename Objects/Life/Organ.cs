using Objects.Primitives;

namespace Objects.Life
{
    public class Organ : Thing<Organ>
    {
        public Organ(Material material) : base(material){}
    }
}