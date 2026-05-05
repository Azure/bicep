// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    /// <summary>
    /// Represents a local 'this' namespace symbol that provides context-specific namespace functionality
    /// within a local scope (e.g., within a resource body).
    /// </summary>
    public class LocalThisNamespaceSymbol : DeclaredSymbol, INamespaceSymbol
    {
        private class LocalThisNameSource : ISymbolNameSource
        {
            private readonly ResourceDeclarationSyntax resource;

            public LocalThisNameSource(ResourceDeclarationSyntax resource)
            {
                this.resource = resource;
            }

            public bool IsValid => true;

            public TextSpan Span => resource.Keyword.Span;
        }

        public LocalThisNamespaceSymbol(ISymbolContext context, string name, ResourceDeclarationSyntax declaringResource, SyntaxBase declaringSyntax, TypeSymbol declaredType)
            : base(
                context,
                name,
                declaringSyntax,
                new LocalThisNameSource(declaringResource))
        {
            this.DeclaredType = declaredType;
            this.DeclaringResource = declaringResource;
        }

        public TypeSymbol DeclaredType { get; }

        public ResourceDeclarationSyntax DeclaringResource { get; }

        // Override the Type property to return DeclaredType instead of using TypeManager
        public new TypeSymbol Type => DeclaredType;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitLocalThisNamespaceSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.Local;

        public override IEnumerable<Symbol> Descendants => this.Type.AsEnumerable();

        public NamespaceType? TryGetNamespaceType() => this.DeclaredType as NamespaceType;
    }
}
