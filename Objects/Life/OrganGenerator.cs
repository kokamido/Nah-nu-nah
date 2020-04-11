using System;
using System.Collections.Generic;

namespace Objects.Life
{
    public class OrganGenerator
    {
        public static Organ Generate(OrganType type, CreatureSize sizeMultiplier)
        {
            throw new NotImplementedException();
        }
    }

    public enum CreatureSize
    {
        Fly,
        Frog,
        Cat,
        Human,
        Gorilla,
        Rhinoceros,
        Elephant,
        Whale,
    }
    public static class CreatureSizeHelper
    {
        private static Dictionary<CreatureSize, double> SizeToMultiplier = new Dictionary<CreatureSize, double>
        {
            [CreatureSize.Fly] = 0.0001,
            [CreatureSize.Frog] = 0.004,
            [CreatureSize.Cat] = 1,
            [CreatureSize.Human] = 15,
            [CreatureSize.Gorilla] = 60,
            [CreatureSize.Rhinoceros] = 500,
            [CreatureSize.Elephant] = 1500,
            [CreatureSize.Whale] = 25000,
        };

        public static double ToDouble(this CreatureSize size) => SizeToMultiplier[size];
    }

}