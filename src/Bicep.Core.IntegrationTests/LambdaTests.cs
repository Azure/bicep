// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class LambdaTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void Lambdas_cannot_be_used_with_feature_disabled()
        {
            var features = BicepTestConstants.Features with { LambdasEnabled = false };
            var context = new CompilationHelper.CompilationHelperContext(Features: features);

            CompilationHelper.Compile(context, "var foo = map([123], i => i)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP082", DiagnosticLevel.Error, "The name \"map\" does not exist in the current context. Did you mean \"max\"?"),
                });

            CompilationHelper.Compile(context, "var foo = filter([123], i => true)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP057", DiagnosticLevel.Error, "The name \"filter\" does not exist in the current context."),
                });

            CompilationHelper.Compile(context, "var foo = sort([123], (x, y) => x < y)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP057", DiagnosticLevel.Error, "The name \"sort\" does not exist in the current context."),
                });

            CompilationHelper.Compile(context, "var foo = reduce([123], 1, (x, y) => x + y)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP057", DiagnosticLevel.Error, "The name \"reduce\" does not exist in the current context."),
                });

            CompilationHelper.Compile(context, "var foo = flatten([123], i => i)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP057", DiagnosticLevel.Error, "The name \"flatten\" does not exist in the current context."),
                });
        }

        [TestMethod]
        public void Map_function_preserves_types_accurately_integers()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map([123], ab|c => abc)
var fo|o2 = map([123], a|bc => 'Hello ${abc}')
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("int[]"),
                x => x.Type.Name.Should().Be("int"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("int"));
        }

        [TestMethod]
        public void Map_function_preserves_types_accurately_strings()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map(['abc', 'def'], ab|c => abc)
var fo|o2 = map(['abc', 'def'], a|bc => length(abc))
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("string"),
                x => x.Type.Name.Should().Be("int[]"),
                x => x.Type.Name.Should().Be("string"));
        }

        [TestMethod]
        public void Map_function_works_with_unknowable_types()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map([], ab|c => abc)
var fo|o2 = map([any('foo')], a|bc => 'Hi ${abc}!')
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("any[]"),
                x => x.Type.Name.Should().Be("any"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("any"));
        }

        [TestMethod]
        public void Map_function_blocks_incorrect_args()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var foo = map([123], (abc, def) => abc)
var foo2 = map(['foo'], () => 'Hi!')
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP070", DiagnosticLevel.Error, "Argument of type \"(int, any) => int\" is not assignable to parameter of type \"any => any\"."),
                ("BCP070", DiagnosticLevel.Error, "Argument of type \"() => 'Hi!'\" is not assignable to parameter of type \"any => any\"."),
            });
        }

        [TestMethod]
        public void Lambdas_are_blocked_in_ternary_or_parenthesized_expressions()
        {
            var result = CompilationHelper.Compile(@"
var ternary = map([123], true ? abc => '${abc}' : def => 'hello!')
var parentheses = map([123], (foo => '${foo}'))
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP241", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                ("BCP241", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                ("BCP241", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
            });
        }

        [TestMethod]
        public void Lambdas_cannot_be_assigned_to_variables()
        {
            var result = CompilationHelper.Compile(@"
var foo = i => 123
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP241", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
            });
        }

        [TestMethod]
        public void Filter_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = filter([123], abc => a|bc == 123)
var fo|o2 = filter(['abc', 'def'], a|bc => abc == '123')
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("int[]"),
                x => x.Type.Name.Should().Be("int"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("string"));
        }

        [TestMethod]
        public void Sort_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = sort([123], (abc, def) => a|bc < def)
var fo|o2 = sort(['bar', 'foo'], (abc, def) => abc < d|ef)
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("int[]"),
                x => x.Type.Name.Should().Be("int"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("string"));
        }

        [TestMethod]
        public void Reduce_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = reduce([123], 0, (c|ur, next) => cur + next)
var fo|o2 = reduce(['abc', 'def'], '', (cur, nex|t) => concat(cur, next))
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("int"),
                x => x.Type.Name.Should().Be("int"),
                x => x.Type.Name.Should().Be("string"),
                x => x.Type.Name.Should().Be("string"));
        }

        [TestMethod]
        public void Lambda_functions_can_be_nested()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var abc = map(
  ['abc', 'def'],
  a => map(
    split(a, ','),
    b => map(
      split(b, ','),
      c => map(
        split(c, ','),
        d => map(
          split(d, ','),
          e => 'Hello ${e}'
        )
      )
    )
  )
)
");

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.variables['abc']", "[map(" +
                "createArray('abc', 'def'), " +
                "lambda('a', map(" +
                    "split(lambdaVariables('a'), ','), " +
                    "lambda('b', map(" +
                        "split(lambdaVariables('b'), ','), " +
                        "lambda('c', map(" +
                            "split(lambdaVariables('c'), ','), " +
                            "lambda('d', map(" +
                                "split(lambdaVariables('d'), ','), " +
                                "lambda('e', format('Hello {0}', lambdaVariables('e'))))))))))))]");
        }
    }
}
