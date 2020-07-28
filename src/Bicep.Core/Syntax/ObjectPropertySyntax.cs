using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ObjectPropertySyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public ObjectPropertySyntax(IdentifierSyntax identifier, Token colon, SyntaxBase value, IEnumerable<Token> newLines)
        {
            AssertTokenType(colon, nameof(colon), TokenType.Colon);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 1);

            this.Identifier = identifier;
            this.Colon = colon;
            this.Value = value;
            this.NewLines = newLines.ToImmutableArray();
        }

        public IdentifierSyntax Identifier { get; }

        public Token Colon { get; }

        public SyntaxBase Value { get; }

        public ImmutableArray<Token> NewLines { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitObjectPropertySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Identifier, this.NewLines.Last());
    }
}