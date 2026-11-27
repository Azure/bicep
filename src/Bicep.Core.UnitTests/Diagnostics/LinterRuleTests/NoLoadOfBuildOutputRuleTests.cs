// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoLoadOfBuildOutputRuleTests : LinterRuleTestsBase
{
    private const string Json = """{ "billingDept": "1234" }""";

    [DataRow("var consts = loadJsonContent('main.json')")]
    [DataRow("var consts = loadJsonContent('./main.json')")]
    [DataRow("var consts = loadJsonContent('main.json', '$.billingDept')")]
    [DataRow("var consts = loadTextContent('main.json')")]
    [DataRow("var consts = loadFileAsBase64('main.json')")]
    [DataRow("var consts = loadYamlContent('main.json')")]
    [DataRow("var consts = loadJsonContent('sub/../main.json')")]
    [TestMethod]
    public void Should_raise_diagnostic_when_loading_default_build_output(string text)
        => CompileAndTest(text, 1);

    [DataRow("var consts = loadJsonContent('other.json')")]
    [DataRow("var consts = loadJsonContent('sub/main.json')")]
    [DataRow("var consts = loadYamlContent('main.yaml')")]
    [DataRow("var consts = loadTextContent('main.bicep')")]
    [TestMethod]
    public void Should_not_raise_diagnostic_for_other_files(string text)
        => CompileAndTest(text, 0);

    private static void CompileAndTest(string text, int expectedCount)
    {
        var options = new Options(AdditionalFiles:
        [
            ("main.json", Json),
            ("other.json", Json),
            ("main.yaml", "billingDept: '1234'"),
            ("sub/main.json", Json),
        ]);
        AssertLinterRuleDiagnostics(NoLoadOfBuildOutputRule.Code, text, expectedCount, options);
    }
}
