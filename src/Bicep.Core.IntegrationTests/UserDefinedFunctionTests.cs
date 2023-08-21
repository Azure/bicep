// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class UserDefinedFunctionTests
{
    private ServiceBuilder Services => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, UserDefinedFunctionsEnabled: true));

    [NotNull] public TestContext? TestContext { get; set; }

    [TestMethod]
    public void User_defined_functions_basic_case()
    {
        var result = CompilationHelper.Compile(Services, @"
func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template);

        evaluated.Should().HaveValueAtPath("$.outputs['foo'].value", "https://google.com/search");
    }

    [TestMethod]
    public void Outer_scope_symbolic_references_are_blocked()
    {
        var result = CompilationHelper.Compile(Services, @"
param foo string
var bar = 'abc'
func getBaz() string => 'baz'

func testFunc(baz string) string => '${foo}-${bar}-${baz}-${getBaz()}'
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new [] {
            ("BCP057", DiagnosticLevel.Error, """The name "foo" does not exist in the current context."""),
            ("BCP057", DiagnosticLevel.Error, """The name "bar" does not exist in the current context."""),
            ("BCP057", DiagnosticLevel.Error, """The name "getBaz" does not exist in the current context."""),
        });
    }

    [TestMethod]
    public void Functions_can_have_descriptions_applied()
    {
        var result = CompilationHelper.Compile(Services, @"
@description('Returns foo')
func returnFoo() string => 'foo'

output outputFoo string = returnFoo()
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template);

        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "foo");
    }

    [TestMethod]
    public void User_defined_functions_cannot_reference_each_other()
    {
        var result = CompilationHelper.Compile(Services, @"
func getAbc() string => 'abc'
func getAbcDef() string => '${getAbc()}def'
");

        result.Should().HaveDiagnostics(new [] {
            ("BCP057", DiagnosticLevel.Error, "The name \"getAbc\" does not exist in the current context."),
        });
    }

    [TestMethod]
    public void User_defined_functions_support_custom_types()
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new(UserDefinedFunctionsEnabled: true));

        var result = CompilationHelper.Compile(services, @"
func getAOrB(aOrB ('a' | 'b')) bool => (aOrB == 'a')
");

        result.Should().NotHaveAnyDiagnostics();

        result = CompilationHelper.Compile(services, @"
func getAOrB(aOrB bool) ('a' | 'b') => aOrB ? 'a' : 'b'
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void User_defined_functions_unsupported_runtime_functions()
    {
        var result = CompilationHelper.Compile(Services, @"
func useRuntimeFunction() string => reference('foo').bar
");

        result.Should().HaveDiagnostics(new [] {
            ("BCP341", DiagnosticLevel.Error, "This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment."),
        });
    }

    [TestMethod]
    public void User_defined_functions_requires_experimental_feature_enabled()
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new());

        var result = CompilationHelper.Compile(services, @"
func useRuntimeFunction() string => 'test'
");

        result.Should().HaveDiagnostics(new [] {
            ("BCP343", DiagnosticLevel.Error, "Using a func declaration statement requires enabling EXPERIMENTAL feature \"UserDefinedFunctions\"."),
        });
    }
}
