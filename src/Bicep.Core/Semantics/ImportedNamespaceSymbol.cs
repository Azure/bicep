// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class ImportedNamespaceSymbol : DeclaredSymbol
    {
        public ImportedNamespaceSymbol(ISymbolContext context, string name, ImportDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ImportDeclarationSyntax DeclaringImport => (ImportDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitImportedNamespaceSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.ImportedNamespace;

        public override IEnumerable<Symbol> Descendants => this.Type.AsEnumerable();
    }
}