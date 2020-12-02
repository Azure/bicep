// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ResourceSymbol : DeclaredSymbol
    {
        public ResourceSymbol(ISymbolContext context, string name, ResourceDeclarationSyntax declaringSyntax, SyntaxBase body)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Body = body;
        }

        public ResourceDeclarationSyntax DeclaringResource => (ResourceDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase Body { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitResourceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Resource;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}