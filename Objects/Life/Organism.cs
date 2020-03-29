using System.Collections.Generic;
using System.Runtime.Serialization;
using Objects.Primitives.Tags;

namespace Objects.Life
{
    [DataContract]
    public class Organism : TagableEntity
    {
        [DataMember] public List<Organ> Organs;

        public override IEnumerable<string> DefaultTags() => new List<string>
        {
            LiveTags.AliveLike
        };
    }
}