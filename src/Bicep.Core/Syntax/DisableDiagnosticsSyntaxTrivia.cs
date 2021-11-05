// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class DisableDiagnosticsSyntaxTrivia : SyntaxTrivia
    {
        public DisableDiagnosticsSyntaxTrivia(Token pound,
                                              Token keyword,
                                              IEnumerable<SyntaxBase> diagnosticCodes,
                                              SyntaxTriviaType type,
                                              TextSpan span,
                                              string text) : base(type, span, text)
        {
            //AssertTokenType(pound, nameof(pound), TokenType.Pound);
            //AssertKeyword(keyword, nameof(keyword), keywordName);
            Pound = pound;
            Keyword = keyword;
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public Token Pound { get; }

        public Token Keyword { get; }

        public ImmutableArray<SyntaxBase> DiagnosticCodes { get; }
    }
}
