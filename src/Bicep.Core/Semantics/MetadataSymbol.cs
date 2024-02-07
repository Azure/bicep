// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class MetadataSymbol(ISymbolContext context, string name, MetadataDeclarationSyntax declaringSyntax, SyntaxBase value) : DeclaredSymbol(context, name, declaringSyntax, declaringSyntax.Name)
    {
        public SyntaxBase Value { get; } = value;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitMetadataSymbol(this);

        public override SymbolKind Kind => SymbolKind.Metadata;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => ValidateName();

        private IEnumerable<ErrorDiagnostic> ValidateName()
        {
            if (this.Name.StartsWith("_"))
            {
                yield return DiagnosticBuilder.ForPosition(this.NameSource).ReservedMetadataIdentifier(this.Name);
            }
        }
    }
}

