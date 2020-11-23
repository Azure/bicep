// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SyntaxTrivia : IPositionable
    {
        public SyntaxTrivia(SyntaxTriviaType type, TextSpan span, string text)
        {
            Type = type;
            Span = span;
            Text = text;
        }

        public SyntaxTriviaType Type { get; }

        public TextSpan Span { get; }

        public string Text { get; }
    }
}
