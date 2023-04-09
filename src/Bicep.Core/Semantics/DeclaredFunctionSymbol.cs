// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class DeclaredFunctionSymbol : DeclaredSymbol
{
    public DeclaredFunctionSymbol(ISymbolContext context, string name, FunctionDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
    }

    public FunctionDeclarationSyntax DeclaringFunction => (FunctionDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitDeclaredFunctionSymbol(this);

    public override SymbolKind Kind => SymbolKind.Variable;

    public override IEnumerable<Symbol> Descendants => Type.AsEnumerable();
}