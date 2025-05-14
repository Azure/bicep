// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Parsing
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token : SyntaxBase
    {
        public Token(TokenType type, TextSpan span, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia)
        {
            Type = type;
            Span = span;
            LeadingTrivia = [.. leadingTrivia];
            TrailingTrivia = [.. trailingTrivia];

#if DEBUG
            var leadingNonNil = LeadingTrivia.Where(x => x.Span != TextSpan.Nil).ToArray();
            var trailingNonNil = TrailingTrivia.Where(x => x.Span != TextSpan.Nil).ToArray();

            Debug.Assert(leadingNonNil.Length == 0
                || leadingNonNil.Zip(leadingNonNil.Skip(1), (a, b) => !TextSpan.AreOverlapping(a.Span, b.Span) && a.Span.Position < b.Span.Position)
                    .All(x => x == true),
                "Leading trivia should be in numeric non-overlapping order");
            Debug.Assert(trailingNonNil.Length == 0
                || trailingNonNil.Zip(trailingNonNil.Skip(1), (a, b) => !TextSpan.AreOverlapping(a.Span, b.Span) && a.Span.Position < b.Span.Position)
                    .All(x => x == true),
                "Trailing trivia should be in numeric order by end position");

            Debug.Assert(leadingNonNil.All(x => x.Span.Position < span.Position && !TextSpan.AreOverlapping(x.Span, span)),
                "Leading trivia should not overlap token span");
            Debug.Assert(trailingNonNil.All(x => x.Span.Position > span.Position && !TextSpan.AreOverlapping(x.Span, span)),
                "Trailing trivia should not overlap token span");
#endif
        }

        public TokenType Type { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitToken(this);

        public override TextSpan Span { get; }

        public virtual string Text => SyntaxFacts.GetText(this.Type) ?? throw new InvalidOperationException($"Unable to get text of token type '{this.Type}'.");

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }

        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }
    }
}
