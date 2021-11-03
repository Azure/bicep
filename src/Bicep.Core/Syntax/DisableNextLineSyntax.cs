// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineSyntax : SyntaxBase
    {
        public DisableNextLineSyntax(Token keyword, IEnumerable<Token> diagnosticCodes)
        {
            AssertTokenType(keyword, nameof(keyword), TokenType.DisableNextLine);

            Keyword = keyword;
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitDisableNextLineSyntax(this);

        public Token Keyword { get; }

        public ImmutableArray<Token> DiagnosticCodes { get; }

        public override TextSpan Span => TextSpan.BetweenInclusiveAndExclusive(Keyword, DiagnosticCodes.Last());
    }
}
