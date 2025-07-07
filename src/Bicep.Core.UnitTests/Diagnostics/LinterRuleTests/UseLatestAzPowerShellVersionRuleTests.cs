// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UseLatestAzPowerShellVersionRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void DeploymentScriptWithOldVersion_ShouldWarn()
        {
            var result = Compile(@"
resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'test-script'
  location: resourceGroup().location
  kind: 'AzurePowerShell'
  properties: {
    azPowerShellVersion: '3.0'
    scriptContent: 'Write-Output \"Hello World\"'
    retentionInterval: 'PT1H'
  }
}");

            result.Should().HaveDiagnostics(new[] {
                ("use-latest-az-powershell-version", DiagnosticLevel.Warning, "Deployment script is using AzPowerShell version '3.0' which is below the recommended minimum version '11.0'. Consider upgrading to version 11.0 or higher to avoid EOL Ubuntu 20.04 LTS."),
            });
        }

        [TestMethod]
        public void DeploymentScriptWithNewVersion_ShouldNotWarn()
        {
            var result = Compile(@"
resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'test-script'
  location: resourceGroup().location
  kind: 'AzurePowerShell'
  properties: {
    azPowerShellVersion: '11.0'
    scriptContent: 'Write-Output \"Hello World\"'
    retentionInterval: 'PT1H'
  }
}");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void NonDeploymentScriptResource_ShouldNotWarn()
        {
            var result = Compile(@"
resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'teststorage'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void DeploymentScriptWithoutAzPowerShellVersion_ShouldNotWarn()
        {
            var result = Compile(@"
resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'test-script'
  location: resourceGroup().location
  kind: 'AzureCLI'
  properties: {
    scriptContent: 'echo \"Hello World\"'
    retentionInterval: 'PT1H'
  }
}");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
} 