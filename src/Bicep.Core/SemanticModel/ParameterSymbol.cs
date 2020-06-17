namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : Symbol
    {
        public ParameterSymbol(string name) : base(name)
        {
        }

        public override SymbolKind Kind { get; }
    }
}