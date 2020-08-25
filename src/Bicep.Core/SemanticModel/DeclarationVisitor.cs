// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class DeclarationVisitor: SyntaxVisitor
    {
        private readonly ITypeManager typeManager;

        private readonly IList<DeclaredSymbol> declaredSymbols;

        public DeclarationVisitor(ITypeManager typeManager, IList<DeclaredSymbol> declaredSymbols)
        {
            this.typeManager = typeManager;
            this.declaredSymbols = declaredSymbols;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);

            var symbol = new ParameterSymbol(this.typeManager, syntax.Name.IdentifierName, syntax, syntax.Modifier);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            base.VisitVariableDeclarationSyntax(syntax);

            var symbol = new VariableSymbol(this.typeManager, syntax.Name.IdentifierName, syntax, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            base.VisitResourceDeclarationSyntax(syntax);

            var symbol = new ResourceSymbol(this.typeManager, syntax.Name.IdentifierName, syntax, syntax.Body);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            base.VisitOutputDeclarationSyntax(syntax);

            var symbol = new OutputSymbol(this.typeManager, syntax.Name.IdentifierName, syntax, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }
    }
}

