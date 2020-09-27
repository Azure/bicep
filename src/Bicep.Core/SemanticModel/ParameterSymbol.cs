// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ISymbolContext context, string name, ParameterDeclarationSyntax declaringSyntax, SyntaxBase? modifier)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Modifier = modifier;
        }

        public ParameterDeclarationSyntax DeclaringParameter => (ParameterDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase? Modifier { get; }

        public override SymbolKind Kind => SymbolKind.Parameter;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParameterSymbol(this);
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
