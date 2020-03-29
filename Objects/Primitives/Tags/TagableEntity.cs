using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Objects.Primitives.Tags
{
    [DataContract]
    public class TagableEntity
    {
        [DataMember] public string Name;
        [DataMember] protected HashSet<string> tags;
        public bool TrySetTag(string t) => tags.Add(t);
        public bool ContainsTag(string t) => tags.Contains(t);

        public virtual IEnumerable<string> DefaultTags() {yield break;}

        public TagableEntity() => tags = new HashSet<string>(DefaultTags());
        

    }
}