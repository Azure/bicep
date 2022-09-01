// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineDiagnosticsSyntaxTrivia : SyntaxTrivia
    {
        public DisableNextLineDiagnosticsSyntaxTrivia(SyntaxTriviaType type, TextSpan span, string text, IEnumerable<Token> diagnosticCodes)
            : base(type, span, text)
        {
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public ImmutableArray<Token> DiagnosticCodes { get; }
    }
}
