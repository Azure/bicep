// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineDiagnosticsSyntaxTrivia(SyntaxTriviaType type, TextSpan span, string text, IEnumerable<Token> diagnosticCodes) : SyntaxTrivia(type, span, text)
    {
        public ImmutableArray<Token> DiagnosticCodes { get; } = diagnosticCodes.ToImmutableArray();
    }
}
