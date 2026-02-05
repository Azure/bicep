// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Emit;

[TestClass]
public class ParameterAssignmentEvaluatorTests
{
    [TestMethod]
    public void BuildParams_ForExpressionVariable_EvaluatesToValue()
    {
        var services = new ServiceBuilder().WithEmptyAzResources();

        var result = CompilationHelper.CompileParams(
            services,
            ("main.bicep", "param p int[]"),
            ("parameters.bicepparam", """
            using 'main.bicep'

            var x = [for item in [1, 2]: item * 2]
            param p = x
            """));

        result.Should().NotHaveAnyDiagnostics();
        result.Parameters.Should().HaveValueAtPath("parameters.p.value", JToken.Parse("[2, 4]"));
    }
}
