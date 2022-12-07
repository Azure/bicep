// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
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
        public void Parentheses_without_arrow_are_not_interpreted_as_lambdas()
        {
            CompilationHelper.Compile("var noElements = ()")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP243", DiagnosticLevel.Error, "Parentheses must contain exactly one expression."),
                });

            CompilationHelper.Compile("var justAComma = (,)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
                    ("BCP243", DiagnosticLevel.Error, "Parentheses must contain exactly one expression."),
                });

            CompilationHelper.Compile("var twoElements = (1, 2)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP243", DiagnosticLevel.Error, "Parentheses must contain exactly one expression."),
                });

            CompilationHelper.Compile("var threeElements = (1, 2, 3)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP243", DiagnosticLevel.Error, "Parentheses must contain exactly one expression."),
                });

            CompilationHelper.Compile("var unterminated1 = (")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
                });

            CompilationHelper.Compile("var unterminated2 = (,")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
                    ("BCP243", DiagnosticLevel.Error, "Parentheses must contain exactly one expression."),
                    ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
                });
        }

        [TestMethod]
        public void Lambdas_can_be_placed_inside_parentheses_and_nothing_else()
        {
            CompilationHelper.Compile("var noParens = map([1], i => i)")
                .ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            CompilationHelper.Compile("var noParens = map([1], (i => i))")
                .ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            CompilationHelper.Compile("var noParens = map([1], (((i => i))))")
                .ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            CompilationHelper.Compile("var asfsasdf = map([1], true ? i => i + 1 : i => i)")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                    ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                });

            CompilationHelper.Compile("var asfsasdf = map([1], true ? (i => i + 1) : (i => i))")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                    ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                });

            CompilationHelper.Compile("var asfsasdf = map([1], [i => i])")
                .ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"[any => any]\" is not assignable to parameter of type \"any => any\"."),
                    ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                });
        }

        [TestMethod]
        public void Map_function_preserves_types_accurately_integers()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map([123], ab|c => abc)
var fo|o2 = map([123], a|bc => 'Hello ${abc}')
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("123[]"),
                x => x.Type.Name.Should().Be("123"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("123"));
        }

        [TestMethod]
        public void Map_function_preserves_types_accurately_strings()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map(['abc', 'def'], ab|c => abc)
var fo|o2 = map(['abc', 'def'], a|bc => length(abc))
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("('abc' | 'def')[]"),
                x => x.Type.Name.Should().Be("'abc' | 'def'"),
                x => x.Type.Name.Should().Be("int[]"),
                x => x.Type.Name.Should().Be("'abc' | 'def'"));
        }

        [TestMethod]
        public void Map_function_works_with_unknowable_types()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = map([], ab|c => abc)
var fo|o2 = map([any('foo')], a|bc => 'Hi ${abc}!')
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("never[]"),
                x => x.Type.Name.Should().Be("never"),
                x => x.Type.Name.Should().Be("string[]"),
                x => x.Type.Name.Should().Be("any"));
        }

        [TestMethod]
        public void Map_function_blocks_incorrect_args()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var foo = map([123], (abc, def) => abc)
var foo2 = map(['foo'], () => 'Hi!')
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP070", DiagnosticLevel.Error, "Argument of type \"(123, any) => 123\" is not assignable to parameter of type \"any => any\"."),
                ("BCP070", DiagnosticLevel.Error, "Argument of type \"() => 'Hi!'\" is not assignable to parameter of type \"any => any\"."),
            });
        }

        [TestMethod]
        public void Lambdas_are_blocked_in_ternary_expressions()
        {
            var result = CompilationHelper.Compile(@"
var ternary = map([123], true ? abc => '${abc}' : def => 'hello!')
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
                ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
            });
        }

        [TestMethod]
        public void Lambdas_cannot_be_assigned_to_variables()
        {
            var result = CompilationHelper.Compile(@"
var foo = i => 123
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP242", DiagnosticLevel.Error, "Lambda functions may only be specified directly as function arguments."),
            });
        }

        [TestMethod]
        public void Filter_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = filter([123], abc => a|bc == 123)
var fo|o2 = filter(['abc', 'def'], a|bc => abc == '123')
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("123[]"),
                x => x.Type.Name.Should().Be("123"),
                x => x.Type.Name.Should().Be("('abc' | 'def')[]"),
                x => x.Type.Name.Should().Be("'abc' | 'def'"));
        }

        [TestMethod]
        public void Sort_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = sort([123], (abc, def) => a|bc < def)
var fo|o2 = sort(['bar', 'foo'], (abc, def) => abc < d|ef)
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("123[]"),
                x => x.Type.Name.Should().Be("123"),
                x => x.Type.Name.Should().Be("('bar' | 'foo')[]"),
                x => x.Type.Name.Should().Be("'bar' | 'foo'"));
        }

        [TestMethod]
        public void Reduce_lambda_functions_assigns_types_accurately()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var fo|o = reduce([123], 0, (c|ur, next) => cur + next)
var fo|o2 = reduce(['abc', 'def'], '', (cur, nex|t) => concat(cur, next))
",
                '|');

            var result = CompilationHelper.Compile(file);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var info = result.GetInfoAtCursors(cursors);

            info.Should().SatisfyRespectively(
                x => x.Type.Name.Should().Be("246"),
                x => x.Type.Name.Should().Be("123"),
                x => x.Type.Name.Should().Be("string"),
                x => x.Type.Name.Should().Be("'abc' | 'def'"));
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
",
                '|');

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

        [TestMethod]
        public void Lambda_functions_cannot_be_used_to_dynamically_access_resource_collections()
        {
            var result = CompilationHelper.Compile(@"
param ids array
resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' = [for i in range(0, 2): {
  name: 'antteststg${i}'
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

output stgKeys array = map(range(0, 2), i => stg[i].listKeys().keys[0].value)
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                ("BCP247", DiagnosticLevel.Error, "Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: \"i\"."),
                ("BCP247", DiagnosticLevel.Error, "Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: \"j\"."),
                ("BCP248", DiagnosticLevel.Error, "Using lambda variables inside the \"listKeys\" function is not currently supported. Found the following lambda variable(s) being accessed: \"id\"."),
                ("BCP247", DiagnosticLevel.Error, "Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: \"k\"."),
                ("BCP247", DiagnosticLevel.Error, "Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: \"x\", \"y\"."),
                ("BCP248", DiagnosticLevel.Error, "Using lambda variables inside the \"reference\" function is not currently supported. Found the following lambda variable(s) being accessed: \"foo\"."),
            });
        }

        [TestMethod]
        public void Lambda_functions_cannot_be_used_to_dynamically_access_module_collections()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
module myMod './module.bicep' = [for i in range(0, 2): {
  name: 'myMod${i}'
}]

output modOutputs array = map(range(0, 2), i => myMod[i].outputs.foo)"),
                ("module.bicep", @"
output foo string = 'HELLO!'
"));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                ("BCP247", DiagnosticLevel.Error, "Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: \"i\"."),
            });
        }

        [TestMethod]
        public void DeployTimeConstant_detection_works_with_lambdas()
        {
            var result = CompilationHelper.Compile(@"
resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'blah'
}

var nonDtcArr = map(range(0, 1), i => stg.properties.accessTier)

resource stg2 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: 'foo${nonDtcArr[0]}'
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Storage/storageAccounts\" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"nonDtcArr\" -> \"stg\"). Properties of stg which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }
    }
}
