using System.Collections.Generic;
using MetaEntities.Tags;

namespace Objects.Primitives
{
    public class TagableEntity
    {
        protected HashSet<Tag> tags;
        public bool TrySetTag(Tag t) => tags.Add(t);
        public bool ContainsTag(Tag t) => tags.Contains(t);

        public virtual IEnumerable<Tag> DefaultTags() {yield break;}

        public TagableEntity() => tags = new HashSet<Tag>(DefaultTags());
        

    }
}