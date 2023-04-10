// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
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
func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

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
func getBaz = () => 'baz'

func testFunc = (string baz) => '${foo}-${bar}-${baz}-${getBaz()}'
");

        result.Should().HaveDiagnostics(new [] {
            ("BCP340", DiagnosticLevel.Error, """Symbol "foo" cannot be used here. Function bodies must only refer to symbols declared as parameters."""),
            ("BCP340", DiagnosticLevel.Error, """Symbol "bar" cannot be used here. Function bodies must only refer to symbols declared as parameters."""),
            ("BCP340", DiagnosticLevel.Error, """Symbol "getBaz" cannot be used here. Function bodies must only refer to symbols declared as parameters."""),
        });
    }

    [TestMethod]
    public void Functions_can_have_descriptions_applied()
    {
        var result = CompilationHelper.Compile(@"
@description('Returns foo')
func returnFoo = () => 'foo'

output outputFoo string = returnFoo()
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template);

        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "foo");
    }
}