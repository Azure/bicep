// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class ExtensionNamespaceSymbol : DeclaredSymbol, INamespaceSymbol
    {
        private class ExtensionNameSource : ISymbolNameSource
        {
            private readonly ExtensionDeclarationSyntax extension;

            public ExtensionNameSource(ExtensionDeclarationSyntax extension)
            {
                this.extension = extension;
            }

            public bool IsValid => true;

            public TextSpan Span => extension.Alias?.Span ?? extension.SpecificationString.Span;
        }

        public ExtensionNamespaceSymbol(ISymbolContext context, ExtensionDeclarationSyntax declaringSyntax, TypeSymbol declaredType)
            : base(
                context,
                declaringSyntax.TryGetSymbolName() ?? declaredType.Name,
                declaringSyntax,
                new ExtensionNameSource(declaringSyntax))
        {
            this.DeclaredType = declaredType;
        }

        public TypeSymbol DeclaredType { get; }

        public ExtensionDeclarationSyntax DeclaringExtension => (ExtensionDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitExtensionNamespaceSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.ImportedNamespace;

        public override IEnumerable<Symbol> Descendants => this.Type.AsEnumerable();

        public ResourceScope TargetScope { get; }

        public NamespaceType? TryGetNamespaceType() => this.DeclaredType as NamespaceType;
    }
}
