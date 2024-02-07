// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token(TokenType type, TextSpan span, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia) : SyntaxBase
    {
        public TokenType Type { get; } = type;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitToken(this);

        public override TextSpan Span { get; } = span;

        public virtual string Text => SyntaxFacts.GetText(this.Type) ?? throw new InvalidOperationException($"Unable to get text of token type '{this.Type}'.");

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; } = leadingTrivia.ToImmutableArray();

        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; } = trailingTrivia.ToImmutableArray();
    }
}
