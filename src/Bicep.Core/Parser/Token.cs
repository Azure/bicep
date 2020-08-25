// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token : IPositionable
    {
        public Token(TokenType type, TextSpan span, string text, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia)
        {
            Type = type;
            Span = span;
            Text = text;
            LeadingTrivia = leadingTrivia.ToImmutableArray();
            TrailingTrivia = trailingTrivia.ToImmutableArray();
        }

        public TokenType Type { get; }

        public TextSpan Span { get; }

        public string Text { get; }

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }

        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }
    }
}
