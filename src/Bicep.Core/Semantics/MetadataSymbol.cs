// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
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

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => RestrictedIdentifierValidatorVisitor.GetDiagnostics(this);

        private sealed class RestrictedIdentifierValidatorVisitor : SymbolVisitor
        {
            private IList<ErrorDiagnostic> Diagnostics { get; } = new List<ErrorDiagnostic>();

            public static IEnumerable<ErrorDiagnostic> GetDiagnostics(MetadataSymbol metadata)
            {
                var visitor = new RestrictedIdentifierValidatorVisitor();
                visitor.Visit(metadata);
                if (metadata.Name.StartsWith("_"))
                {
                    visitor.Diagnostics.Add(DiagnosticBuilder.ForPosition(metadata.NameSyntax).ReservedIdentifier(metadata.Name));
                }
                return visitor.Diagnostics;
            }
        }
    }
}

