// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestSyntaxFactory
    {
        public static ObjectSyntax CreateObject(IEnumerable<ObjectPropertySyntax> properties) => new ObjectSyntax(CreateToken(TokenType.LeftBrace), CreateChildrenWithNewLines(properties), CreateToken(TokenType.RightBrace));

        public static ArraySyntax CreateArray(IEnumerable<ArrayItemSyntax> items) => new ArraySyntax(CreateToken(TokenType.LeftSquare), CreateChildrenWithNewLines(items), CreateToken(TokenType.RightSquare));

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> itemValues) => CreateArray(itemValues.Select(CreateArrayItem));

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value) => new ArrayItemSyntax(value);

        public static StringSyntax CreateString(string value) => SyntaxFactory.CreateStringLiteral(value);

        public static IntegerLiteralSyntax CreateInt(ulong value) => new(CreateToken(TokenType.Integer), value);

        public static BooleanLiteralSyntax CreateBool(bool value) => new BooleanLiteralSyntax(value ? CreateToken(TokenType.TrueKeyword) : CreateToken(TokenType.FalseKeyword), value);

        public static NullLiteralSyntax CreateNull() => new NullLiteralSyntax(CreateToken(TokenType.NullKeyword));

        public static IdentifierSyntax CreateIdentifier(string identifier) => new IdentifierSyntax(CreateToken(TokenType.Identifier, identifier));

        public static ObjectPropertySyntax CreateProperty(string name, SyntaxBase value) => CreateProperty(CreateIdentifier(name), value);

        public static ObjectPropertySyntax CreateProperty(IdentifierSyntax name, SyntaxBase value) => new ObjectPropertySyntax(name, CreateToken(TokenType.Colon), value);

        public static UnaryOperationSyntax CreateUnaryMinus(SyntaxBase operand) => new(CreateToken(TokenType.Minus), operand);

        public static Token CreateToken(TokenType type, string text = "") => new Token(type, SyntaxFactory.EmptySpan, text, ImmutableArray.Create<SyntaxTrivia>(), ImmutableArray.Create<SyntaxTrivia>());

        private static IEnumerable<SyntaxBase> CreateChildrenWithNewLines(IEnumerable<SyntaxBase> children)
        {
            var result = new List<SyntaxBase>();
            foreach (var child in children)
            {
                result.Add(child);
                result.Add(CreateToken(TokenType.NewLine));
            }

            return result;
        }
    }
}
