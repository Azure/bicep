// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
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

        public Result<ISemanticModel, ErrorDiagnostic> TryGetSemanticModel()
            => SemanticModelHelper.TryGetTemplateModelForArtifactReference(Context.Compilation.SourceFileGrouping,
                DeclaringTest,
                b => b.ModuleDeclarationMustReferenceBicepModule(),
                Context.Compilation);

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

    }
}
