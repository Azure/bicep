// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseRecentAzPowerShellVersionRuleTests : LinterRuleTestsBase
{
    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(UseRecentAzPowerShellVersionRule.Code, text, expectedDiagnosticCount, options);
    }

    [DataRow("""
    resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
      name: 'test-script'
      location: resourceGroup().location
      kind: 'AzurePowerShell'
      properties: {
        azPowerShellVersion: '3.0'
        scriptContent: 'Write-Output "Hello World"'
        retentionInterval: 'PT1H'
      }
    }
    """)]
    [TestMethod]
    public void Linter_validation_should_warn_for_old_azpowershell_version(string text)
        => CompileAndTest(text, 1);

    [DataRow("""
    resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
      name: 'test-script'
      location: resourceGroup().location
      kind: 'AzurePowerShell'
      properties: {
        azPowerShellVersion: '11.0'
        scriptContent: 'Write-Output "Hello World"'
        retentionInterval: 'PT1H'
      }
    }
    """)]
    [DataRow("""
    resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
      name: 'test-script'
      location: resourceGroup().location
      kind: 'AzureCLI'
      properties: {
        scriptContent: 'echo "Hello World"'
        retentionInterval: 'PT1H'
      }
    }
    """)]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_new_or_irrelevant_cases(string text)
        => CompileAndTest(text, 0);
}
