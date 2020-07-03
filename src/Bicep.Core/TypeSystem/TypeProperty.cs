namespace Bicep.Core.TypeSystem
{
    public class TypeProperty
    {
        public TypeProperty(string name, TypeSymbol type, bool required)
        {
            this.Name = name;
            this.Type = type;
            this.Required = required;
        }

        public string Name { get; }

        public TypeSymbol Type { get; }

        public bool Required { get; }
    }
}