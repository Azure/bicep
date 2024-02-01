// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class ProviderNamespaceSymbol : DeclaredSymbol, INamespaceSymbol
    {
        public ProviderNamespaceSymbol(ISymbolContext context, ProviderDeclarationSyntax declaringSyntax, TypeSymbol declaredType)
            : base(context, declaringSyntax.Alias?.IdentifierName ?? declaringSyntax.Specification.NamespaceIdentifier, declaringSyntax, declaringSyntax.Alias as ISymbolNameSource ?? declaringSyntax.Specification)
        {
            this.DeclaredType = declaredType;
        }

        public TypeSymbol DeclaredType { get; }

        public ProviderDeclarationSyntax DeclaringProvider => (ProviderDeclarationSyntax)this.DeclaringSyntax;

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
