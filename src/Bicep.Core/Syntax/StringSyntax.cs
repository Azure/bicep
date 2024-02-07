// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class StringSyntax(IEnumerable<Token> stringTokens, IEnumerable<SyntaxBase> expressions, IEnumerable<string> segmentValues) : ExpressionSyntax
    {
        public ImmutableArray<Token> StringTokens { get; } = stringTokens.ToImmutableArray();

        public ImmutableArray<SyntaxBase> Expressions { get; } = expressions.ToImmutableArray();

        public ImmutableArray<string> SegmentValues { get; } = segmentValues.ToImmutableArray();

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitStringSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringTokens.First(), StringTokens.Last());
    }
}
