using System;
using System.Runtime.Serialization;
using Objects.Life;

namespace Objects.Primitives.Things
{
    [DataContract]
    public class NecessaryInnerPartTemplate<T> where T : struct, IConvertible
    {
        [IgnoreDataMember] public T Type;
        [DataMember] public int MinCount;
        [DataMember] public int MaxCount;
        [DataMember(Name = "Type")] private string type;
        
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
           type = Type.ToString();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)            
        {
            if (!typeof(T).IsEnum) 
                throw new ArgumentException("T must be an enumerated type");
            Enum.TryParse(type, true, out Type);
        }
    }
    
    [DataContract]
    public class PossibleInnerPartTemplate<T> where T : struct, IConvertible
    {
        [IgnoreDataMember] public T Type;
        [DataMember] public double Probability;
        [DataMember] public int MaxCount;
        [DataMember(Name = "Type")] private string type;
        
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            type = Type.ToString();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)            
        {
            if (!typeof(T).IsEnum) 
                throw new ArgumentException("T must be an enumerated type");
            Enum.TryParse(type, true, out Type);
            if(Probability < 0 || Probability > 1)
                throw new ArgumentException("Probability must be in [0, 1]");
        }
    }
}