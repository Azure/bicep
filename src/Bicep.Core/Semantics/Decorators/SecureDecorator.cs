// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class SecureDecorator : Decorator
    {
        public SecureDecorator()
            : base(UnionType.Create(LanguageConstants.String, LanguageConstants.Object), new FunctionOverloadBuilder("secure")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Makes the parameter a secure parameter.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override KeyValuePair<string, SyntaxBase>? Evaluate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType)
        {
            if (ReferenceEquals(targetType, LanguageConstants.String))
            {
                return new KeyValuePair<string, SyntaxBase>("type", CreateStringLiteral("secureString"));
            }

            if (ReferenceEquals(targetType, LanguageConstants.Object))
            {
                return new KeyValuePair<string, SyntaxBase>("type", CreateStringLiteral("secureObject"));
            }

            return null;
        }

        private static readonly TextSpan EmptySpan = new TextSpan(0, 0);

        private static readonly IEnumerable<SyntaxTrivia> EmptyTrivia = Enumerable.Empty<SyntaxTrivia>();

        private static Token CreateToken(TokenType tokenType, string text)
            => new Token(tokenType, EmptySpan, text, EmptyTrivia, EmptyTrivia);

        private static StringSyntax CreateStringLiteral(string value)
        {
            return new StringSyntax(CreateStringLiteralToken(value).AsEnumerable(), Enumerable.Empty<SyntaxBase>(), value.AsEnumerable());
        }

        private static Token CreateStringLiteralToken(string value)
        {
            return CreateToken(TokenType.StringComplete, $"'{value}'");
        }
    }
}
