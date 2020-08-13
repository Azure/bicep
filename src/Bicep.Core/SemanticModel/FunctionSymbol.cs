using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.SemanticModel
{
    public class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, IEnumerable<FunctionOverload> overloads)
            : base(name)
        {
            this.Overloads = overloads.ToImmutableArray();
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitFunctionSymbol(this);

        public override SymbolKind Kind => SymbolKind.Function;

        public ImmutableArray<FunctionOverload> Overloads { get; }
    }
}