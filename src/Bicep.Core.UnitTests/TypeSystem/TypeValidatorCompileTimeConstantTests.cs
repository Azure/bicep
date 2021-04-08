// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Parsing;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class TypeValidatorCompileTimeConstantTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetLiteralExpressionData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void CompileTimeConstantExpressionShouldReturnNoViolations(string displayName, SyntaxBase expression)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            TypeValidator.GetCompileTimeConstantViolation(expression, diagnosticWriter);

            diagnosticWriter.GetDiagnostics().Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetNonLiteralExpressionData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void NonLiteralExpression_IsLiteralExpression_ShouldReturnViolations(string displayName, SyntaxBase expression)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            TypeValidator.GetCompileTimeConstantViolation(expression, diagnosticWriter);

            diagnosticWriter.GetDiagnostics().Should().NotBeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetNonExpressionData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void NonExpressionShouldProduceNoViolations(string displayName, SyntaxBase expression)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            TypeValidator.GetCompileTimeConstantViolation(expression, diagnosticWriter);

            diagnosticWriter.GetDiagnostics().Should().BeEmpty();
        }

        public static string GetDisplayName(MethodInfo method, object[] row)
        {
            row.Length.Should().Be(2);
            row[0].Should().BeOfType<string>();
            return $"{method.Name}_{row[0]}";
        }

        private static IEnumerable<object[]> GetNonLiteralExpressionData()
        {
            // local function
            object[] CreateTextRow(string text) => CreateRow(text, ParserHelper.ParseExpression(text));

            // variable access
            yield return CreateTextRow("x");

            // binary operation
            yield return CreateTextRow("x+2");
            yield return CreateTextRow("x%2");
            yield return CreateTextRow("2/'a'");
            yield return CreateTextRow("2-'a'");

            // unary
            yield return CreateTextRow("!true");
            yield return CreateTextRow("!x");
            yield return CreateTextRow("-x");
            yield return CreateTextRow("!true");

            // ternary
            yield return CreateTextRow("true?true:true");

            // parentheses
            yield return CreateTextRow("(true)");

            // function call
            yield return CreateTextRow("foo()");

            // member access
            yield return CreateTextRow("true.y");

            // array access
            yield return CreateTextRow("true[0]");

            // complex expression
            yield return CreateTextRow("true || false && null");
            yield return CreateTextRow("2 && 3 && 4 && 5");
            yield return CreateTextRow("2 + 3 * !4 - 10 % 2 - -1");
            yield return CreateTextRow("null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15");
        }

        private static IEnumerable<object[]> GetLiteralExpressionData()
        {
            // simple types
            yield return CreateRow("null", TestSyntaxFactory.CreateNull());
            yield return CreateRow("true", TestSyntaxFactory.CreateBool(true));
            yield return CreateRow("false", TestSyntaxFactory.CreateBool(false));
            yield return CreateRow("string", TestSyntaxFactory.CreateString("hello"));
            yield return CreateRow("int", TestSyntaxFactory.CreateInt(42));
            yield return CreateRow("negative int", TestSyntaxFactory.CreateUnaryMinus(TestSyntaxFactory.CreateInt(42)));

            yield return CreateRow("empty object", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]));

            yield return CreateRow("object literal", TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("one", TestSyntaxFactory.CreateNull()),
                TestSyntaxFactory.CreateProperty("two", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("three", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("four", TestSyntaxFactory.CreateString("hello")),
                TestSyntaxFactory.CreateProperty("five", TestSyntaxFactory.CreateInt(42)),
                TestSyntaxFactory.CreateProperty("six", TestSyntaxFactory.CreateObject(new []
                {
                    TestSyntaxFactory.CreateProperty("one", TestSyntaxFactory.CreateNull()),
                    TestSyntaxFactory.CreateProperty("two", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("three", TestSyntaxFactory.CreateBool(false)),
                    TestSyntaxFactory.CreateProperty("four", TestSyntaxFactory.CreateString("test")),
                    TestSyntaxFactory.CreateProperty("five", TestSyntaxFactory.CreateInt(100)),
                    TestSyntaxFactory.CreateProperty("six",TestSyntaxFactory.CreateArray(new SyntaxBase[]
                    {
                        TestSyntaxFactory.CreateNull(),
                        TestSyntaxFactory.CreateBool(true),
                        TestSyntaxFactory.CreateBool(false),
                        TestSyntaxFactory.CreateString("other"),
                        TestSyntaxFactory.CreateInt(103)
                    }))
                }))
            }));

            yield return CreateRow("empty array", TestSyntaxFactory.CreateArray(new ArrayItemSyntax[0]));

            yield return CreateRow("array literal", TestSyntaxFactory.CreateArray(new SyntaxBase[]
            {
                TestSyntaxFactory.CreateNull(),
                TestSyntaxFactory.CreateBool(true),
                TestSyntaxFactory.CreateBool(false),
                TestSyntaxFactory.CreateString("other"),
                TestSyntaxFactory.CreateInt(103),
                TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("one", TestSyntaxFactory.CreateNull()),
                    TestSyntaxFactory.CreateProperty("two", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("three", TestSyntaxFactory.CreateBool(false)),
                    TestSyntaxFactory.CreateProperty("four", TestSyntaxFactory.CreateString("test")),
                    TestSyntaxFactory.CreateProperty("five", TestSyntaxFactory.CreateInt(100)),
                    TestSyntaxFactory.CreateProperty("six", TestSyntaxFactory.CreateArray(new SyntaxBase[]
                    {
                        TestSyntaxFactory.CreateNull(),
                        TestSyntaxFactory.CreateBool(true),
                        TestSyntaxFactory.CreateBool(false),
                        TestSyntaxFactory.CreateString("other"),
                        TestSyntaxFactory.CreateInt(103)
                    }))
                }),
                TestSyntaxFactory.CreateArray(new SyntaxBase[]
                {
                    TestSyntaxFactory.CreateNull(),
                    TestSyntaxFactory.CreateBool(true),
                    TestSyntaxFactory.CreateBool(false),
                    TestSyntaxFactory.CreateString("other"),
                    TestSyntaxFactory.CreateInt(103)
                })
            }));
        }

        private static IEnumerable<object[]> GetNonExpressionData()
        {
            yield return CreateRow("param declaration", new Parser("param foo string").Program());

            yield return CreateRow("empty file", new Parser("").Program());
        }

        private static object[] CreateRow(string name, SyntaxBase expression) => new object[] { name, expression };
    }
}

