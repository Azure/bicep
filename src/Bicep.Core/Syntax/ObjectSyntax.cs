// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ObjectSyntax : SyntaxBase, IExpressionSyntax
    {
        public ObjectSyntax(Token openBrace, IEnumerable<Token> newLines, IEnumerable<SyntaxBase> children, Token closeBrace)
        {
            AssertTokenType(openBrace, nameof(openBrace), TokenType.LeftBrace);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 0);
            AssertTokenType(closeBrace, nameof(closeBrace), TokenType.RightBrace);

            this.OpenBrace = openBrace;
            this.NewLines = newLines.ToImmutableArray();
            this.Children = children.ToImmutableArray();
            this.CloseBrace = closeBrace;
        }

        public Token OpenBrace { get; }

        public ImmutableArray<Token> NewLines { get; }

        /// <summary>
        /// Gets the child syntax nodes. May return nodes that aren't valid object properties.
        /// </summary>
        public ImmutableArray<SyntaxBase> Children { get; }

        public Token CloseBrace { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitObjectSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(this.OpenBrace, this.CloseBrace);

        /// <summary>
        /// Gets the object properties. May return duplicate properties.
        /// </summary>
        public IEnumerable<ObjectPropertySyntax> Properties => this.Children.OfType<ObjectPropertySyntax>();

        public ExpressionKind ExpressionKind => ExpressionKind.ComplexLiteral;
    }
}
