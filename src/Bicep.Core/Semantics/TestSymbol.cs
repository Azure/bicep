// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Semantics
{
    public class TestSymbol : DeclaredSymbol
    {
        public TestSymbol(ISymbolContext context, string name, TestDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public TestDeclarationSyntax DeclaringTest => (TestDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitTestSymbol(this);

        public override SymbolKind Kind => SymbolKind.Test;

        public ResultWithDiagnostic<ISemanticModel> TryGetSemanticModel()
            => DeclaringTest.TryGetReferencedModel(Context.SourceFileLookup, Context.ModelLookup, b => b.ModuleDeclarationMustReferenceBicepModule());

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
