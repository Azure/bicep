using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class DeclarationVisitor: SyntaxVisitor
    {
        private readonly ISemanticContext context;

        private readonly List<Symbol> declaredSymbols;

        public DeclarationVisitor(ISemanticContext context, List<Symbol> declaredSymbols)
        {
            this.context = context;
            this.declaredSymbols = declaredSymbols;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);

            TypeSymbol parameterType = this.GetPrimitiveTypeByName(syntax.Type.TypeName) ?? new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType());
            
            var symbol = new ParameterSymbol(this.context, syntax.Name.IdentifierName, syntax, parameterType, syntax.Modifier);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            base.VisitVariableDeclarationSyntax(syntax);

            TypeSymbol variableType = this.context.GetTypeInfo(syntax.Value);

            var symbol = new VariableSymbol(this.context, syntax.Name.IdentifierName, syntax, syntax.Value, variableType);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            base.VisitResourceDeclarationSyntax(syntax);

            // if type string is malformed, the type value will be null which will resolve to a null type
            // below this will be corrected into an error type
            var stringSyntax = syntax.TryGetType();
            TypeSymbol? resourceType;

            if (stringSyntax != null && stringSyntax.IsInterpolated())
            {
                // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                // right now, codegen will still generate a format string however, which will cause problems for the type.
                resourceType = new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).ResourceTypeInterpolationUnsupported());
            }
            else
            {
                var stringContent = stringSyntax?.TryGetFormatString();
                resourceType = this.context.GetTypeByName(stringContent);

                // TODO: This check is likely too simplistic
                if (resourceType?.TypeKind != TypeKind.Resource)
                {
                    resourceType = new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidResourceType());
                }
            }

            var symbol = new ResourceSymbol(this.context, syntax.Name.IdentifierName, syntax, resourceType, syntax.Body);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            base.VisitOutputDeclarationSyntax(syntax);

            var outputType = this.GetPrimitiveTypeByName(syntax.Type.TypeName) ?? new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());

            var symbol = new OutputSymbol(this.context, syntax.Name.IdentifierName, syntax, outputType, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }

        private TypeSymbol? GetPrimitiveTypeByName(string typeName)
        {
            var type = this.context.GetTypeByName(typeName);
            if (type?.TypeKind == TypeKind.Primitive)
            {
                return type;
            }

            return null;
        }
    }
}
