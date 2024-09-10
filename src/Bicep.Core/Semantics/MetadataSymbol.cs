// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
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

        public override IEnumerable<Diagnostic> GetDiagnostics() => ValidateName();

        private IEnumerable<Diagnostic> ValidateName()
        {
            if (this.Name.StartsWith('_'))
            {
                yield return DiagnosticBuilder.ForPosition(this.NameSource).ReservedMetadataIdentifier(this.Name);
            }
        }
    }
}

