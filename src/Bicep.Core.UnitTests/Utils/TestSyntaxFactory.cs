// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestSyntaxFactory
    {
        public static ObjectSyntax CreateObject(IEnumerable<ObjectPropertySyntax> properties) => new ObjectSyntax(CreateToken(TokenType.LeftBrace), CreateNewLines(), properties, CreateToken(TokenType.RightBrace));

        public static ArraySyntax CreateArray(IEnumerable<ArrayItemSyntax> items) => new ArraySyntax(CreateToken(TokenType.LeftSquare), CreateNewLines(), items, CreateToken(TokenType.RightSquare));

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> itemValues) => CreateArray(itemValues.Select(CreateArrayItem));

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value) => new ArrayItemSyntax(value, CreateNewLines());

        // TODO: Escape string correctly
        public static StringSyntax CreateString(string value)
        {
            var token = CreateToken(TokenType.StringComplete, StringUtils.EscapeBicepString(value));
            var segment = Lexer.TryGetStringValue(token) ?? throw new ArgumentException($"Unable to parse {value}");

            return new StringSyntax(new [] { token }, Enumerable.Empty<SyntaxBase>(), new [] { segment });
        }

        public static NumericLiteralSyntax CreateInt(int value) => new NumericLiteralSyntax(CreateToken(TokenType.Number), value);

        public static BooleanLiteralSyntax CreateBool(bool value) => new BooleanLiteralSyntax(value ? CreateToken(TokenType.TrueKeyword) : CreateToken(TokenType.FalseKeyword), value);

        public static NullLiteralSyntax CreateNull() => new NullLiteralSyntax(CreateToken(TokenType.NullKeyword));

        public static IdentifierSyntax CreateIdentifier(string identifier) => new IdentifierSyntax(CreateToken(TokenType.Identifier, identifier));

        public static ObjectPropertySyntax CreateProperty(string name, SyntaxBase value) => CreateProperty(CreateIdentifier(name), value);

        public static ObjectPropertySyntax CreateProperty(IdentifierSyntax name, SyntaxBase value) => new ObjectPropertySyntax(name, CreateToken(TokenType.Colon), value, CreateNewLines());

        private static Token CreateToken(TokenType type, string text = "") => new Token(type, new TextSpan(0, 0), text, ImmutableArray.Create<SyntaxTrivia>(), ImmutableArray.Create<SyntaxTrivia>());

        private static Token[] CreateNewLines() => new[] {CreateToken(TokenType.NewLine)};
    }
}

