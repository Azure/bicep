// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class MetadataSymbol : DeclaredSymbol
    {
        public MetadataSymbol(ISymbolContext context, string name, MetadataDeclarationSyntax declaringSyntax, SyntaxBase value)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Value = value;
        }

        public MetadataDeclarationSyntax DeclaringVariable => (MetadataDeclarationSyntax)this.DeclaringSyntax;

        public SyntaxBase Value { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitMetadataSymbol(this);

        public override SymbolKind Kind => SymbolKind.Metadata;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}

