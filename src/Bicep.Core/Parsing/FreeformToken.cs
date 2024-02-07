// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class FreeformToken(TokenType type, TextSpan span, string text, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia) : Token(type, span, leadingTrivia, trailingTrivia)
    {
        public override string Text { get; } = text;
    }
}
