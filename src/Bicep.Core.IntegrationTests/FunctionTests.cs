// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
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
        result.Template.Should().HaveValueAtPath("$.outputs['prop1'].value", "[reference(resourceId('Microsoft.foo/bar', 'name'), '2020-01-01').prop1]");
    }
}