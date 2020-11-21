// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class OutputSymbol : DeclaredSymbol
    {
        public OutputSymbol(ISymbolContext context, string name, OutputDeclarationSyntax declaringSyntax, SyntaxBase value)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Value = value;
        }

        public OutputDeclarationSyntax DeclaringOutput => (OutputDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase Value { get; }

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

