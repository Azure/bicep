// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class SkippedTokensTriviaSyntax : SyntaxBase
    {
        public SkippedTokensTriviaSyntax(IEnumerable<Token> tokens, ErrorDiagnostic? errorInfo, Token? errorCause)
        {
            this.Tokens = tokens.ToList().AsReadOnly();
            this.ErrorInfo = errorInfo;
            this.ErrorCause = errorCause;
        }

        public override bool IsSkipped => true;

        /// <summary>
        /// The tokens that were skipped.
        /// </summary>
        public IReadOnlyList<Token> Tokens { get; }

        public ErrorDiagnostic? ErrorInfo { get; }

        /// <summary>
        /// Gets the token that caused the error. This token may fall outside of the Tokens list. For example, an unexpected newline will terminate parsing of a parameter declaration but will not be included in skipped tokens.
        /// </summary>
        public Token? ErrorCause { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitSkippedTokensTriviaSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Tokens.First(), Tokens.Last());
    }
}
