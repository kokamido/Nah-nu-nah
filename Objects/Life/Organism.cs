using System.Collections.Generic;
using MetaEntities.Tags;
using Objects.Primitives;

namespace Objects.Life
{
    public class Organism : TagableEntity
    {
        public List<Organ> Organs;

        public override IEnumerable<Tag> DefaultTags() => new List<Tag>
        {
            LifeTags.AliveLike
        };
    }
}