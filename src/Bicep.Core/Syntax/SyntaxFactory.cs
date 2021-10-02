// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public static class SyntaxFactory
    {
        public static readonly TextSpan EmptySpan = new TextSpan(0, 0);

        public static readonly IEnumerable<SyntaxTrivia> EmptyTrivia = Enumerable.Empty<SyntaxTrivia>();

        public static readonly SkippedTriviaSyntax EmptySkippedTrivia = new SkippedTriviaSyntax(EmptySpan, Enumerable.Empty<SyntaxBase>(), Enumerable.Empty<IDiagnostic>());

        public static Token CreateToken(TokenType tokenType, string text = "")
            => new Token(tokenType, EmptySpan, string.IsNullOrEmpty(text) ? TryGetTokenText(tokenType) : text, EmptyTrivia, EmptyTrivia);

        public static IdentifierSyntax CreateIdentifier(string text)
            => new IdentifierSyntax(CreateToken(TokenType.Identifier, text));

        public static VariableAccessSyntax CreateVariableAccess(string text)
            => new VariableAccessSyntax(CreateIdentifier(text));

        public static Token NewlineToken => CreateToken(TokenType.NewLine, Environment.NewLine);
        public static Token AtToken => CreateToken(TokenType.At, "@");
        public static Token LeftBraceToken => CreateToken(TokenType.LeftBrace, "{");
        public static Token RightBraceToken => CreateToken(TokenType.RightBrace, "}");
        public static Token LeftParenToken => CreateToken(TokenType.LeftParen, "(");
        public static Token RightParenToken => CreateToken(TokenType.RightParen, ")");
        public static Token LeftSquareToken => CreateToken(TokenType.LeftSquare, "[");
        public static Token RightSquareToken => CreateToken(TokenType.RightSquare, "]");
        public static Token CommaToken => CreateToken(TokenType.Comma, ",");
        public static Token DotToken => CreateToken(TokenType.Dot, ".");
        public static Token QuestionToken => CreateToken(TokenType.Question, "?");
        public static Token ColonToken => CreateToken(TokenType.Colon, ":");
        public static Token SemicolonToken => CreateToken(TokenType.Semicolon, ";");
        public static Token AssignmentToken => CreateToken(TokenType.Assignment, "=");
        public static Token PlusToken => CreateToken(TokenType.Plus, "+");
        public static Token MinusToken => CreateToken(TokenType.Minus, "-");
        public static Token AsteriskToken => CreateToken(TokenType.Asterisk, "*");
        public static Token SlashToken => CreateToken(TokenType.Slash, "/");
        public static Token ModuloToken => CreateToken(TokenType.Modulo, "%");
        public static Token ExclamationToken => CreateToken(TokenType.Exclamation, "!");
        public static Token LessThanToken => CreateToken(TokenType.LessThan, "<");
        public static Token GreaterThanToken => CreateToken(TokenType.GreaterThan, ">");
        public static Token LessThanOrEqualToken => CreateToken(TokenType.LessThanOrEqual, "<=");
        public static Token GreaterThanOrEqualToken => CreateToken(TokenType.GreaterThanOrEqual, ">=");
        public static Token EqualsToken => CreateToken(TokenType.Equals, "==");
        public static Token NotEqualsToken => CreateToken(TokenType.NotEquals, "!=");
        public static Token EqualsInsensitiveToken => CreateToken(TokenType.EqualsInsensitive, "=~");
        public static Token NotEqualsInsensitiveToken => CreateToken(TokenType.NotEqualsInsensitive, "!~");
        public static Token LogicalAndToken => CreateToken(TokenType.LogicalAnd, "&&");
        public static Token LogicalOrToken => CreateToken(TokenType.LogicalOr, "||");
        public static Token TrueKeywordToken => CreateToken(TokenType.TrueKeyword, "true");
        public static Token FalseKeywordToken => CreateToken(TokenType.FalseKeyword, "false");
        public static Token NullKeywordToken => CreateToken(TokenType.NullKeyword, "null");

        public static ObjectPropertySyntax CreateObjectProperty(string key, SyntaxBase value)
            => new ObjectPropertySyntax(CreateObjectPropertyKey(key), CreateToken(TokenType.Colon, ":"), value);

        public static ObjectSyntax CreateObject(IEnumerable<ObjectPropertySyntax> properties)
        {
            var children = new List<SyntaxBase> { NewlineToken };

            foreach (var property in properties)
            {
                children.Add(property);
                children.Add(NewlineToken);
            }

            return new ObjectSyntax(
                LeftBraceToken,
                children,
                RightBraceToken);
        }

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value)
            => new ArrayItemSyntax(value);

        public static ForSyntax CreateRangedForSyntax(string indexIdentifier, SyntaxBase count, SyntaxBase body)
        {
            // generates "range(0, <count>)"
            var rangeSyntax = new FunctionCallSyntax(
                CreateIdentifier("range"),
                LeftParenToken,
                new FunctionArgumentSyntax[] {
                    new FunctionArgumentSyntax(
                        new IntegerLiteralSyntax(CreateToken(TokenType.Integer, "0"), 0),
                        CommaToken),
                    new FunctionArgumentSyntax(count, null),
                },
                RightParenToken);

            return CreateForSyntax(indexIdentifier, rangeSyntax, body);
        }

        public static ForSyntax CreateForSyntax(string indexIdentifier, SyntaxBase inSyntax, SyntaxBase body)
        {
            // generates "[for <identifier> in <inSyntax>: <body>]"
            return new(
                LeftSquareToken,
                CreateToken(TokenType.Identifier, "for"),
                new LocalVariableSyntax(new IdentifierSyntax(CreateToken(TokenType.Identifier, indexIdentifier))),
                CreateToken(TokenType.Identifier, "in"),
                inSyntax,
                ColonToken,
                body,
                RightSquareToken);
        }

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> items)
        {
            var children = new List<SyntaxBase> { NewlineToken };

            foreach (var item in items)
            {
                children.Add(CreateArrayItem(item));
                children.Add(NewlineToken);
            }

            return new ArraySyntax(
                LeftSquareToken,
                children,
                RightSquareToken);
        }

        public static SyntaxBase CreateObjectPropertyKey(string text)
        {
            if (Regex.IsMatch(text, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                return CreateIdentifier(text);
            }

            return CreateStringLiteral(text);
        }

        public static IntegerLiteralSyntax CreateIntegerLiteral(long value) => new(CreateToken(TokenType.Integer, value.ToString()), value);

        public static StringSyntax CreateStringLiteral(string value) => CreateString(value.AsEnumerable(), Enumerable.Empty<SyntaxBase>());

        public static BooleanLiteralSyntax CreateBooleanLiteral(bool value) => new(TrueKeywordToken, value);

        public static NullLiteralSyntax CreateNullLiteral() => new(NullKeywordToken);

        public static StringSyntax CreateString(IEnumerable<string> values, IEnumerable<SyntaxBase> expressions)
        {
            var valuesArray = values.ToArray();
            var expressionsArray = expressions.ToArray();
            
            if (valuesArray.Length != expressionsArray.Length + 1)
            {
                throw new ArgumentException($"The number of values must be 1 greater than the number of expressions");
            }

            var stringTokens = new List<Token>();
            for (var i = 0; i < valuesArray.Length; i++)
            {
                var isStart = (i == 0);
                var isEnd = (i == valuesArray.Length - 1);

                stringTokens.Add(CreateStringInterpolationToken(isStart, isEnd, valuesArray[i]));
            }

            return new StringSyntax(stringTokens, expressionsArray, valuesArray);
        }

        public static StringSyntax CreateStringLiteralWithComment(string value, string comment)
        {
            var trailingTrivia = new SyntaxTrivia(SyntaxTriviaType.MultiLineComment, EmptySpan, $"/*{comment.Replace("*/", "*\\/")}*/");
            var stringToken = new Token(TokenType.StringComplete, EmptySpan, $"'{EscapeBicepString(value)}'", EmptyTrivia, trailingTrivia.AsEnumerable());

            return new StringSyntax(stringToken.AsEnumerable(), Enumerable.Empty<SyntaxBase>(), value.AsEnumerable());
        }

        public static Token CreateStringLiteralToken(string value)
        {
            return CreateToken(TokenType.StringComplete, $"'{EscapeBicepString(value)}'");
        }

        public static StringSyntax CreateInterpolatedKey(SyntaxBase syntax)
        {
            var startToken = CreateStringInterpolationToken(true, false, "");
            var endToken = CreateStringInterpolationToken(false, true, "");

            return new StringSyntax(
                new[] { startToken, endToken },
                syntax.AsEnumerable(),
                new[] { "", "" });
        }

        public static Token CreateStringInterpolationToken(bool isStart, bool isEnd, string value)
        {
            return (isStart, isEnd) switch {
                (true, true) => CreateToken(TokenType.StringComplete, $"'{EscapeBicepString(value)}'"),
                (true, false) => CreateToken(TokenType.StringLeftPiece, $"'{EscapeBicepString(value)}${{"),
                (false, false) => CreateToken(TokenType.StringMiddlePiece, $"}}{EscapeBicepString(value)}${{"),
                (false, true) => CreateToken(TokenType.StringRightPiece, $"}}{EscapeBicepString(value)}'"),
            };
        }

        public static FunctionCallSyntax CreateFunctionCall(string functionName)
            => CreateFunctionCall(functionName, new FunctionArgumentSyntax[0]);

        public static FunctionCallSyntax CreateFunctionCall(string functionName, params SyntaxBase[] argumentExpressions)
        {
            var arguments = argumentExpressions.Select((expression, i) => new FunctionArgumentSyntax(
                expression,
                i < argumentExpressions.Length - 1 ? CommaToken : null));

            return new FunctionCallSyntax(
                CreateIdentifier(functionName),
                LeftParenToken,
                arguments,
                RightParenToken);
        }

        public static FunctionCallSyntax CreateFunctionCall(string functionName, params FunctionArgumentSyntax[] argumentSyntaxes)
            => new FunctionCallSyntax(
                CreateIdentifier(functionName),
                LeftParenToken,
                argumentSyntaxes,
                RightParenToken);

        public static DecoratorSyntax CreateDecorator(string functionName, params SyntaxBase[] argumentExpressions)
        {
            return new DecoratorSyntax(AtToken, CreateFunctionCall(functionName, argumentExpressions));
        }

        private static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");

        private static string TryGetTokenText(TokenType tokenType) => tokenType switch
        {
            TokenType.At => "@",
            TokenType.LeftBrace => "{",
            TokenType.RightBrace => "}",
            TokenType.LeftParen => "(",
            TokenType.RightParen => ")",
            TokenType.LeftSquare => "[",
            TokenType.RightSquare => "]",
            TokenType.Comma => ",",
            TokenType.Dot => ".",
            TokenType.Question => "?",
            TokenType.Colon => ":",
            TokenType.Semicolon => ";",
            TokenType.Assignment => "=",
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Asterisk => "*",
            TokenType.Slash => "/",
            TokenType.Modulo => "%",
            TokenType.Exclamation => "!",
            TokenType.LessThan => "<",
            TokenType.GreaterThan => ">",
            TokenType.LessThanOrEqual => "<=",
            TokenType.GreaterThanOrEqual => ">=",
            TokenType.Equals => "==",
            TokenType.NotEquals => "!=",
            TokenType.EqualsInsensitive => "=~",
            TokenType.NotEqualsInsensitive => "!~",
            TokenType.LogicalAnd => "&&",
            TokenType.LogicalOr => "||",
            TokenType.TrueKeyword => "true",
            TokenType.FalseKeyword => "false",
            TokenType.NullKeyword => "null",
            _ => ""
        };

        public static SyntaxBase FlattenStringOperations(SyntaxBase original)
        {
            if (original is FunctionCallSyntax functionCallSyntax)
            {
                if (functionCallSyntax.NameEquals("concat"))
                {
                    // just return the inner portion if there's only one concat entry
                    if (functionCallSyntax.Arguments.Length == 1)
                    {
                        return functionCallSyntax.Arguments.Select(arg => arg.Expression).First();
                    }
                    else
                    {
                        var concatArguments = new List<FunctionArgumentSyntax>();
                        foreach (var arg in functionCallSyntax.Arguments)
                        {
                            // recurse
                            var flattenedExpression = FlattenStringOperations(arg.Expression);

                            if (flattenedExpression is FunctionCallSyntax childFunction && childFunction.NameEquals("concat"))
                            {
                                // concat directly inside a concat - break it out
                                concatArguments.AddRange(childFunction.Arguments);
                                continue;
                            }

                            concatArguments.Add(new FunctionArgumentSyntax(flattenedExpression, arg.Comma));
                        }

                        // overwrite the original expression
                        functionCallSyntax = CreateFunctionCall("concat", CombineConcatArguments(concatArguments).ToArray());
                        return functionCallSyntax;
                    }
                }
            }

            // return unchanged
            return original;
        }

        private static IEnumerable<FunctionArgumentSyntax> CombineConcatArguments(IEnumerable<FunctionArgumentSyntax> arguments)
        {
            var stringList = new List<string>();
            foreach (var argument in arguments)
            {
                // accumulate string literals if possible
                if (argument.Expression is StringSyntax stringSyntax)
                {
                    if (!stringSyntax.Expressions.Any())
                    {
                        foreach (var text in stringSyntax.SegmentValues)
                        {
                            stringList.Add(text);
                        }

                        // don't return the string yet - there may be another string to join
                        continue;
                    }
                }

                // check to see if a previous string needs to be yielded back
                if (stringList.Any())
                {
                    yield return new FunctionArgumentSyntax(CreateStringLiteral(string.Join("", stringList)), null);
                    stringList.Clear();
                }

                // if the arg is not a string it will yield back here
                yield return argument;
            }

            // yield final string if there is one
            if (stringList.Any())
            {
                yield return new FunctionArgumentSyntax(CreateStringLiteral(string.Join("", stringList)), null);
            }
        }

        public static Token CreateNewLineWithIndent(string indent)
        {
            return new Token(
                TokenType.NewLine,
                SyntaxFactory.EmptySpan,
                Environment.NewLine,
                SyntaxFactory.EmptyTrivia,
                new SyntaxTrivia[] { new SyntaxTrivia(SyntaxTriviaType.Whitespace, SyntaxFactory.EmptySpan, indent) });
        }
    }
}
