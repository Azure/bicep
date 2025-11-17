// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public static class SyntaxFactory
    {
        public static readonly IEnumerable<SyntaxTrivia> EmptyTrivia = [];

        public static readonly IEnumerable<SyntaxTrivia> SingleSpaceTrivia = ImmutableArray.Create(
            new SyntaxTrivia(SyntaxTriviaType.Whitespace, TextSpan.Nil, " "));

        public static readonly SkippedTriviaSyntax EmptySkippedTrivia = new(TextSpan.Nil, []);

        public static readonly ProgramSyntax EmptyProgram = new(Array.Empty<SyntaxBase>(), EndOfFileToken);

        public static Token CreateToken(TokenType tokenType, IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null)
        {
            leadingTrivia ??= EmptyTrivia;
            trailingTrivia ??= EmptyTrivia;

            return new(tokenType, TextSpan.Nil, leadingTrivia, trailingTrivia);
        }

        public static FreeformToken CreateFreeformToken(TokenType tokenType, string text, IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null)
        {
            leadingTrivia ??= EmptyTrivia;
            trailingTrivia ??= EmptyTrivia;

            return new(tokenType, TextSpan.Nil, text, leadingTrivia, trailingTrivia);
        }

        public static FreeformToken CreateIdentifierToken(string text, IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null) =>
            CreateFreeformToken(TokenType.Identifier, text, leadingTrivia, trailingTrivia);

        public static FreeformToken CreateIdentifierTokenWithTrailingSpace(string text) => CreateIdentifierToken(text, EmptyTrivia, SingleSpaceTrivia);

        public static IdentifierSyntax CreateIdentifier(string text) => new(CreateIdentifierToken(text));

        public static IdentifierSyntax CreateIdentifierWithTrailingSpace(string text) => new(CreateIdentifierTokenWithTrailingSpace(text));

        public static VariableAccessSyntax CreateVariableAccess(string text) => new(CreateIdentifier(text));

        public static VariableBlockSyntax CreateVariableBlock(IEnumerable<IdentifierSyntax> variables)
            => new(
                LeftParenToken,
                Interleave(variables.Select(x => new LocalVariableSyntax(x)), () => CommaToken),
                SyntaxFactory.RightParenToken);

        public static Token DoubleNewlineToken => CreateFreeformToken(TokenType.NewLine, Environment.NewLine + Environment.NewLine);
        public static Token NewlineToken => CreateFreeformToken(TokenType.NewLine, Environment.NewLine);
        public static Token GetNewlineToken(IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null)
            => CreateFreeformToken(TokenType.NewLine, Environment.NewLine, leadingTrivia, trailingTrivia);
        public static Token AtToken => CreateToken(TokenType.At);
        public static Token LeftBraceToken => CreateToken(TokenType.LeftBrace);
        public static Token RightBraceToken => CreateToken(TokenType.RightBrace);
        public static Token LeftParenToken => CreateToken(TokenType.LeftParen);
        public static Token RightParenToken => CreateToken(TokenType.RightParen);
        public static Token LeftSquareToken => CreateToken(TokenType.LeftSquare);
        public static Token RightSquareToken => CreateToken(TokenType.RightSquare);
        public static Token CommaToken => GetCommaToken();
        public static Token GetCommaToken(IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null)
            => CreateToken(TokenType.Comma, leadingTrivia, trailingTrivia);
        public static Token DotToken => CreateToken(TokenType.Dot);
        public static Token QuestionToken => CreateToken(TokenType.Question);
        public static Token DoubleQuestionToken => CreateToken(TokenType.DoubleQuestion);
        public static Token ColonToken => CreateToken(TokenType.Colon);
        public static Token SemicolonToken => CreateToken(TokenType.Semicolon);
        public static Token AssignmentToken => CreateToken(TokenType.Assignment, EmptyTrivia, SingleSpaceTrivia);
        public static Token PlusToken => CreateToken(TokenType.Plus);
        public static Token MinusToken => CreateToken(TokenType.Minus);
        public static Token AsteriskToken => CreateToken(TokenType.Asterisk);
        public static Token SlashToken => CreateToken(TokenType.Slash);
        public static Token ModuloToken => CreateToken(TokenType.Modulo);
        public static Token ExclamationToken => CreateToken(TokenType.Exclamation);
        public static Token LessThanToken => CreateToken(TokenType.LeftChevron);
        public static Token GreaterThanToken => CreateToken(TokenType.RightChevron);
        public static Token LessThanOrEqualToken => CreateToken(TokenType.LessThanOrEqual);
        public static Token GreaterThanOrEqualToken => CreateToken(TokenType.GreaterThanOrEqual);
        public static Token EqualsToken => CreateToken(TokenType.Equals);
        public static Token NotEqualsToken => CreateToken(TokenType.NotEquals);
        public static Token EqualsInsensitiveToken => CreateToken(TokenType.EqualsInsensitive);
        public static Token NotEqualsInsensitiveToken => CreateToken(TokenType.NotEqualsInsensitive);
        public static Token LogicalAndToken => CreateToken(TokenType.LogicalAnd);
        public static Token LogicalOrToken => CreateToken(TokenType.LogicalOr);
        public static Token TrueKeywordToken => CreateToken(TokenType.TrueKeyword);
        public static Token FalseKeywordToken => CreateToken(TokenType.FalseKeyword);
        public static Token NullKeywordToken => CreateToken(TokenType.NullKeyword);
        public static Token ArrowToken => CreateToken(TokenType.Arrow);
        public static Token EndOfFileToken => CreateToken(TokenType.EndOfFile);
        public static Token EllipsisToken => CreateToken(TokenType.Ellipsis);
        public static Token HatToken => CreateToken(TokenType.Hat);
        public static Token TargetScopeKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.TargetScopeKeyword);
        public static Token ImportKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ImportKeyword);
        public static Token ExtensionKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ExtensionKeyword);
        public static Token UsingKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.UsingKeyword);
        public static Token MetadataKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.MetadataKeyword);
        public static Token ParameterKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ParameterKeyword);
        public static Token ExtensionConfigKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ExtensionConfigKeyword);
        public static Token VariableKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.VariableKeyword);
        public static Token FunctionKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.FunctionKeyword);
        public static Token ResourceKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ResourceKeyword);
        public static Token ModuleKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ModuleKeyword);
        public static Token OutputKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.OutputKeyword);
        public static Token IfKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.IfKeyword);
        public static Token ForKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.ForKeyword);
        public static Token InKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.InKeyword);
        public static Token TypeKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.TypeKeyword);
        public static Token WithKeywordToken => CreateIdentifierTokenWithTrailingSpace(LanguageConstants.WithKeyword);

        public static ObjectPropertySyntax CreateObjectProperty(string key, SyntaxBase value)
        {
            if (value is SkippedTriviaSyntax)
            {
                return new ObjectPropertySyntax(CreateObjectPropertyKey(key), CreateToken(TokenType.Colon, EmptyTrivia), value);
            }

            return new ObjectPropertySyntax(CreateObjectPropertyKey(key), CreateToken(TokenType.Colon, EmptyTrivia, SingleSpaceTrivia), value);
        }

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
            => new(value);

        public static ForSyntax CreateRangedForSyntax(string indexIdentifier, SyntaxBase count, SyntaxBase body)
        {
            // generates "range(0, <count>)"
            var rangeSyntax = new FunctionCallSyntax(
                CreateIdentifier("range"),
                LeftParenToken,
                new SyntaxBase[] {
                    new FunctionArgumentSyntax(new IntegerLiteralSyntax(CreateFreeformToken(TokenType.Integer, "0"), 0)),
                    CommaToken,
                    new FunctionArgumentSyntax(count),
                },
                RightParenToken);

            return CreateForSyntax(indexIdentifier, rangeSyntax, body);
        }

        public static ForSyntax CreateForSyntax(string indexIdentifier, SyntaxBase inSyntax, SyntaxBase body)
        {
            // generates "[for <identifier> in <inSyntax>: <body>]"
            return new(
                LeftSquareToken,
                [],
                ForKeywordToken,
                new LocalVariableSyntax(CreateIdentifierWithTrailingSpace(indexIdentifier)),
                InKeywordToken,
                inSyntax,
                ColonToken,
                body,
                [],
                RightSquareToken);
        }

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> items)
        {
            var children = new List<SyntaxBase>();

            foreach (var item in items)
            {
                children.Add(NewlineToken);
                children.Add(CreateArrayItem(item));
            }

            if (children.Any())
            {
                // only add a newline if we actually have children
                children.Add(NewlineToken);
            }

            return new ArraySyntax(
                LeftSquareToken,
                children,
                RightSquareToken);
        }

        public static ArrayAccessSyntax CreateArrayAccess(SyntaxBase baseExpression, SyntaxBase indexExpression, bool safeAccess = false, bool fromEnd = false)
            => new(baseExpression, LeftSquareToken, safeAccess ? QuestionToken : null, fromEnd ? HatToken : null, indexExpression, RightSquareToken);

        public static SyntaxBase CreateObjectPropertyKey(string text)
        {
            if (Lexer.IsValidIdentifier(text))
            {
                return CreateIdentifier(text);
            }

            return CreateStringLiteral(text);
        }

        public static IntegerLiteralSyntax CreateIntegerLiteral(ulong value) => new(CreateFreeformToken(TokenType.Integer, value.ToString()), value);

        public static UnaryOperationSyntax CreateNegativeIntegerLiteral(ulong value) => new(MinusToken, CreateIntegerLiteral(value));

        public static ExpressionSyntax CreatePositiveOrNegativeInteger(long intValue)
        {
            if (intValue >= 0)
            {
                return SyntaxFactory.CreateIntegerLiteral((ulong)intValue);
            }
            else
            {
                return SyntaxFactory.CreateNegativeIntegerLiteral((ulong)-intValue);
            }
        }

        public static StringSyntax CreateMultilineString(string value, IEnumerable<SyntaxTrivia>? leadingTrivia = null, IEnumerable<SyntaxTrivia>? trailingTrivia = null)
        {
            // It's the responsibility of the caller to have already verified this, to avoid throwing here.
            if (value.Contains("'''"))
            {
                throw new ArgumentException("The value must not contain the sequence '''");
            }

            // there's intentionally no escaping for a multi-line string
            var tokenValue = $"'''{value}'''";

            return new(
                [CreateFreeformToken(TokenType.StringComplete, tokenValue, leadingTrivia, trailingTrivia)],
                [],
                [value]);
        }

        public static StringSyntax CreateStringLiteral(string value) => CreateString(value.AsEnumerable(), []);

        public static StringSyntax CreateStringLiteralWithTextSpan(string value)
            => new(new FreeformToken(TokenType.StringComplete, new TextSpan(0, value.Length), value, [], []).AsEnumerable(), [], [value]);

        public static BooleanLiteralSyntax CreateBooleanLiteral(bool value) => new(value ? TrueKeywordToken : FalseKeywordToken, value);

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

        private static StringSyntax CreateStringSyntaxWithComment(string value, string comment)
        {
            var trailingTrivia = new SyntaxTrivia(SyntaxTriviaType.MultiLineComment, TextSpan.Nil, $"/*{comment.Replace("*/", "*\\/")}*/");
            var stringToken = CreateFreeformToken(TokenType.StringComplete, value, EmptyTrivia, SyntaxFactory.SingleSpaceTrivia.Append(trailingTrivia));

            return new StringSyntax(stringToken.AsEnumerable(), [], value.AsEnumerable());
        }

        public static StringSyntax CreateStringLiteralWithComment(string value, string comment)
        {
            return CreateStringSyntaxWithComment($"'{EscapeBicepString(value)}'", comment);
        }

        public static StringSyntax CreateEmptySyntaxWithComment(string comment)
        {
            return CreateStringSyntaxWithComment("", comment);
        }

        public static StringSyntax CreateInvalidSyntaxWithComment(string comment)
        {
            return CreateStringSyntaxWithComment("?", comment);
        }

        public static Token CreateStringLiteralToken(string value)
        {
            return CreateFreeformToken(TokenType.StringComplete, $"'{EscapeBicepString(value)}'");
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
            return (isStart, isEnd) switch
            {
                (true, true) => CreateFreeformToken(TokenType.StringComplete, $"'{EscapeBicepString(value)}'"),
                (true, false) => CreateFreeformToken(TokenType.StringLeftPiece, $"'{EscapeBicepString(value)}${{"),
                (false, false) => CreateFreeformToken(TokenType.StringMiddlePiece, $"}}{EscapeBicepString(value)}${{"),
                (false, true) => CreateFreeformToken(TokenType.StringRightPiece, $"}}{EscapeBicepString(value)}'"),
            };
        }

        public static FunctionCallSyntax CreateFunctionCall(string functionName, params SyntaxBase[] argumentExpressions)
            => new(
                CreateIdentifier(functionName),
                LeftParenToken,
                Interleave(argumentExpressions.Select(x => new FunctionArgumentSyntax(x)), () => CommaToken),
                RightParenToken);

        public static InstanceFunctionCallSyntax CreateInstanceFunctionCall(SyntaxBase baseSyntax, string functionName, params SyntaxBase[] argumentExpressions)
            => new(
                baseSyntax,
                DotToken,
                CreateIdentifier(functionName),
                LeftParenToken,
                Interleave(argumentExpressions.Select(x => new FunctionArgumentSyntax(x)), () => CommaToken),
                RightParenToken);

        public static DecoratorSyntax CreateDecorator(string functionName, params SyntaxBase[] argumentExpressions)
            => new(AtToken, CreateFunctionCall(functionName, argumentExpressions));

        private static IEnumerable<SyntaxBase> Interleave(IEnumerable<SyntaxBase> elements, Func<SyntaxBase> getInterleaveSyntax)
            => elements.SelectMany((x, i) => i > 0 ? getInterleaveSyntax().AsEnumerable().Concat(x) : x.AsEnumerable());

        private static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");

        public static Token CreateNewLineWithIndent(string indent) => GetNewlineToken(
            trailingTrivia: new SyntaxTrivia(SyntaxTriviaType.Whitespace, TextSpan.Nil, indent).AsEnumerable());

        public static LambdaSyntax CreateLambdaSyntax(IReadOnlyList<string> parameterNames, SyntaxBase body)
        {
            SyntaxBase variableBlock = parameterNames.Count switch
            {
                1 => new LocalVariableSyntax(CreateIdentifier(parameterNames[0])),
                _ => new VariableBlockSyntax(
                    LeftParenToken,
                    Interleave(
                        parameterNames.Select(name => new LocalVariableSyntax(CreateIdentifier(name))),
                        () => CommaToken),
                    RightParenToken),
            };

            return new(variableBlock, ArrowToken, [], body);
        }

        public static TypedLambdaSyntax CreateTypedLambdaSyntax(IEnumerable<(string name, SyntaxBase type)> parameters, SyntaxBase returnType, SyntaxBase body)
        {
            TypedVariableBlockSyntax variableBlock = new(
                LeftParenToken,
                Interleave(
                    parameters.Select(parameter => new TypedLocalVariableSyntax(CreateIdentifierWithTrailingSpace(parameter.name), parameter.type)),
                    () => CommaToken),
                RightParenToken);

            return new(variableBlock, returnType, ArrowToken, [], body);
        }

        public static NonNullAssertionSyntax AsNonNullable(SyntaxBase @base) => @base switch
        {
            NonNullAssertionSyntax alreadyNonNull => alreadyNonNull,
            _ => new NonNullAssertionSyntax(@base, ExclamationToken),
        };

        public static AccessExpressionSyntax CreateAccessSyntax(SyntaxBase @base, string propertyName)
            => CreateAccessSyntax(@base, false, propertyName);

        public static AccessExpressionSyntax CreateAccessSyntax(SyntaxBase @base, bool safe, string propertyName)
            => Lexer.IsValidIdentifier(propertyName) ?
                new PropertyAccessSyntax(@base, DotToken, safe ? QuestionToken : null, CreateIdentifier(propertyName)) :
                CreateArrayAccess(@base, safe, false, CreateStringLiteral(propertyName));

        private static ArrayAccessSyntax CreateArrayAccess(SyntaxBase @base, bool safe, bool fromEnd, SyntaxBase accessExpression)
            => new(@base, LeftSquareToken, safe ? QuestionToken : null, fromEnd ? HatToken : null, accessExpression, RightSquareToken);

        public static AccessExpressionSyntax CreateSafeAccess(SyntaxBase @base, SyntaxBase accessExpression)
            => (accessExpression is StringSyntax stringAccess && stringAccess.TryGetLiteralValue() is { } stringValue) ?
                CreateAccessSyntax(@base, true, stringValue) :
                CreateArrayAccess(@base, true, false, accessExpression);

        public static ParameterAssignmentSyntax CreateParameterAssignmentSyntax(string name, SyntaxBase value)
            => new(
                [],
                ParameterKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                AssignmentToken,
                value);

        public static ExtensionConfigAssignmentSyntax CreateExtensionConfigAssignmentSyntax(string name, ObjectSyntax value)
            => new(
                [],
                ExtensionConfigKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                new ExtensionWithClauseSyntax(WithKeywordToken, value));

        public static BinaryOperationSyntax CreateBinaryOperationSyntax(SyntaxBase left, TokenType operatorType, SyntaxBase right)
            => new(
                left,
                CreateToken(operatorType, SingleSpaceTrivia, SingleSpaceTrivia),
                right);

        public static ParenthesizedExpressionSyntax CreateParenthesized(SyntaxBase inner)
            => new(LeftParenToken, inner, RightParenToken);

        public static VariableDeclarationSyntax CreateVariableDeclaration(string name, SyntaxBase value, SyntaxBase? type = null, IEnumerable<SyntaxBase>? leadingNodes = null)
            => new(
                leadingNodes ?? [],
                VariableKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                type,
                AssignmentToken,
                value);

        public static FunctionDeclarationSyntax CreateFunctionDeclaration(string name, IEnumerable<(string name, SyntaxBase type)> parameters, SyntaxBase outputType, SyntaxBase body, IEnumerable<SyntaxBase>? leadingNodes = null)
            => new(
                leadingNodes ?? [],
                FunctionKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                CreateTypedLambdaSyntax(parameters, outputType, body));

        public static ParameterDeclarationSyntax CreateParameterDeclaration(string name, SyntaxBase type, SyntaxBase? defaultValue = null, IEnumerable<SyntaxBase>? leadingNodes = null)
            => new(
                leadingNodes ?? [],
                ParameterKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                type,
                defaultValue is { } ? new ParameterDefaultValueSyntax(AssignmentToken, defaultValue) : null);

        public static TypeDeclarationSyntax CreateTypeDeclaration(string name, SyntaxBase typeValue, IEnumerable<SyntaxBase>? leadingNodes = null)
            => new(
                leadingNodes ?? [],
                TypeKeywordToken,
                CreateIdentifierWithTrailingSpace(name),
                AssignmentToken,
                typeValue);
    }
}
