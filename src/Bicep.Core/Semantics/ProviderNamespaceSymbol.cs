// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ProviderNamespaceSymbol : DeclaredSymbol, INamespaceSymbol
    {
        private readonly ITypeReference declaredType;
        public ProviderNamespaceSymbol(ISymbolContext context, ProviderDeclarationSyntax declaringSyntax, ITypeReference declaredType)
            : base(context, declaringSyntax.Alias?.IdentifierName ?? declaringSyntax.Specification.Name, declaringSyntax, declaringSyntax.Alias as ISymbolNameSource ?? declaringSyntax.Specification)
        {
            this.declaredType = declaredType;
        }

        public TypeSymbol DeclaredType => declaredType.Type;

        public ProviderDeclarationSyntax DeclaringImport => (ProviderDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitProviderNamespaceSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.ImportedNamespace;

        public override IEnumerable<Symbol> Descendants => this.Type.AsEnumerable();

        public ResourceScope TargetScope { get; }

        public NamespaceType? TryGetNamespaceType() => this.DeclaredType as NamespaceType;
    }
}
