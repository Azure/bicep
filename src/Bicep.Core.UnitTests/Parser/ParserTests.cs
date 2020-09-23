// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parser
{
    [TestClass]
    public class ParserTests
    {
        [DataTestMethod]
        [DataRow("true", "true", typeof(BooleanLiteralSyntax))]
        [DataRow("false", "false", typeof(BooleanLiteralSyntax))]
        [DataRow("432", "432", typeof(NumericLiteralSyntax))]
        [DataRow("null", "null", typeof(NullLiteralSyntax))]
        [DataRow("'hello world!'", "'hello world!'", typeof(StringSyntax))]
        public void LiteralExpressionsShouldParseCorrectly(string text, string expected, Type expectedRootType)
        {
            RunExpressionTest(text, expected, expectedRootType);
        }

        [DataTestMethod]
        [DataRow("param myParam string", typeof(ParameterDeclarationSyntax))]
        [DataRow("var mvVar = 'hello'", typeof(VariableDeclarationSyntax))]
        [DataRow("resource myRes 'My.Provider/someResource@2020-08-01' = { \n }", typeof(ResourceDeclarationSyntax))]
        [DataRow("output string myOutput = 'hello'", typeof(OutputDeclarationSyntax))]
        public void NewLinesForDeclarationsShouldBeOptionalAtEof(string text, Type expectedType)
        {
            var validFiles = new (int statementCount, string file)[]
            {
                (1, text),
                (1, $"{text}\n"),
                (1, $"{text}\r\n"),
                (1, $"{text}\n\n"),
                (1, $"{text}\r\n\r\n"),
                (2, $"{text}\n{text}"),
                (2, $"{text}\n{text}\r\n"),
            };

            foreach (var (statementCount, file) in validFiles)
            {
                var becauseFileValid = $"{file} is considered valid";
                var program = ParserHelper.Parse(file, diags => diags.Should().BeEmpty(becauseFileValid));
                program.Declarations.Should().HaveCount(statementCount, becauseFileValid);
                program.Declarations.Should().AllBeOfType(expectedType, becauseFileValid);
            }

            var invalidFiles = new []
            {
                $"{text} {text}", // newline should be enforced between statements
            };

            foreach (var file in invalidFiles)
            {
                var program = ParserHelper.Parse(file, diags => diags.Should().NotBeEmpty());
            }
        }

        [DataTestMethod]
        [DataRow("'${abc}def'", "'${abc}def'")]
        [DataRow("'abc${def}'", "'abc${def}'")]
        [DataRow("'${abc}def${ghi}'", "'${abc}def${ghi}'")]
        [DataRow("'abc${def}ghi${klm}nop'", "'abc${def}ghi${klm}nop'")]
        [DataRow("'abc${1234}def'", "'abc${1234}def'")]
        [DataRow("'abc${true}def'", "'abc${true}def'")]
        // [DataRow("'abc${[]}def'", "'abc${[]}def'")] - currently unsupported because we force a newline between [ and ]
        // [DataRow("'abc${{}}def'", "'abc${{}}def'")] - currently unsupported because we force a newline between { and }
        public void StringInterpolationShouldParseCorrectly(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(StringSyntax));
        }

        [DataTestMethod]
        [DataRow("'${>}def'")]
        [DataRow("'${b+}def'")]
        [DataRow("'${concat(}def'")]
        [DataRow("'${concat)}def'")]
        [DataRow("'${'nest\\ed'}def'")]
        [DataRow("'${a b c}def'")]
        [DataRow("'abc${}'")]
        [DataRow("'def${>}'")]
        [DataRow("'abc${>}def${=}'")]
        [DataRow("'${>}def${=}abc'")]
        [DataRow("'${>}def${=}'")]
        public void Interpolation_with_bad_expressions_should_parse_successfully(string text)
        {
            var expression = ParseAndVerifyType<StringSyntax>(text);
            expression.Expressions.Should().Contain(x => x is SkippedTriviaSyntax);
        }

        [DataTestMethod]
        [DataRow("foo()", "foo()", 0)]
        [DataRow("bar(true)", "bar(true)", 1)]
        [DataRow("bar(true,1,'a',true,null)", "bar(true,1,'a',true,null)", 5)]
        [DataRow("test(2 + 3*4, true || false && null)", "test((2+(3*4)),(true||(false&&null)))", 2)]
        public void FunctionsShouldParseCorrectly(string text, string expected, int expectedArgumentCount)
        {
            var expression = (FunctionCallSyntax) RunExpressionTest(text, expected, typeof(FunctionCallSyntax));
            expression.Arguments.Length.Should().Be(expectedArgumentCount);
        }

        [DataTestMethod]
        [DataRow("foo","foo")]
        [DataRow("bar", "bar")]
        public void VariablesShouldParseCorrectly(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(VariableAccessSyntax));
        }

        [DataTestMethod]
        [DataRow("-10","(-10)")]
        [DataRow("!x", "(!x)")]
        public void UnaryOperationsShouldParseCorrectly(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(UnaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("!!true")]
        [DataRow("--10")]
        [DataRow("-!null")]
        public void UnaryOperatorsCannotBeChained(string text)
        {
            Action fail = () => ParserHelper.ParseExpression(text);
            fail.Should().Throw<ExpectedTokenException>().WithMessage("Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");
        }

        [DataTestMethod]
        [DataRow("2 + 3 * 4", "(2+(3*4))")]
        [DataRow("3 * 4 + 7", "((3*4)+7)")]
        [DataRow("2 + 3 * 4 - 10 % 2 - 1", "(((2+(3*4))-(10%2))-1)")]
        [DataRow("true || false && null","(true||(false&&null))")]
        [DataRow("false && true =~ 'aaa' || null !~ 1","((false&&(true=~'aaa'))||(null!~1))")]
        public void BinaryOperationsShouldHaveCorrectPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("2 + 3 + 4 -10", "(((2+3)+4)-10)")]
        [DataRow("2 * 3 / 5 % 100", "(((2*3)/5)%100)")]
        [DataRow("2 && 3 && 4 && 5","(((2&&3)&&4)&&5)")]
        [DataRow("true || null || 'a' || 'b'","(((true||null)||'a')||'b')")]
        [DataRow("true == false != null == 4 != 'a'","((((true==false)!=null)==4)!='a')")]
        [DataRow("x < y >= z > a","(((x<y)>=z)>a)")]
        [DataRow("a == b !~ c =~ d != e", "((((a==b)!~c)=~d)!=e)")]
        public void BinaryOperationsWithEqualPrecedenceShouldBeLeftToRightAssociative(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("2 + !null * 4","(2+((!null)*4))")]
        [DataRow("-2 +-3 + -4 -10", "((((-2)+(-3))+(-4))-10)")]
        [DataRow("2 + 3 * !4 - 10 % 2 - -1", "(((2+(3*(!4)))-(10%2))-(-1))")]
        [DataRow("-2 && 3 && !4 && 5", "((((-2)&&3)&&(!4))&&5)")]
        public void UnaryOperatorsShouldHavePrecedenceOverBinaryOperators(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("null ? 4: false","(null?4:false)")]
        public void TernaryOperatorShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(TernaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("null && !false ? 2+3*-8 : !13 < 10","((null&&(!false))?(2+(3*(-8))):((!13)<10))")]
        [DataRow("true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null", "(((((true==false)!=null)==4)!='a')?((((-2)&&3)&&(!4))&&5):(true||(false&&null)))")]
        [DataRow("null ? 1 : 2 + 3", "(null?1:(2+3))")]
        public void TernaryOperatorShouldHaveLowestPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(TernaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("(true)", "(true)")]
        [DataRow("(false)", "(false)")]
        [DataRow("(null)", "(null)")]
        [DataRow("(42)", "(42)")]
        [DataRow("('a${b}c${d}e')", "('a${b}c${d}e')")]
        public void ParenthesizedExpressionShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(ParenthesizedExpressionSyntax));
        }

        [DataTestMethod]
        [DataRow("(2+3)*4","(((2+3))*4)")]
        [DataRow("true && (false || null)", "(true&&((false||null)))")]
        [DataRow("(null ? 1 : 2) + 3", "(((null?1:2))+3)")]
        public void ParenthesizedExpressionsShouldHaveHighestPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15", "(null?1:(2?(true?'a':'b'):(false?'d':15)))")]
        public void TernaryOperatorShouldBeRightToLeftAssociative(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(TernaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a.b","(a.b)")]
        [DataRow("null.fail", "(null.fail)")]
        [DataRow("foo().bar","(foo().bar)")]
        [DataRow("a.b.c.foo().bar", "(((a.b).c).foo().bar)")]
        public void PropertyAccessShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(PropertyAccessSyntax));
        }

        [DataTestMethod]
        [DataRow("a.b.c.foo()", "((a.b).c).foo()")]
        [DataRow("a.b.c.d.e.f.g.foo()", "((((((a.b).c).d).e).f).g).foo()")]
        public void InstanceFunctionCallShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(InstanceFunctionCallSyntax));
        }

        [DataTestMethod]
        [DataRow("a.b.c + 0","(((a.b).c)+0)")]
        [DataRow("(a.b[c]).c[d]+q()", "((((((a.b)[c])).c)[d])+q())")]
        public void MemberAccessShouldBeLeftToRightAssociative(string text, string expected)
        {
            // this also asserts that (), [], and . have equal precedence
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a + b.c * z[12].a && q[foo()] == c.a", "((a+((b.c)*((z[12]).a)))&&((q[foo()])==(c.a)))")]
        public void MemberAccessShouldHaveHighestPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a[b]","(a[b])")]
        [DataRow("1[b]", "(1[b])")]
        [DataRow("a[12]", "(a[12])")]
        [DataRow("null[foo()]", "(null[foo()])")]
        [DataRow("foo()[bar()]", "(foo()[bar()])")]
        [DataRow("a.b.c.foo()[bar()]", "(((a.b).c).foo()[bar()])")]
        public void ArrayAccessShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(ArrayAccessSyntax));
        }

        private static SyntaxBase RunExpressionTest(string text, string expected, Type expectedRootType)
        {
            SyntaxBase expression = ParserHelper.ParseExpression(text);
            expression.Should().BeOfType(expectedRootType);
            SerializeExpressionWithExtraParentheses(expression).Should().Be(expected);

            return expression;
        }

        public static TSyntax ParseAndVerifyType<TSyntax>(string text)
            where TSyntax : SyntaxBase
        {
            var expression = ParserHelper.ParseExpression(text);
            expression.Should().BeOfType<TSyntax>();

            return (TSyntax)expression;
        }

        private static string SerializeExpressionWithExtraParentheses(SyntaxBase expression)
        {
            var buffer = new StringBuilder();
            var visitor = new ExpressionTestVisitor(buffer);

            visitor.Visit(expression);

            return buffer.ToString();
        }
    }
}

