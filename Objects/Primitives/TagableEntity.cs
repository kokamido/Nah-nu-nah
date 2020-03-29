using System.Collections.Generic;
using System.Runtime.Serialization;
using MetaEntities.Tags;

namespace Objects.Primitives
{
    [DataContract]
    public class TagableEntity
    {
        [DataMember] public string Name;
        [DataMember] protected HashSet<Tag> tags;
        public bool TrySetTag(Tag t) => tags.Add(t);
        public bool ContainsTag(Tag t) => tags.Contains(t);

        public virtual IEnumerable<Tag> DefaultTags() {yield break;}

        public TagableEntity() => tags = new HashSet<Tag>(DefaultTags());
        

    }
}