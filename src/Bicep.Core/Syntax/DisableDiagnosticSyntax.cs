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
        public DisableDiagnosticsSyntax(Token pound, Token keyword, IEnumerable<SyntaxBase> diagnosticCodes, string keywordName)
        {
            AssertTokenType(pound, nameof(pound), TokenType.Pound);
            AssertKeyword(keyword, nameof(keyword), keywordName);
            Pound = pound;
            Keyword = keyword;
            DiagnosticCodes = diagnosticCodes.ToImmutableArray();
        }

        public Token Pound { get; }

        public Token Keyword { get; }

        public ImmutableArray<SyntaxBase> DiagnosticCodes { get; }

        public override TextSpan Span => TextSpan.Between(Keyword, DiagnosticCodes.Last());
    }
}
