namespace Mapper
{
    public class MappingEntry
    {
        public Mapping Mapping { get; private set; }
        public object Value { get; private set; }

        public MappingEntry(Mapping mapping, object value)
        {
            Mapping = mapping;
            Value = value;
        }
    }
}
