using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Bicep.Core.Errors;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ISemanticContext context, string name, ParameterDeclarationSyntax declaringSyntax, TypeSymbol type, SyntaxBase? modifier)
            : base(context, name, declaringSyntax)
        {
            this.Type = type;
            this.Modifier = modifier;
        }

        public TypeSymbol Type { get; }

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

        public override IEnumerable<Error> GetDiagnostics()
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
                        return this.CreateError(defaultValueSyntax.DefaultValue, ErrorCode.ErrParameterTypeMismatch, this.Type.Name, defaultValueType.Name).AsEnumerable();
                    }

                    break;

                case ObjectSyntax modifierSyntax when this.Type.TypeKind != TypeKind.Error:
                    return TypeValidator.GetExpressionAssignmentDiagnostics(this.Context, modifierSyntax, LanguageConstants.CreateParameterModifierType(this.Type));
            }

            return Enumerable.Empty<Error>();
        }

        public override SyntaxBase? NameSyntax => (this.DeclaringSyntax as ParameterDeclarationSyntax)?.Name;
    }
}