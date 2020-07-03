namespace Bicep.Core.TypeSystem
{
    public class ArrayType : TypeSymbol
    {
        public ArrayType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Array;

        public virtual TypeSymbol ItemType => LanguageConstants.Any;
    }
}