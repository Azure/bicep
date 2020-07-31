using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
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
            TypeSymbol valueType = this.Context.GetTypeInfo(this.Value);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is ErrorTypeSymbol)
            {
                return valueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(valueType, this.Type) == false)
            {
                return this.CreateError($"The output expects a value of type '{this.Type.Name} but the provided value is of type '{valueType?.Name}'.", this.Value).AsEnumerable();
            }

            return Enumerable.Empty<Error>();
        }

        public override SyntaxBase? NameSyntax => (this.DeclaringSyntax as OutputDeclarationSyntax)?.Name;
    }
}
