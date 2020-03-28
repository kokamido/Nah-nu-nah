namespace MetaEntities.Tags
{
    public class Tag  
    {
        public readonly string Value;
        public Tag(string value) => Value = value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tag) obj);
        }

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;      
        protected bool Equals(Tag other) => string.Equals(Value, other.Value);
        
    } 
}