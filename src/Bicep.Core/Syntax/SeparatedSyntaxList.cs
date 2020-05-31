using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class SeparatedSyntaxList : SyntaxBase
    {
        public SeparatedSyntaxList(IEnumerable<SyntaxBase> elements, IEnumerable<Token> separators, TextSpan span)
        {
            Elements = elements.ToList();
            Separators = separators.ToList();
            Span = span;
        }

        public IReadOnlyList<SyntaxBase> Elements { get; }

        public IReadOnlyList<Token> Separators { get; }

        public IEnumerable<(SyntaxBase, Token?)> GetPairedElements()
        {
            foreach (var (element, separator) in Elements.Zip(Separators, Tuple.Create))
            {
                yield return (element, separator);
            }

            if (Elements.Count > Separators.Count)
            {
                yield return (Elements.Last(), null);
            }
        }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitSeparatedSyntaxList(this);

        public override TextSpan Span { get; }
    }
}