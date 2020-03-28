namespace Objects.Primitives
{
    public class Material : TagableEntity
    {
        public readonly double Density;
        public readonly double Transcalency;
        public readonly double HeatCapacity;
        public readonly double SoundAbsorption;
        public readonly double WaterAbsorption;
        public readonly double Durability; //прочность
        public readonly double Hardness; // твёрдость
        public readonly double Toughness; // ударная вязкость
        public readonly double Plasticity; // пластичность
    }
}