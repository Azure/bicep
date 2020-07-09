using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class OutputSymbol : DeclaredSymbol
    {
        public OutputSymbol(ISemanticContext context, string name, SyntaxBase declaringSyntax, TypeSymbol type, SyntaxBase value)
            : base(context, name, declaringSyntax)
        {
            this.Type = type;
            this.Value = value;
        }

        public TypeSymbol Type { get; }
        
        public SyntaxBase Value { get; }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitOutputSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.Output;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            TypeSymbol? valueType = this.Context.GetTypeInfo(this.Value);

            if (TypeValidator.AreTypesAssignable(valueType, this.Type) == false)
            {
                yield return this.CreateError($"The output expects a value of type '{this.Type.Name} but the provided value is of type '{valueType?.Name}'.", this.Value);
            }
        }
    }
}
