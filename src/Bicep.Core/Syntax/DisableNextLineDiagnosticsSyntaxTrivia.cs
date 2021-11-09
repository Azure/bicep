// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineDiagnosticsSyntaxTrivia : SyntaxTrivia
    {
        public DisableNextLineDiagnosticsSyntaxTrivia(SyntaxTriviaType type,
                                                      TextSpan span,
                                                      string text,
                                                      TextNode keyword,
                                                      IEnumerable<TextNode> diagnosticCodes)
            :base(type, span, text)
        {
            Keyword = keyword;
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public TextNode Keyword { get; }

        public ImmutableArray<TextNode> DiagnosticCodes { get; }
    }
}
