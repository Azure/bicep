// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class DisableDiagnosticsSyntax : SyntaxBase
    {
        public DisableDiagnosticsSyntax(Token keyword, IEnumerable<SyntaxBase> diagnosticCodes, TokenType tokenType)
        {
            AssertTokenType(keyword, nameof(keyword), tokenType);

            Keyword = keyword;
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public Token Keyword { get; }

        public ImmutableArray<SyntaxBase> DiagnosticCodes { get; }

        public override TextSpan Span => TextSpan.Between(Keyword, DiagnosticCodes.Last());
    }
}
