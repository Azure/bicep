using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ObjectSyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public ObjectSyntax(Token openBrace, IEnumerable<Token> newLines, IEnumerable<ObjectPropertySyntax> properties, Token closeBrace)
        {
            AssertTokenType(openBrace, nameof(openBrace), TokenType.LeftBrace);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 1);
            AssertTokenType(closeBrace, nameof(closeBrace), TokenType.RightBrace);

            this.OpenBrace = openBrace;
            this.NewLines = newLines.ToImmutableArray();
            this.Properties = properties.ToImmutableArray();
            this.CloseBrace = closeBrace;
        }

        public Token OpenBrace { get; }

        public ImmutableArray<Token> NewLines { get; }

        public ImmutableArray<ObjectPropertySyntax> Properties { get; }

        public Token CloseBrace { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitObjectSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(this.OpenBrace, this.CloseBrace);
    }
}