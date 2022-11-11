// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Text;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing
{
    [TestClass]
    public class ParserTests
    {
        [DataTestMethod]
        [DataRow("true", "true", typeof(BooleanLiteralSyntax))]
        [DataRow("false", "false", typeof(BooleanLiteralSyntax))]
        [DataRow("432", "432", typeof(IntegerLiteralSyntax))]
        [DataRow("1125899906842624", "1125899906842624", typeof(IntegerLiteralSyntax))]
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
        [DataRow("type arraysOfArraysOfArraysOfStrings = string[][][]", typeof(TypeDeclarationSyntax))]
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
                var program = ParserHelper.Parse(file);
                program.GetParseDiagnostics().Should().BeEmpty(becauseFileValid);
                program.Declarations.Should().HaveCount(statementCount, becauseFileValid);
                program.Declarations.Should().AllBeOfType(expectedType, becauseFileValid);
            }

            var invalidFiles = new[]
            {
                $"{text} {text}", // newline should be enforced between statements
            };

            foreach (var file in invalidFiles)
            {
                var program = ParserHelper.Parse(file);
                program.GetParseDiagnostics().Should().NotBeEmpty();
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
        // empty
        [DataRow("''''''", "")]
        [DataRow("'''\r\n'''", "")]
        [DataRow("'''\n'''", "")]
        // basic
        [DataRow("'''abc'''", "abc")]
        // first preceding newline should be stripped
        [DataRow("'''\r\nabc'''", "abc")]
        [DataRow("'''\nabc'''", "abc")]
        [DataRow("'''\rabc'''", "abc")]
        // only the first should be stripped!
        [DataRow("'''\n\nabc'''", "\nabc")]
        [DataRow("'''\n\rabc'''", "\rabc")]
        // no escaping necessary
        [DataRow("''' \n \r \t \\ ' ${ } '''", " \n \r \t \\ ' ${ } ")]
        // leading and terminating ' characters
        [DataRow("''''a''''", "'a'")]
        public void Multiline_strings_should_parse_correctly(string text, string expectedValue)
        {
            var stringSyntax = ParseAndVerifyType<StringSyntax>(text);

            stringSyntax.TryGetLiteralValue().Should().Be(expectedValue);
        }

        [DataTestMethod]
        [DataRow("'${>}def'")]
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
            expression.Expressions.Should().Contain(x => x is SkippedTriviaSyntax || x is BinaryOperationSyntax);
        }

        [DataTestMethod]
        [DataRow("'${!}def'")]
        [DataRow("'${ -}def'")]
        [DataRow("'${b+}def'")]
        [DataRow("'${b + (d /}def'")]
        [DataRow("'${true ? }def'")]
        [DataRow("'${true ? false }def'")]
        [DataRow("'${true ? : }def'")]
        [DataRow("'${true ? : null}def'")]
        public void Interpolation_with_incomplete_expressions_should_parse_successfully(string text)
        {
            var expression = ParseAndVerifyType<StringSyntax>(text);
            expression.Expressions.Should().Contain(x => x is UnaryOperationSyntax || x is BinaryOperationSyntax || x is TernaryOperationSyntax);
        }

        [DataTestMethod]
        [DataRow("foo()", "foo()", 0)]
        [DataRow("bar(true)", "bar(true)", 1)]
        [DataRow("bar(true,1,'a',true,null)", "bar(true,1,'a',true,null)", 5)]
        [DataRow("test(2 + 3*4, true || false && null)", "test((2+(3*4)),(true||(false&&null)))", 2)]
        public void FunctionsShouldParseCorrectly(string text, string expected, int expectedArgumentCount)
        {
            var expression = (FunctionCallSyntax)RunExpressionTest(text, expected, typeof(FunctionCallSyntax));
            expression.Arguments.Count().Should().Be(expectedArgumentCount);
        }

        [DataTestMethod]
        [DataRow("foo", "foo")]
        [DataRow("bar", "bar")]
        public void VariablesShouldParseCorrectly(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(VariableAccessSyntax));
        }

        [DataTestMethod]
        [DataRow("-10", "(-10)")]
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
            var expression = ParseAndVerifyType<UnaryOperationSyntax>(text);
            expression.Expression.Should().BeOfType<SkippedTriviaSyntax>()
                .Which.Diagnostics.Single().Message.Should().Be("Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");
        }

        [DataTestMethod]
        [DataRow("2 + 3 * 4", "(2+(3*4))")]
        [DataRow("3 * 4 + 7", "((3*4)+7)")]
        [DataRow("2 + 3 * 4 - 10 % 2 - 1", "(((2+(3*4))-(10%2))-1)")]
        [DataRow("true || false && null", "(true||(false&&null))")]
        [DataRow("false && true =~ 'aaa' || null !~ 1", "((false&&(true=~'aaa'))||(null!~1))")]
        public void BinaryOperationsShouldHaveCorrectPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("2 + 3 + 4 -10", "(((2+3)+4)-10)")]
        [DataRow("2 * 3 / 5 % 100", "(((2*3)/5)%100)")]
        [DataRow("2 && 3 && 4 && 5", "(((2&&3)&&4)&&5)")]
        [DataRow("true || null || 'a' || 'b'", "(((true||null)||'a')||'b')")]
        [DataRow("true == false != null == 4 != 'a'", "((((true==false)!=null)==4)!='a')")]
        [DataRow("x < y >= z > a", "(((x<y)>=z)>a)")]
        [DataRow("a == b !~ c =~ d != e", "((((a==b)!~c)=~d)!=e)")]
        public void BinaryOperationsWithEqualPrecedenceShouldBeLeftToRightAssociative(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("2 + !null * 4", "(2+((!null)*4))")]
        [DataRow("-2 +-3 + -4 -10", "((((-2)+(-3))+(-4))-10)")]
        [DataRow("2 + 3 * !4 - 10 % 2 - -1", "(((2+(3*(!4)))-(10%2))-(-1))")]
        [DataRow("-2 && 3 && !4 && 5", "((((-2)&&3)&&(!4))&&5)")]
        public void UnaryOperatorsShouldHavePrecedenceOverBinaryOperators(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("null ? 4: false", "(null?4:false)")]
        public void TernaryOperatorShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(TernaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("null && !false ? 2+3*-8 : !13 < 10", "((null&&(!false))?(2+(3*(-8))):((!13)<10))")]
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
        [DataRow("(2+3)*4", "(((2+3))*4)")]
        [DataRow("true && (false || null)", "(true&&((false||null)))")]
        [DataRow("(null ? 1 : 2) + 3", "(((null?1:2))+3)")]
        [DataRow("null ?? (b ?? c) ?? a", "((null??((b??c)))??a)")]
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
        [DataRow("a.b", "(a.b)")]
        [DataRow("null.fail", "(null.fail)")]
        [DataRow("foo().bar", "(foo().bar)")]
        [DataRow("a.b.c.foo().bar", "(((a.b).c).foo().bar)")]
        public void PropertyAccessShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(PropertyAccessSyntax));
        }

        [DataTestMethod]
        [DataRow("a::b", "(a::b)")]
        [DataRow("null::fail", "(null::fail)")]
        [DataRow("foo()::bar", "(foo()::bar)")]
        public void ResourceAccessShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(ResourceAccessSyntax));
        }

        [DataTestMethod]
        [DataRow("foo?bar:baz::biz", "(foo?bar:(baz::biz))")]
        [DataRow("foo?bar::biz.prop1:baz::boo", "(foo?((bar::biz).prop1):(baz::boo))")]
        [DataRow("foo::boo?bar:baz", "((foo::boo)?bar:baz)")]
        public void ResourceAccessShouldParseSuccessfullyWithTernaries(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(TernaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a.b.c.foo()", "((a.b).c).foo()")]
        [DataRow("a.b.c.d.e.f.g.foo()", "((((((a.b).c).d).e).f).g).foo()")]
        [DataRow("a::b::c.d::e::f::g.foo()", "((((((a::b)::c).d)::e)::f)::g).foo()")]
        public void InstanceFunctionCallShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(InstanceFunctionCallSyntax));
        }

        [DataTestMethod]
        [DataRow("a.b.c + 0", "(((a.b).c)+0)")]
        [DataRow("(a.b[c]).c[d]+q()", "((((((a.b)[c])).c)[d])+q())")]
        public void MemberAccessShouldBeLeftToRightAssociative(string text, string expected)
        {
            // this also asserts that (), [], and . have equal precedence
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a::b::c + 0", "(((a::b)::c)+0)")]
        [DataRow("(a::b[c])::c[d]+q()", "((((((a::b)[c]))::c)[d])+q())")]
        public void ResourceAccessShouldBeLeftToRightAssociative(string text, string expected)
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
        [DataRow("a + b::c * z[12]::a && q[foo()] == c::a", "((a+((b::c)*((z[12])::a)))&&((q[foo()])==(c::a)))")]
        public void ResourceAccessShouldHaveHighestPrecedence(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [DataTestMethod]
        [DataRow("a[b]", "(a[b])")]
        [DataRow("1[b]", "(1[b])")]
        [DataRow("a[12]", "(a[12])")]
        [DataRow("null[foo()]", "(null[foo()])")]
        [DataRow("foo()[bar()]", "(foo()[bar()])")]
        [DataRow("a.b.c.foo()[bar()]", "(((a.b).c).foo()[bar()])")]
        public void ArrayAccessShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(ArrayAccessSyntax));
        }

        [DataTestMethod]
        // 3 alternative ways to produce the same character
        [DataRow(@"'𐐷'", @"'𐐷'", @"𐐷")]
        [DataRow(@"'\u{10437}'", @"'\u{10437}'", @"𐐷")]
        [DataRow(@"'\u{D801}\u{DC37}'", @"'\u{D801}\u{DC37}'", @"𐐷")]
        // simple ascii escape
        [DataRow(@"'\u{20}'", @"'\u{20}'", " ")]
        [DataRow(@"'Hello\u{20}World! ☕'", @"'Hello\u{20}World! ☕'", @"Hello World! ☕")]
        public void UnicodeEscapesShouldProduceExpectedCharacters(string text, string expectedSerialized, string expectedLiteralValue)
        {
            var syntax = (StringSyntax)RunExpressionTest(text, expectedSerialized, typeof(StringSyntax));
            var value = syntax.TryGetLiteralValue();
            value.Should().NotBeNull();
            value.Should().Be(expectedLiteralValue);
        }

        [DataTestMethod]
        [DataRow("a ?? b", "(a??b)")]
        [DataRow("a ?? b ?? c", "((a??b)??c)")]
        [DataRow("a ?? b || d ?? c", "((a??(b||d))??c)")]
        [DataRow("foo() ?? bar().v ?? null", "((foo()??(bar().v))??null)")]
        public void CoalesceShouldParseSuccessfully(string text, string expected)
        {
            RunExpressionTest(text, expected, typeof(BinaryOperationSyntax));
        }

        [TestMethod]
        public void ObjectTypeLiteralsShouldParseSuccessfully()
        {
            var typeDeclaration = @"
@description('The foo type')
@sealed()
type foo = {
    @minLength(3)
    @maxLength(10)
    stringProp: string

    objectProp: {
        @minValue(1)
        intProp: int
        arrayProp: int[][][]
    }

    unionProp: 'several'|'string'|'literals'
}";

            var parsed = ParserHelper.Parse(typeDeclaration);
            parsed.Should().BeOfType<ProgramSyntax>();
            (parsed as ProgramSyntax).Declarations.Should().HaveCount(1);
            (parsed as ProgramSyntax).Declarations.Single().Should().BeOfType<TypeDeclarationSyntax>();
            var declaration = (TypeDeclarationSyntax) (parsed as ProgramSyntax).Declarations.Single();
            declaration.Decorators.Should().HaveCount(2);

            declaration.Value.Should().BeOfType<ObjectTypeSyntax>();
            var declaredObject = (ObjectTypeSyntax) declaration.Value;
            declaredObject.Properties.Should().HaveCount(3);
            declaredObject.Properties.First().Decorators.Should().HaveCount(2);
            declaredObject.Properties.First().Value.Should().BeOfType<VariableAccessSyntax>();
            declaredObject.Properties.Skip(1).First().Value.Should().BeOfType<ObjectTypeSyntax>();

            var objectProp = (ObjectTypeSyntax) declaredObject.Properties.Skip(1).First().Value;
            objectProp.Properties.Should().HaveCount(2);
            objectProp.Properties.Last().Value.Should().BeOfType<ArrayTypeSyntax>();

            var arrayProp = (ArrayTypeSyntax) objectProp.Properties.Last().Value;
            arrayProp.Item.Value.Should().BeOfType<ArrayTypeSyntax>();
            var intermediateArray = (ArrayTypeSyntax) arrayProp.Item.Value;
            intermediateArray.Item.Value.Should().BeOfType<ArrayTypeSyntax>();
            var innerArray = (ArrayTypeSyntax) intermediateArray.Item.Value;
            innerArray.Item.Value.Should().BeOfType<VariableAccessSyntax>();
        }

        [TestMethod]
        public void TupleTypeLiteralsShouldParseSuccessfully()
        {
            var typeDeclaration = @"
type aTuple = [
    @description('First element')
    @minLength(10)
    string

    @description('Second element')
    -37|0|37
]";

            var parsed = ParserHelper.Parse(typeDeclaration);
            parsed.Should().BeOfType<ProgramSyntax>();
            (parsed as ProgramSyntax).Declarations.Should().HaveCount(1);
            (parsed as ProgramSyntax).Declarations.Single().Should().BeOfType<TypeDeclarationSyntax>();
            var declaration = (TypeDeclarationSyntax) (parsed as ProgramSyntax).Declarations.Single();

            declaration.Value.Should().BeOfType<TupleTypeSyntax>();
            var declaredObject = (TupleTypeSyntax) declaration.Value;
            declaredObject.Items.Should().HaveCount(2);
            declaredObject.Items.First().Decorators.Should().HaveCount(2);
            declaredObject.Items.First().Value.Should().BeOfType<VariableAccessSyntax>();
            declaredObject.Items.Last().Decorators.Should().HaveCount(1);
            declaredObject.Items.Last().Value.Should().BeOfType<UnionTypeSyntax>();
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
