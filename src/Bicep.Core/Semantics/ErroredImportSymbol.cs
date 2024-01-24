// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ErroredImportSymbol : DeclaredSymbol
{
    private readonly ImmutableArray<ErrorDiagnostic> errors;

    public ErroredImportSymbol(ISymbolContext context, string name, SyntaxBase declaringSyntax, ISymbolNameSource nameSource, ImmutableArray<ErrorDiagnostic> errors)
        : base(context, name, declaringSyntax, nameSource)
    {
        this.errors = errors;
    }

    public override SymbolKind Kind => SymbolKind.Error;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => errors;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitErroredImportSymbol(this);
}
