using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ITypeContext context, string name, ParameterDeclarationSyntax declaringSyntax, SyntaxBase? modifier)
            : base(context, name, declaringSyntax)
        {
            this.Modifier = modifier;
        }

        public ParameterDeclarationSyntax DeclaringParameter => (ParameterDeclarationSyntax) this.DeclaringSyntax;

        public TypeSymbol Type => this.GetPrimitiveTypeByName(this.DeclaringParameter.Type.TypeName) ?? new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(this.DeclaringParameter.Type).InvalidParameterType());

        public SyntaxBase? Modifier { get; }

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

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            switch (this.Modifier)
            {
                case ParameterDefaultValueSyntax defaultValueSyntax:
                    // figure out type of the default value
                    TypeSymbol? defaultValueType = this.Context.GetTypeInfo(defaultValueSyntax.DefaultValue);

                    // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
                    if (defaultValueType is ErrorTypeSymbol)
                    {
                        return defaultValueType.GetDiagnostics();
                    }

                    if (TypeValidator.AreTypesAssignable(defaultValueType, this.Type) == false)
                    {
                        return this.CreateError(defaultValueSyntax.DefaultValue, b => b.ParameterTypeMismatch(this.Type.Name, defaultValueType.Name)).AsEnumerable();
                    }

                    break;

                case ObjectSyntax modifierSyntax when this.Type.TypeKind != TypeKind.Error:
                    return TypeValidator.GetExpressionAssignmentDiagnostics(this.Context, modifierSyntax, LanguageConstants.CreateParameterModifierType(this.Type));
            }

            return Enumerable.Empty<ErrorDiagnostic>();
        }

        public override SyntaxBase? NameSyntax => (this.DeclaringSyntax as ParameterDeclarationSyntax)?.Name;
    }
}