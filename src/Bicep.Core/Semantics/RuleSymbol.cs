// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class RuleSymbol : DeclaredSymbol
{
    public RuleSymbol(ISymbolContext context, string name, RuleDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
    }

    public RuleDeclarationSyntax DeclaringRule => (RuleDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitRuleSymbol(this);

    public override SymbolKind Kind => SymbolKind.Rule;

    public override IEnumerable<Symbol> Descendants
    {
        get
        {
            yield return this.Type;
        }
    }
}
