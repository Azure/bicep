// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class StringSyntax : ExpressionSyntax
    {
        public StringSyntax(IEnumerable<Token> stringTokens, IEnumerable<SyntaxBase> expressions, IEnumerable<string> segmentValues)
        {
            this.StringTokens = stringTokens.ToImmutableArray();
            this.Expressions = expressions.ToImmutableArray();
            this.SegmentValues = segmentValues.ToImmutableArray();
        }

        public ImmutableArray<Token> StringTokens { get; }

        public ImmutableArray<SyntaxBase> Expressions { get; }

        public ImmutableArray<string> SegmentValues { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitStringSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringTokens.First(), StringTokens.Last());
    }
}
