// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void AllUnaryOperatorsShouldValidText()
        {
            RunTextTest(Operators.UnaryOperatorToText);
        }

        [TestMethod]
        public void AllUnaryOperatorsShouldMapToTokens()
        {
            RunTokenTest(Operators.TokenTypeToUnaryOperator);
        }

        [TestMethod]
        public void AllBinaryOperatorsShouldMapToTokens()
        {
            RunTokenTest(Operators.TokenTypeToBinaryOperator);
        }

        private static void RunTextTest<TEnum>(IDictionary<TEnum, string> data) where TEnum : struct
        {
            foreach (TEnum @operator in GetValues<TEnum>())
            {
                data[@operator].Should().NotBeNullOrWhiteSpace($"because the {@operator} operator should have text");
            }

            var duplicatedTexts = data.Values
                .GroupBy(text => text)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToList();

            duplicatedTexts.Should().BeEmpty($"because operators {duplicatedTexts.ConcatString(", ")} should have unique text");
        }

        private static void RunTokenTest<TEnum>(IDictionary<TokenType, TEnum> data) where TEnum : struct
        {
            data.Values.Should().BeEquivalentTo(GetValues<TEnum>(), "because there should be a mapping from a token type to every operator");
        }

        private static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).OfType<TEnum>();
        }

        public record DiagnosticMatcherData(DiagnosticLevel Level, string Code, string Message);

        [DataTestMethod]
        [DynamicData(nameof(GetUnaryTestCases), DynamicDataSourceType.Method)]
        public void Unary_operator_resolves_correct_type(UnaryOperationSyntax expression, TypeSymbol operandType, TypeSymbol expected, IEnumerable<DiagnosticMatcherData> expectedDiagnostics)
        {
            var diagnosticsWriter = ToListDiagnosticWriter.Create();
            var actual = OperationReturnTypeEvaluator.TryFoldUnaryExpression(expression.Operator, operandType, diagnosticsWriter);
            actual.Should().Be(expected);

            if (diagnosticsWriter.GetDiagnostics().Any() || expectedDiagnostics.Any())
            {
                diagnosticsWriter.GetDiagnostics().Should().SatisfyRespectively(expectedDiagnostics.Select<DiagnosticMatcherData, Action<IDiagnostic>>(matcherData => x =>
                {
                    x.Level.Should().Be(matcherData.Level);
                    x.Code.Should().Be(matcherData.Code);
                    x.Message.Should().Be(matcherData.Message);
                }));
            }
        }

        private static IEnumerable<object[]> GetUnaryTestCases()
        {
            static object[] Case(UnaryOperationSyntax expression, TypeSymbol operandType, TypeSymbol expected, params DiagnosticMatcherData[] matcherData)
                => [expression, operandType, expected, matcherData];


            var symbolRef = TestSyntaxFactory.CreateVariableAccess("foo");
            UnaryOperationSyntax minus = new(TestSyntaxFactory.CreateToken(TokenType.Minus), symbolRef);
            UnaryOperationSyntax not = new(TestSyntaxFactory.CreateToken(TokenType.Exclamation), symbolRef);

            return new[]
            {
                Case(minus, LanguageConstants.Int, LanguageConstants.Int),
                Case(minus, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(-1)),
                Case(minus, TypeFactory.CreateIntegerType(-20, -10), TypeFactory.CreateIntegerType(10, 20)),
                Case(minus, TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerLiteralType(-100), TypeFactory.CreateIntegerType(10, 20)), TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerLiteralType(100), TypeFactory.CreateIntegerType(-20, -10))),
                Case(minus, TypeFactory.CreateIntegerLiteralType(long.MinValue), LanguageConstants.Int),
                Case(not, LanguageConstants.Bool, LanguageConstants.Bool),
                Case(not, TypeFactory.CreateBooleanLiteralType(true), TypeFactory.CreateBooleanLiteralType(false)),
                Case(not, TypeHelper.CreateTypeUnion(TypeFactory.CreateBooleanLiteralType(true), TypeFactory.CreateBooleanLiteralType(false)), TypeHelper.CreateTypeUnion(TypeFactory.CreateBooleanLiteralType(true), TypeFactory.CreateBooleanLiteralType(false))),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBinaryTestCases), DynamicDataSourceType.Method)]
        public void Binary_operator_resolves_correct_type(BinaryOperator @operator, TypeSymbol leftOperandType, TypeSymbol rightOperandType, TypeSymbol expected, IEnumerable<DiagnosticMatcherData> expectedDiagnostics)
        {
            var diagnosticsWriter = ToListDiagnosticWriter.Create();
            var actual = OperationReturnTypeEvaluator.TryFoldBinaryExpression(OperationSyntaxFor(@operator), leftOperandType, rightOperandType, diagnosticsWriter);
            actual.Should().Be(expected);

            if (diagnosticsWriter.GetDiagnostics().Any() || expectedDiagnostics.Any())
            {
                diagnosticsWriter.GetDiagnostics().Should().SatisfyRespectively(expectedDiagnostics.Select<DiagnosticMatcherData, Action<IDiagnostic>>(matcherData => x =>
                {
                    x.Level.Should().Be(matcherData.Level);
                    x.Code.Should().Be(matcherData.Code);
                    x.Message.Should().Be(matcherData.Message);
                }));
            }
        }

        [TestMethod]
        public void Modulo_by_zero_resolves_to_error()
        {
            var actual = OperationReturnTypeEvaluator.TryFoldBinaryExpression(
                OperationSyntaxFor(BinaryOperator.Modulo),
                LanguageConstants.Int,
                TypeFactory.CreateIntegerLiteralType(0),
                ToListDiagnosticWriter.Create());

            actual.Should().NotBeNull();
            actual!.TypeKind.Should().Be(TypeKind.Error);
            actual.GetDiagnostics().Should().SatisfyRespectively(
                d =>
                {
                    d.Level.Should().Be(DiagnosticLevel.Error);
                    d.Code.Should().Be("BCP410");
                    d.Message.Should().Be("Division by zero is not supported.");
                });
        }

        private static BinaryOperationSyntax OperationSyntaxFor(BinaryOperator @operator) => new(leftExpression: TestSyntaxFactory.CreateVariableAccess("foo"), rightExpression: TestSyntaxFactory.CreateVariableAccess("bar"), operatorToken: @operator switch
        {
            BinaryOperator.LogicalOr => TestSyntaxFactory.CreateToken(TokenType.LogicalOr),
            BinaryOperator.LogicalAnd => TestSyntaxFactory.CreateToken(TokenType.LogicalAnd),
            BinaryOperator.Equals => TestSyntaxFactory.CreateToken(TokenType.Equals),
            BinaryOperator.NotEquals => TestSyntaxFactory.CreateToken(TokenType.NotEquals),
            BinaryOperator.EqualsInsensitive => TestSyntaxFactory.CreateToken(TokenType.EqualsInsensitive),
            BinaryOperator.NotEqualsInsensitive => TestSyntaxFactory.CreateToken(TokenType.NotEqualsInsensitive),
            BinaryOperator.LessThan => TestSyntaxFactory.CreateToken(TokenType.LeftChevron),
            BinaryOperator.LessThanOrEqual => TestSyntaxFactory.CreateToken(TokenType.LessThanOrEqual),
            BinaryOperator.GreaterThan => TestSyntaxFactory.CreateToken(TokenType.RightChevron),
            BinaryOperator.GreaterThanOrEqual => TestSyntaxFactory.CreateToken(TokenType.GreaterThanOrEqual),
            BinaryOperator.Add => TestSyntaxFactory.CreateToken(TokenType.Plus),
            BinaryOperator.Subtract => TestSyntaxFactory.CreateToken(TokenType.Minus),
            BinaryOperator.Multiply => TestSyntaxFactory.CreateToken(TokenType.Asterisk),
            BinaryOperator.Divide => TestSyntaxFactory.CreateToken(TokenType.Slash),
            BinaryOperator.Modulo => TestSyntaxFactory.CreateToken(TokenType.Modulo),
            BinaryOperator.Coalesce => TestSyntaxFactory.CreateToken(TokenType.DoubleQuestion),
            _ => throw new InvalidOperationException("Unrecognized operator")
        });

        private static IEnumerable<object[]> GetBinaryTestCases()
        {
            static object[] Case(BinaryOperator @operator, TypeSymbol leftOperandType, TypeSymbol rightOperandType, TypeSymbol expected, params DiagnosticMatcherData[] matcherData)
                => [@operator, leftOperandType, rightOperandType, expected, matcherData];

            return new[]
            {
                // ||
                Case(BinaryOperator.LogicalOr, LanguageConstants.Bool, LanguageConstants.Bool, LanguageConstants.Bool),
                Case(BinaryOperator.LogicalOr, LanguageConstants.True, LanguageConstants.Any, LanguageConstants.True),
                Case(BinaryOperator.LogicalOr, LanguageConstants.Any, LanguageConstants.True, LanguageConstants.True),
                Case(BinaryOperator.LogicalOr, LanguageConstants.False, LanguageConstants.False, LanguageConstants.False),

                // &&
                Case(BinaryOperator.LogicalAnd, LanguageConstants.Bool, LanguageConstants.Bool, LanguageConstants.Bool),
                Case(BinaryOperator.LogicalAnd, LanguageConstants.False, LanguageConstants.Any, LanguageConstants.False),
                Case(BinaryOperator.LogicalAnd, LanguageConstants.Any, LanguageConstants.False, LanguageConstants.False),
                Case(BinaryOperator.LogicalAnd, LanguageConstants.True, LanguageConstants.True, LanguageConstants.True),

                // ==
                Case(BinaryOperator.Equals, LanguageConstants.Any, LanguageConstants.Any, LanguageConstants.Bool),
                Case(BinaryOperator.Equals, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("literal"), LanguageConstants.True),
                Case(BinaryOperator.Equals, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("LiTeRaL"), LanguageConstants.False),

                // !=
                Case(BinaryOperator.NotEquals, LanguageConstants.Any, LanguageConstants.Any, LanguageConstants.Bool),
                Case(BinaryOperator.NotEquals, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("literal"), LanguageConstants.False),
                Case(BinaryOperator.NotEquals, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("LiTeRaL"), LanguageConstants.True),

                // =~
                Case(BinaryOperator.EqualsInsensitive, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.EqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("literal"), LanguageConstants.True),
                Case(BinaryOperator.EqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("LiTeRaL"), LanguageConstants.True),
                Case(BinaryOperator.EqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("L1T3R4L"), LanguageConstants.False),

                // !~
                Case(BinaryOperator.NotEqualsInsensitive, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.NotEqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("literal"), LanguageConstants.False),
                Case(BinaryOperator.NotEqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("LiTeRaL"), LanguageConstants.False),
                Case(BinaryOperator.NotEqualsInsensitive, TypeFactory.CreateStringLiteralType("literal"), TypeFactory.CreateStringLiteralType("L1T3R4L"), LanguageConstants.True),

                // int < int
                Case(BinaryOperator.LessThan, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Bool),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2), LanguageConstants.True),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(2), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.False),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.False),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.True),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.False),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.False),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.True),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.False),
                Case(BinaryOperator.LessThan, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.False),

                // int <= int
                Case(BinaryOperator.LessThanOrEqual, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Bool),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(2), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.False),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 0), LanguageConstants.Bool),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.False),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 0), LanguageConstants.Bool),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.False),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.Bool),

                // int > int
                Case(BinaryOperator.GreaterThan, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(2), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.True),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 0), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.True),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.False),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 0), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.True),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.Bool),

                // int >= int
                Case(BinaryOperator.GreaterThanOrEqual, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(2), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerType(maxValue: 2), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 2), LanguageConstants.False),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerType(maxValue: 1), TypeFactory.CreateIntegerType(minValue: 1), LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 0), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 1), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateIntegerType(minValue: 1), TypeFactory.CreateIntegerType(maxValue: 2), LanguageConstants.Bool),

                // string < string
                Case(BinaryOperator.LessThan, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.LessThan, TypeFactory.CreateStringLiteralType("a"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.True),
                Case(BinaryOperator.LessThan, TypeFactory.CreateStringLiteralType("b"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.False),

                // string <= string
                Case(BinaryOperator.LessThanOrEqual, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateStringLiteralType("a"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateStringLiteralType("b"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.True),
                Case(BinaryOperator.LessThanOrEqual, TypeFactory.CreateStringLiteralType("c"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.False),

                // string > string
                Case(BinaryOperator.GreaterThan, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateStringLiteralType("b"), TypeFactory.CreateStringLiteralType("a"), LanguageConstants.True),
                Case(BinaryOperator.GreaterThan, TypeFactory.CreateStringLiteralType("a"), TypeFactory.CreateStringLiteralType("a"), LanguageConstants.False),

                // string >= string
                Case(BinaryOperator.GreaterThanOrEqual, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Bool),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateStringLiteralType("c"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateStringLiteralType("b"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.True),
                Case(BinaryOperator.GreaterThanOrEqual, TypeFactory.CreateStringLiteralType("a"), TypeFactory.CreateStringLiteralType("b"), LanguageConstants.False),

                // +
                Case(BinaryOperator.Add, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
                Case(BinaryOperator.Add, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(20)),
                Case(BinaryOperator.Add, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateIntegerType(20, 30)),
                Case(BinaryOperator.Add, TypeFactory.CreateIntegerType(0, 10), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateIntegerType(10, 30)),

                // -
                Case(BinaryOperator.Subtract, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
                Case(BinaryOperator.Subtract, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(0)),
                Case(BinaryOperator.Subtract, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateIntegerType(-10, 0)),
                Case(BinaryOperator.Subtract, TypeFactory.CreateIntegerType(0, 10), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateIntegerType(-20, 0)),

                // *
                Case(BinaryOperator.Multiply, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
                Case(BinaryOperator.Multiply, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(100)),
                Case(BinaryOperator.Multiply, TypeFactory.CreateIntegerLiteralType(long.MaxValue), TypeFactory.CreateIntegerLiteralType(long.MaxValue), LanguageConstants.Int,
                    new DiagnosticMatcherData(DiagnosticLevel.Warning, "BCP234",
                        "The ARM function \"mul\" failed when invoked on the value [9223372036854775807, 9223372036854775807]: The template language function 'mul' overflowed with the operants '9223372036854775807' and '9223372036854775807'. Please see https://aka.ms/arm-functions for usage details.")),

                // /
                Case(BinaryOperator.Divide, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
                Case(BinaryOperator.Divide, TypeFactory.CreateIntegerLiteralType(50), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateIntegerLiteralType(5)),

                // %
                Case(BinaryOperator.Modulo, LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
                Case(BinaryOperator.Modulo, TypeFactory.CreateIntegerLiteralType(4), TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerLiteralType(1)),
                Case(BinaryOperator.Modulo, LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerType(-2, 2)),
                Case(BinaryOperator.Modulo, TypeFactory.CreateIntegerType(0, 27), TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerType(0, 2)),
                Case(BinaryOperator.Modulo, LanguageConstants.Int, TypeFactory.CreateIntegerType(-28, 33), TypeFactory.CreateIntegerType(-32, 32)),
                Case(BinaryOperator.Modulo, TypeFactory.CreateIntegerType(0, 27), TypeFactory.CreateIntegerType(0, 3), TypeFactory.CreateIntegerType(0, 2)),

                // ??
                Case(BinaryOperator.Coalesce, LanguageConstants.Any, LanguageConstants.Any, LanguageConstants.Any),
                Case(BinaryOperator.Coalesce, LanguageConstants.String, LanguageConstants.Bool, LanguageConstants.String),
                Case(BinaryOperator.Coalesce, LanguageConstants.Null, LanguageConstants.Bool, LanguageConstants.Bool),
                Case(BinaryOperator.Coalesce,
                    TypeHelper.CreateTypeUnion(LanguageConstants.Null, TypeFactory.CreateStringLiteralType("Fizz"), TypeFactory.CreateStringLiteralType("Buzz")),
                    TypeFactory.CreateStringLiteralType("Pop"),
                    TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("Fizz"), TypeFactory.CreateStringLiteralType("Buzz"), TypeFactory.CreateStringLiteralType("Pop"))),
            };
        }
    }
}
