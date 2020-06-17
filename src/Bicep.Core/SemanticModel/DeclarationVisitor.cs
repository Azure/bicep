using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class DeclarationVisitor: SyntaxVisitor
    {
        private readonly FileSymbol containingFile;

        private readonly List<Symbol> declaredSymbols;

        public DeclarationVisitor(FileSymbol containingFile, List<Symbol> declaredSymbols)
        {
            this.containingFile = containingFile;
            this.declaredSymbols = declaredSymbols;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);

            TypeSymbol parameterType = this.containingFile.ContainingModel.GetTypeByName(syntax.Type.TypeName) ?? new ErrorTypeSymbol(new Error($"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PropertyTypesString}", syntax.Type.Span));

            var symbol = new ParameterSymbol(this.containingFile, syntax.Name.IdentifierName, syntax, parameterType, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }
    }
}
