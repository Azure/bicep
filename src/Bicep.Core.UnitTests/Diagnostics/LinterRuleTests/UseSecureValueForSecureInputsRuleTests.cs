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
    [TestMethod]
    public void Linter_validation_should_warn_for_insecure_password_field(string text)
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
}
