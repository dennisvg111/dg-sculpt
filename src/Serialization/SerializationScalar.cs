namespace DG.Sculpt.Serialization
{
    public class SerializationScalar : ISerializationObject
    {
        private readonly string _value;

        public SerializationScalar(string value)
        {
            _value = value;
        }

        public virtual string ValueAsString()
        {
            return _value;
        }
    }
}
