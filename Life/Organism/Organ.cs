using System.Collections.Generic;

namespace Life
{
    public class Organ
    {
        public int Health { get; set; }
        public List<OrganTag> Tags { get; set; }
        public bool IsCriticalToSurvive { get; set; }
    }
}