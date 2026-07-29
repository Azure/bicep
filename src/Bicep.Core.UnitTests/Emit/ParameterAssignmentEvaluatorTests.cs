// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Testing.Assertions;
using Bicep.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Emit;

[TestClass]
public class ParameterAssignmentEvaluatorTests
{
    private static TestCompilationResult CompileParams(string bicepParamsText) => TestCompiler
        .ForInMemoryCompilation()
        .WithEmptyAzResources()
        .CompileWithoutRestore(
            "parameters.bicepparam",
            ("main.bicep", "param p int[]"),
            ("parameters.bicepparam", bicepParamsText));

    [TestMethod]
    public void BuildParams_ForExpressionParameter_EvaluatesToValue()
    {
        var result = CompileParams("""
            using 'main.bicep'

            param p = [for item in range(0, 4): item * 2]
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Parameters.Should().HaveValueAtPath("parameters.p.value", JToken.Parse("[0, 2, 4, 6]"));
    }

    [TestMethod]
    public void BuildParams_ForExpressionVariable_EvaluatesToValue()
    {
        var result = CompileParams("""
            using 'main.bicep'

            var x = [for item in [1, 2]: item * 2]
            param p = x
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Parameters.Should().HaveValueAtPath("parameters.p.value", JToken.Parse("[2, 4]"));
    }
}
