using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ObjectPropertySyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public ObjectPropertySyntax(SyntaxBase key, Token colon, SyntaxBase value, IEnumerable<Token> newLines)
        {
            AssertTokenType(colon, nameof(colon), TokenType.Colon);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 1);

            this.Key = key;
            this.Colon = colon;
            this.Value = value;
            this.NewLines = newLines.ToImmutableArray();
        }

        public string GetKeyText()
            => Key switch {
                IdentifierSyntax identifier => identifier.IdentifierName,
                StringSyntax @string => @string.GetLiteralValue(),
                _ => string.Empty,
            };

        public SyntaxBase Key { get; }

        public Token Colon { get; }

        public SyntaxBase Value { get; }

        public ImmutableArray<Token> NewLines { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitObjectPropertySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Key, this.NewLines.Last());
    }
}