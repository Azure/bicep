// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class OutputSymbol : DeclaredSymbol
    {
        public OutputSymbol(ISymbolContext context, string name, OutputDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public OutputDeclarationSyntax DeclaringOutput => (OutputDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitOutputSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.Output;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}

