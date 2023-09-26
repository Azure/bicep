// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoDeploymentsResourcesRuleTests : LinterRuleTestsBase
{
    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(NoDeploymentsResourcesRule.Code, text, expectedDiagnosticCount, options);
    }

    [DataRow("""
param name string
param specId string
resource foo 'Microsoft.Resources/deployments@2021-04-01' = {
  name: name
  properties: {
    mode: 'Incremental'
    templateLink: {
      uri: specId
    }
    parameters: {}
  }
}
""")]
    [DataRow("""
param name string
resource foo 'Microsoft.Resources/deployments@2021-04-01' existing = {
  name: name
}
""")]
    [DataTestMethod]
    public void Linter_validation_should_warn_for_nested_deployment_resources(string text)
    {
        CompileAndTest(text, 1);
    }

    [DataRow("""
param name string
param location string
resource foo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  name: name
  properties: {
    azCliVersion: 's'
    retentionInterval: 's'
  }
  location: location
}
""")]
    [DataTestMethod]
    public void Linter_validation_should_not_warn_for_non_deployment_resource_types(string text)
    {
        CompileAndTest(text, 0);
    }
}
