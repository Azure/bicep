// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ErroredImportSymbol(ISymbolContext context, string name, SyntaxBase declaringSyntax, ISymbolNameSource nameSource, ImmutableArray<ErrorDiagnostic> errors) : DeclaredSymbol(context, name, declaringSyntax, nameSource)
{
    private readonly ImmutableArray<ErrorDiagnostic> errors = errors;

    public override SymbolKind Kind => SymbolKind.Error;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => errors;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitErroredImportSymbol(this);
}
