// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public static class SyntaxExtensions
    {
        public static bool IsSingleLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.SingleLineComment;

        public static bool IsMultiLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.MultiLineComment;

        public static bool IsComment(this SyntaxTrivia? trivia) => IsSingleLineComment(trivia) || IsMultiLineComment(trivia);

        public static bool IsOf(this Token token, TokenType type) => token.Type == type;

        public static bool IsOneOf(this Token token, TokenType firstType, TokenType secondType, params TokenType[] types) =>
            types.Append(firstType).Append(secondType).Any(x => token.Type == x);

        public static bool IsMultiLineNewLine(this Token token) => token.IsOf(TokenType.NewLine) && StringUtils.CountNewlines(token.Text) > 1;

        public static bool HasProperties(this ObjectSyntax syntax) => syntax.Properties.Any();

        public static bool NameEquals(this FunctionCallSyntax funcSyntax, string compareTo)
            => LanguageConstants.IdentifierComparer.Equals(funcSyntax.Name.IdentifierName, compareTo);

        public static bool NameEquals(this IdentifierSyntax identifier, string compareTo)
            => LanguageConstants.IdentifierComparer.Equals(identifier.IdentifierName, compareTo);

        private static TypeProperty? TryGetTypeProperty(SemanticModel model, SyntaxBase objectSyntax, string propertyName)
        {
            // Cannot use assigned type here because it won't handle the case where the property value
            // is an array accesss or a string interpolation.
            return model.TypeManager.GetDeclaredType(objectSyntax) switch
            {
                ObjectType { Properties: var properties }
                    when properties.TryGetValue(propertyName, out var typeProperty) => typeProperty,
                DiscriminatedObjectType { DiscriminatorKey: var discriminatorKey, DiscriminatorProperty: var typeProperty }
                    when LanguageConstants.IdentifierComparer.Equals(propertyName, discriminatorKey) => typeProperty,
                _ => null,
            };
        }

        public static TypeProperty? TryGetTypeProperty(this ObjectPropertySyntax syntax, SemanticModel model)
        {
            if (syntax.TryGetKeyText() is not string propertyName || model.Binder.GetParent(syntax) is not ObjectSyntax objectSyntax)
            {
                return null;
            }

            return TryGetTypeProperty(model, objectSyntax, propertyName);
        }

        /// <remarks>
        /// If a chain of accesses starts with a "safe" access (e.g., <code><i>base</i>[?0].property</code> or <code><i>base</i>.?some.deeply.nested.property</code>),
        /// it may short-circuit at runtime, meaning that <code>.deeply.nested.property</code> will only be evaluated if <code><i>base</i>.?some</code> returns a non-null value.
        /// The upshot of this is that we will need to mark <code><i>base</i>.?some</code> as non-nullable when evaluating any chained property accesses, then
        /// mark the resultant type as nullable iff the original "safe" access might return null.
        /// Because of this requirement, it's necessary to evaluate the full access chain and determine if it is kicked off by a .? or [?] operator rather than
        /// just evaluating <code>syntax.BaseExpression</code> recursively
        /// </remarks>
        public static Stack<AccessExpressionSyntax> ToAccessExpressionStack(this AccessExpressionSyntax syntax)
        {
            Stack<AccessExpressionSyntax> chainedAccesses = new();
            chainedAccesses.Push(syntax);

            while (chainedAccesses.TryPeek(out var current) && current.SafeAccessMarker is null && current.BaseExpression is AccessExpressionSyntax baseAccessExpression)
            {
                chainedAccesses.Push(baseAccessExpression);
            }

            return chainedAccesses;
        }
    }
}
