using System;

namespace Objects.Life
{
    public class OrganGenerator
    {
        public static Organ Generate(OrganType type, CreatureSize size)
        {
            throw new NotImplementedException();
        }
    }

    public enum CreatureSize
    {
        Fly,
        Frog,
        Cat,
        Wolf,
        Human,
        Gorilla,
        Rhinoceros,
        Elephant,
        Whale
    }

    
}