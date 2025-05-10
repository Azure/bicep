// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ObjectTypeSyntax : TypeSyntax
{
    public ObjectTypeSyntax(Token openBrace, IEnumerable<SyntaxBase> children, Token closeBrace)
    {
        AssertTokenType(openBrace, nameof(openBrace), TokenType.LeftBrace);
        AssertTokenType(closeBrace, nameof(closeBrace), TokenType.RightBrace);

        OpenBrace = openBrace;
        Children = [.. children];
        CloseBrace = closeBrace;
    }

    public Token OpenBrace { get; }

    /// <summary>
    /// Gets the child syntax nodes. May return nodes that aren't valid object type properties.
    /// </summary>
    public ImmutableArray<SyntaxBase> Children { get; }

    /// <summary>
    /// Gets the object property types. May return duplicate property types.
    /// </summary>
    public IEnumerable<ObjectTypePropertySyntax> Properties => this.Children.OfType<ObjectTypePropertySyntax>();

    public ObjectTypeAdditionalPropertiesSyntax? AdditionalProperties => Children.OfType<ObjectTypeAdditionalPropertiesSyntax>().FirstOrDefault();

    public Token CloseBrace { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.OpenBrace, this.CloseBrace);
}
