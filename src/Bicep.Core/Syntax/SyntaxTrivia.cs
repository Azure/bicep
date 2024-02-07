// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SyntaxTrivia(SyntaxTriviaType type, TextSpan span, string text) : IPositionable
    {
        public SyntaxTriviaType Type { get; } = type;

        public TextSpan Span { get; } = span;

        public string Text { get; } = text;
    }
}
