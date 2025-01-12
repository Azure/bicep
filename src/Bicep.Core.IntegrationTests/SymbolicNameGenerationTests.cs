// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;


[TestClass]
public class SymbolicNameTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    [EmbeddedFilesTestData(@"Files/SymbolicNameTests/ResourceInfo/.*/main\.bicep")]
    public async Task ResourceInfoCodegenEnabled_output_is_valid(EmbeddedFile bicepFile)
        => await ExamplesTests.RunExampleTest(TestContext, bicepFile, new(ResourceInfoCodegenEnabled: true));

    [TestMethod]
    public void Unqualified_names_are_output_correctly()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(ResourceInfoCodegenEnabled: true)), """
resource noParent 'Microsoft.Test/parent/child@2020-01-01' = {
  name: 'parent/noParent'
}

resource foo 'Microsoft.Test/parent@2020-01-01' = {
  name: 'parent'

  resource nested 'child' = {
    name: 'nested'
  }
}

resource parentProperty 'Microsoft.Test/parent/child@2020-01-01' = {
  parent: foo
  name: 'parentProperty'
}

output noParent string = noParent.name
output nested string = foo::nested.name
output parentProperty string = parentProperty.name
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyCompilationBlockingDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['noParent'].value", "parent/noParent");
        evaluated.Should().HaveValueAtPath("$.outputs['nested'].value", "nested");
        evaluated.Should().HaveValueAtPath("$.outputs['parentProperty'].value", "parentProperty");
    }
}
