// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseSecureValueForSecureInputsRuleTests : LinterRuleTestsBase
{
    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(UseSecureValueForSecureInputsRule.Code, text, expectedDiagnosticCount, options);
    }

    [DataRow("""
resource ubuntuVM 'Microsoft.Compute/virtualMachineScaleSets@2023-09-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    virtualMachineProfile: {
      osProfile: {
        adminUsername: 'adminUsername'
        adminPassword: 'adminPassword'
      }
    }
  }
}
""")]
    [DataRow("""
resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: 'adminPassword'
    }
  }
}
""")]
    [DataRow("""
param adminPassword string

resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: adminPassword
    }
  }
}
""")]
    [DataRow("""
resource server 'Microsoft.DBforMySQL/flexibleServers@2023-12-30' = {
  name: 'myServer'
  location: 'westus'
  properties: {
    administratorLoginPassword: 'hardCodedValue'
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_warn_for_insecure_fields(string text)
        => CompileAndTest(text, 1);

    [DataRow("""
@secure()
param adminPassword string

resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: adminPassword
    }
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_secure_password_field(string text)
        => CompileAndTest(text, 0);

    [DataRow("""
param sqlLogicalServer object

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: null
  }
}
""")]
    [DataRow("""
param sqlLogicalServer object

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: ''
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_null_or_empty_string_assignment(string text)
        => CompileAndTest(text, 0);

    [DataRow("""
resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'azcli-script'
  location: 'westus'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.0.81'
    scriptContent: 'echo "Hello, World!"'
    retentionInterval: 'PT1H'
    storageAccountSettings: {
      storageAccountName: storageAccount.name
      storageAccountKey: storageAccount.listKeys().keys[0].value
    }
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_chained_values(string text)
        => CompileAndTest(text, 0);

    // https://github.com/Azure/bicep/issues/17371
    [DataRow("""
param sqlLogicalServer object
@secure()
param password string

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: sqlLogicalServer.login =~ 'Entra' ? null : password
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_conditional_null_assignment(string text)
          => CompileAndTest(text, 0);
}
