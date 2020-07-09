using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class VariableSymbol : DeclaredSymbol
    {
        public VariableSymbol(ISemanticContext context, string name, SyntaxBase declaringSyntax, SyntaxBase value, TypeSymbol type) 
            : base(context, name, declaringSyntax)
        {
            this.Value = value;
            this.Type = type;
        }

        public SyntaxBase Value { get; }

        public TypeSymbol Type { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Variable;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
