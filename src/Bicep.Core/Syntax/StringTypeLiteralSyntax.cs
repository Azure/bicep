// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class StringTypeLiteralSyntax(IEnumerable<Token> stringTokens, IEnumerable<SyntaxBase> expressions, IEnumerable<string> segmentValues) : TypeSyntax
    {
        public ImmutableArray<Token> StringTokens { get; } = [.. stringTokens];

        public ImmutableArray<SyntaxBase> Expressions { get; } = [.. expressions];

        public ImmutableArray<string> SegmentValues { get; } = [.. segmentValues];

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitStringTypeLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringTokens.First(), StringTokens.Last());

        /// <summary>
        /// Returns the span between the quotes for a string token.
        /// </summary>
        public TextSpan GetInnerSpan()
        {
            var skipChars = StringTokens.First().Type == TokenType.MultilineString ? 3 : 1;
            var outerSpan = Span;

            return new(outerSpan.Position + skipChars, outerSpan.Length - (skipChars * 2));
        }
    }
}
