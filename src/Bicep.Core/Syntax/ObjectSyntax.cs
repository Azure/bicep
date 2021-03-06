// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ObjectSyntax : ExpressionSyntax
    {
        public ObjectSyntax(Token openBrace, IEnumerable<SyntaxBase> children, Token closeBrace)
        {
            AssertTokenType(openBrace, nameof(openBrace), TokenType.LeftBrace);
            AssertTokenType(closeBrace, nameof(closeBrace), TokenType.RightBrace);

            this.OpenBrace = openBrace;
            this.Children = children.ToImmutableArray();
            this.CloseBrace = closeBrace;
        }

        public Token OpenBrace { get; }

        /// <summary>
        /// Gets the child syntax nodes. May return nodes that aren't valid object properties.
        /// </summary>
        public ImmutableArray<SyntaxBase> Children { get; }

        public Token CloseBrace { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitObjectSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(this.OpenBrace, this.CloseBrace);

        /// <summary>
        /// Gets the object properties. May return duplicate properties.
        /// </summary>
        public IEnumerable<ObjectPropertySyntax> Properties => this.Children.OfType<ObjectPropertySyntax>();

        /// <summary>
        /// Gets the child resources of this object.
        /// </summary>
        public IEnumerable<ResourceDeclarationSyntax> Resources => this.Children.OfType<ResourceDeclarationSyntax>();
    }
}
