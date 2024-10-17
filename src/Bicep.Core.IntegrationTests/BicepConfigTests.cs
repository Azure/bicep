// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class BicepConfigTests
{
    [TestMethod]
    public async Task Version_mismatch_should_block_compilation()
    {
        var result = await CompilationHelper.RestoreAndCompile(
          ("main.bicep", new("""
output foo string = ''
""")), ("../bicepconfig.json", new("""
{
  "bicep": {
    "version": "0.0.1"
  }
}
""")));

        var configPath = InMemoryFileResolver.GetFileUri("/path/bicepconfig.json");
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP409", DiagnosticLevel.Error, $"""Bicep version "{BicepVersion.Instance.Value}" was used for compilation, but version "0.0.1" is required in configuration file "{configPath.LocalPath}"."""),
        ]);
    }

    [TestMethod]
    public async Task Correct_version_should_permit_compilation()
    {
        var result = await CompilationHelper.RestoreAndCompile(
          ("main.bicep", new("""
output foo string= ''
""")), ("../bicepconfig.json", new($$"""
{
  "bicep": {
    "version": "{{BicepVersion.Instance.Value}}"
  }
}
""")));

        result.Should().NotHaveAnyDiagnostics();
    }
}
