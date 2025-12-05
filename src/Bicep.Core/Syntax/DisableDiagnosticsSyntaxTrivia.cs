// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class DisableDiagnosticsSyntaxTrivia : SyntaxTrivia
{
    public DisableDiagnosticsSyntaxTrivia(SyntaxTriviaType type, TextSpan span, string text, IEnumerable<Token> diagnosticCodes)
        : base(type, span, text)
    {
        DiagnosticCodes = [.. diagnosticCodes];
    }

    public ImmutableArray<Token> DiagnosticCodes { get; }
}
