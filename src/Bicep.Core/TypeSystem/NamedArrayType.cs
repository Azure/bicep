namespace Bicep.Core.TypeSystem
{
    public class NamedArrayType : ArrayType
    {
        public NamedArrayType(string name, TypeSymbol itemType)
            : base(name)
        {
            this.ItemType = itemType;
        }

        public override TypeSymbol ItemType { get; }
    }
}