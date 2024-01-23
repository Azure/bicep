// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class NestedRuntimeMemberAccessTests
    {
        [TestMethod]
        public void Nested_runtime_member_access_is_not_supported()
        {
            var result = CompilationHelper.Compile(@"
resource appPlan 'Microsoft.Web/serverfarms@2020-12-01' existing = {
  name: 'foo'
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'westus'

  resource windowsVMDsc 'extensions' existing = {
    name: appPlan.properties.provisioningState
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: appPlan.properties.provisioningState
}

resource vm2 'Microsoft.Compute/virtualMachines@2020-12-01' existing = {
  name: appPlan.properties.workerTierName // Create a runtime reference.
}

resource windowsVMDsc2 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' existing = {
  name: 'vmDsc2'
  parent: vm2
}

resource diag 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' existing = {
  name: 'diag'
  scope: vm2
}

var foo = vm::windowsVMDsc.properties.autoUpgradeMinorVersion
var bar = storage.properties.accessTier
var baz = storage.listKeys().keys
var qux = windowsVMDsc2.properties
var quux = diag.properties
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP307", DiagnosticLevel.Error, @"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including ""name"" cannot be calculated at the start of the deployment. In this situation, the accessible properties of ""windowsVMDsc"" include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including ""name"" cannot be calculated at the start of the deployment. In this situation, the accessible properties of ""storage"" include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including ""name"" cannot be calculated at the start of the deployment. In this situation, the accessible properties of ""storage"" include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including ""parent"" cannot be calculated at the start of the deployment. In this situation, the accessible properties of ""windowsVMDsc2"" include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including ""scope"" cannot be calculated at the start of the deployment. In this situation, the accessible properties of ""diag"" include ""apiVersion"", ""id"", ""name"", ""type""."),
            });
        }
    }
}
