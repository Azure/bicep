// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'westus'

  resource windowsVMDsc 'extensions' existing = {
    name: appPlan.properties.provisioningState
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: appPlan.properties.provisioningState
}

var foo = virtualMachine::windowsVMDsc.properties.autoUpgradeMinorVersion
var bar = storage.properties.accessTier
var baz = storage.listKeys().keys
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP307", DiagnosticLevel.Error, @"The expression may lead to nested runtime functions, which is not supported. Accessible properties of windowsVMDsc include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression may lead to nested runtime functions, which is not supported. Accessible properties of storage include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP307", DiagnosticLevel.Error, @"The expression may lead to nested runtime functions, which is not supported. Accessible properties of storage include ""apiVersion"", ""id"", ""name"", ""type""."),
            });
        }
    }
}
