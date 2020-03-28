using System.Collections.Generic;
using MetaEntities.Tags;

namespace Objects.Primitives
{
    public class Entity
    {
        public string Name;
        protected HashSet<Tag> tags = new HashSet<Tag>();      
        public bool TrySetTag(Tag t) => tags.Add(t);    
        public bool ContainsTag(Tag t) => tags.Contains(t);
    }
}