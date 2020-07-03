using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ISemanticContext context, string name, ParameterDeclarationSyntax declaringSyntax, TypeSymbol type, SyntaxBase? defaultValue)
            : base(context, name, declaringSyntax)
        {
            this.Type = type;
            this.DefaultValue = defaultValue;
        }

        public TypeSymbol Type { get; }

        public SyntaxBase? DefaultValue { get; }

        public override SymbolKind Kind => SymbolKind.Parameter;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParameterSymbol(this);
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            if(this.DefaultValue != null)
            {
                // figure out type of the default value
                TypeSymbol? defaultValueType = this.Context.GetTypeInfo(this.DefaultValue);

                if (TypeValidator.AreTypesAssignable(defaultValueType, this.Type) == false)
                {
                    yield return this.CreateError($"The parameter expects a default value of type '{this.Type.Name}' but provided value is of type '{defaultValueType?.Name}'.", this.DefaultValue);
                }
            }
        }
    }
}