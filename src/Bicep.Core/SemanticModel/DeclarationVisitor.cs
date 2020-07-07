using System.Collections.Generic;
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

            TypeSymbol parameterType = this.context.GetTypeByName(syntax.Type.TypeName) ?? new ErrorTypeSymbol(new Error($"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}", syntax.Type.Span));

            var symbol = new ParameterSymbol(this.context, syntax.Name.IdentifierName, syntax, parameterType, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }
    }
}
