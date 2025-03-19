// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Parsing
{
    public class FreeformToken : Token
    {
        public FreeformToken(TokenType type, TextSpan span, string text, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia)
            : base(type, span, leadingTrivia, trailingTrivia)
        {
            this.Text = text;
        }

        public override string Text { get; }
    }
}
