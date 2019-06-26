using System;

namespace Settings
{
    public static class SystemSettings
    {
        public static int AvaliableThreads { get; private set; }

        static SystemSettings()
        {
            AvaliableThreads = Environment.ProcessorCount;
        }
    }
}