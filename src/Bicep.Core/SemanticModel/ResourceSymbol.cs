using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ResourceSymbol : DeclaredSymbol
    {
        public ResourceSymbol(ISemanticContext context, string name, SyntaxBase declaringSyntax, TypeSymbol type, SyntaxBase body)
            : base(context, name, declaringSyntax)
        {
            this.Type = type;
            this.Body = body;
        }

        public TypeSymbol Type { get; }

        public SyntaxBase Body { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitResourceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Resource;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            return TypeValidator.GetExpressionAssignmentDiagnostics(this.Context, this.Body, this.Type);
        }
    }
}
