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
public class FunctionTests
{
    [NotNull] public TestContext? TestContext { get; set; }

    [TestMethod]
    public void User_defined_functions_basic_case()
    {
        var result = CompilationHelper.Compile(@"
func buildUrl = (bool https, string hostname, string path) => string '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template);

        evaluated.Should().HaveValueAtPath("$.outputs['foo'].value", "https://google.com/search");
    }

    [TestMethod]
    public void Outer_scope_symbolic_references_are_blocked()
    {
        var result = CompilationHelper.Compile(@"
param foo string
var bar = 'abc'
func getBaz = () => string 'baz'

func testFunc = (string baz) => '${foo}-${bar}-${baz}-${getBaz()}'
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
        var result = CompilationHelper.Compile(@"
@description('Returns foo')
func returnFoo = () => string 'foo'

output outputFoo string = returnFoo()
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template);

        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "foo");
    }

    [TestMethod]
    public void User_defined_functions_cannot_reference_each_other()
    {
        var result = CompilationHelper.Compile(@"
func getAbc = () => string 'abc'
func getAbcDef = () => string '${getAbc()}def'
");
    
        result.Should().HaveDiagnostics(new [] {
            ("BCP057", DiagnosticLevel.Error, "The name \"getAbc\" does not exist in the current context."),
        });
    }

    [TestMethod]
    public void User_defined_functions_unsupported_custom_types()
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new(UserDefinedTypesEnabled: true));

        var result = CompilationHelper.Compile(services, @"
func getAOrB = (('a' | 'b') aOrB) => bool (aOrB == 'a')
");
    
        // TODO(functions) this should raise a diagnostic - we can only emit simple types
        result.Should().HaveDiagnostics(new [] {
            ("TODO", DiagnosticLevel.Error, "This should be blocked!"),
        });

        result = CompilationHelper.Compile(services, @"
func getAOrB = (bool aOrB) => ('a' | 'b') aOrB ? 'a' : 'b'
");
    
        // TODO(functions) this should raise a diagnostic - we can only emit simple types
        result.Should().HaveDiagnostics(new [] {
            ("TODO", DiagnosticLevel.Error, "This should be blocked!"),
        });
    }

    [TestMethod]
    public void User_defined_functions_unsupported_runtime_functions()
    {
        var result = CompilationHelper.Compile(@"
func useRuntimeFunction = () => string reference('foo').bar
");
    
        result.Should().HaveDiagnostics(new [] {
            ("BCP340", DiagnosticLevel.Error, "This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment."),
        });
    }

    [TestMethod]
    public void Current_syntax_is_ambiguous()
    {
        var result = CompilationHelper.Compile(@"
func sayBlah2 = (string name) => array [
  true
]
");

        // TODO(functions) this shouldn't emit any diagnostics
        result.Should().NotHaveAnyDiagnostics();
    }
}