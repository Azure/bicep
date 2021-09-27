// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ImportedNamespaceSymbol : DeclaredSymbol, INamespaceSymbol
    {
        public ImportedNamespaceSymbol(ISymbolContext context, string name, TypeSymbol declaredType, ImportDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            DeclaredType = declaredType;
        }

        public ImportDeclarationSyntax DeclaringImport => (ImportDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitImportedNamespaceSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.ImportedNamespace;

        public override IEnumerable<Symbol> Descendants => this.Type.AsEnumerable();

        public TypeSymbol DeclaredType { get; }

        public NamespaceType? TryGetNamespaceType() => DeclaredType as NamespaceType;
    }
}