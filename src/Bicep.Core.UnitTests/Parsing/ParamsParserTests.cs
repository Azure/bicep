// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Text;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing
{
    [TestClass]
    public class ParamsParserTests
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

        [TestMethod]
        public void TestDeclaringParams()
        {
            var paramIntTest = ParamsParserHelper.ParamsParse("param myint = 12 \n");
            var paramStringTest = ParamsParserHelper.ParamsParse("param mystr = 'hello world' \n");
            
            paramIntTest.Should().BeOfType(typeof(ProgramSyntax));
            paramStringTest.Should().BeOfType(typeof(ProgramSyntax));

            paramIntTest.Children.OfType<ParameterAssignmentSyntax>().Should().HaveCount(1);
            paramStringTest.Children.OfType<ParameterAssignmentSyntax>().Should().HaveCount(1);
        }

        [TestMethod]
        public void TestUsingKeyword()
        {
            var usingTest = ParamsParserHelper.ParamsParse("using './bicep.main' \n");

            usingTest.Should().BeOfType(typeof(ProgramSyntax));

            usingTest.Children.OfType<UsingDeclarationSyntax>().Should().HaveCount(1);
        }


        private static SyntaxBase RunExpressionTest(string text, string expected, Type expectedRootType)
        {
            SyntaxBase expression = ParserHelper.ParseExpression(text);
            expression.Should().BeOfType(expectedRootType);
            SerializeExpressionWithExtraParentheses(expression).Should().Be(expected);

            return expression;
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
