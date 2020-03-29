using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaEntities.Tags;
using Objects.Primitives;

namespace Objects.Life
{
    [DataContract]
    public class Organism : TagableEntity
    {
        [DataMember] public List<Organ> Organs;

        public override IEnumerable<Tag> DefaultTags() => new List<Tag>
        {
            LifeTags.AliveLike
        };
    }
}