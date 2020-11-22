// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SeparatedSyntaxList : SyntaxBase
    {
        public SeparatedSyntaxList(IEnumerable<SyntaxBase> elements, IEnumerable<SyntaxBase> separators, TextSpan span)
        {
            this.Elements = elements.ToImmutableArray();
            this.Separators = separators.ToImmutableArray();
            this.Span = span;

            if (this.Elements.Any())
            {
                if (this.Elements.Length != this.Separators.Length && this.Elements.Length != (this.Separators.Length + 1))
                {
                    throw new ArgumentException($"The number of separators ({this.Separators.Length}) must be the same or one less than the number of elements ({this.Elements.Length}).");
                }

                // with one or more elements, the span should be from the beginning of the first element to the end of the last element
                var expectedSpan = TextSpan.Between(this.Elements.First().Span, this.Elements.Last().Span);
                if (span.Equals(expectedSpan) == false)
                {
                    throw new ArgumentException($"The specified span '{span}' must be from the beginning of the first element (inclusive) to the end of the last element (exclusive). Expected span '{expectedSpan}'.");
                }
            }
            else
            {
                if (this.Separators.Length != 0)
                {
                    throw new ArgumentException("With zero elements, the number of separators must also be zero.");
                }
                
                if (span.Length != 0)
                {
                    throw new ArgumentException($"The specified span was '{span}' but expected a zero-length span.");
                }
            }
        }

        public ImmutableArray<SyntaxBase> Elements { get; }

        public ImmutableArray<SyntaxBase> Separators { get; }

        public IEnumerable<(SyntaxBase, SyntaxBase?)> GetPairedElements()
        {
            foreach (var (element, separator) in this.Elements.Zip(this.Separators, Tuple.Create))
            {
                yield return (element, separator);
            }

            if (this.Elements.Length > this.Separators.Length)
            {
                yield return (Elements.Last(), null);
            }
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSeparatedSyntaxList(this);

        public override TextSpan Span { get; }
    }
}