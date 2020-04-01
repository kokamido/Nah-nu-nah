using System;
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

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SensitiveToTag : Attribute
    {
        public readonly Type TagContainer;
        public readonly string Tag;

        public SensitiveToTag(Type tagContainer, string tag)
        {
            Tag = tag;
            TagContainer = tagContainer;
        }
    } 
}