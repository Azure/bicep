// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class ObjectPropertySyntax : ExpressionSyntax
    {
        public ObjectPropertySyntax(SyntaxBase key, SyntaxBase colon, SyntaxBase value)
        {
            AssertSyntaxType(key, nameof(key), typeof(IdentifierSyntax), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

            this.Key = key;
            this.Colon = colon;
            this.Value = value;
        }

        public string? TryGetKeyText()
            => Key switch {
                IdentifierSyntax identifier => identifier.IdentifierName,
                StringSyntax @string => @string.TryGetLiteralValue(),
                SkippedTriviaSyntax _ => null,
                // this should not be possible as we assert the type in the constructor
                _ => throw new InvalidOperationException($"Unexpected key syntax {Key.GetType()}"),
            };

        public SyntaxBase Key { get; }

        public SyntaxBase Colon { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectPropertySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Key, this.Value);

        public TypeProperty? TryGetTypeProperty(IBinder binder, ITypeManager typeManager)
        {
            if (this.TryGetKeyText() is not string propertyName || binder.GetParent(this) is not ObjectSyntax objectSyntax)
            {
                return null;
            }

            // Cannot use assigned type here because it won't handle the case where the propery value
            // is an array accesss or a string interpolation.
            return typeManager.GetDeclaredType(objectSyntax) switch
            {
                ObjectType { Properties: var properties }
                    when properties.TryGetValue(propertyName, out var typeProperty) => typeProperty,
                DiscriminatedObjectType { DiscriminatorKey: var discriminatorKey, DiscriminatorProperty: var typeProperty }
                    when propertyName.Equals(discriminatorKey, LanguageConstants.IdentifierComparison) => typeProperty,
                _ => null,
            };
        }
    }
}
