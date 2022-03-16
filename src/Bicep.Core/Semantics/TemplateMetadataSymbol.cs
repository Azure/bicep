// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class TemplateMetadataSymbol : DeclaredSymbol
    {
        public TemplateMetadataSymbol(ISymbolContext context, string name, TemplateMetadataSyntax declaringSyntax, SyntaxBase value)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Value = value;
        }

        public TemplateMetadataSyntax DeclaringVariable => (TemplateMetadataSyntax)this.DeclaringSyntax;

        public SyntaxBase Value { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitTemplateMetadataSymbol(this);

        public override SymbolKind Kind => SymbolKind.TemplateMetadata;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}

