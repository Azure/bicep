// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class ThisNamespaceTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void ThisNamespace_ShouldOnlyWorkInResourcePropertiesWhenFeatureEnabled()
        {
            // Test that this.exists() function fails outside of resource properties when feature is enabled
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true)),
                @"
                output result bool = this.exists()
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP437", DiagnosticLevel.Error, "The \"this.exists()\" function can only be used within resource property expressions.")
            });

            // Test that this.exists() function works inside resource properties when feature is enabled
            var result2 = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true)),
                @"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    allowBlobPublicAccess: this.exists()
                  }
                }
                ");
            // Should not have the BCP437 error when used inside resource properties
            result2.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void ThisNamespace_ShouldNotBeRecognizedWhenFeatureDisabled()
        {
            // Test that this.exists() function is not recognized when feature is disabled
            var result = CompilationHelper.Compile(@"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    customProperty: this.exists()
                  }
                }
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });
        }

        [TestMethod]
        public void ThisNamespace_CompilationGeneratesCorrectArmFunctions()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.exists()
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.exists() is compiled to targetExists()
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[targetExists()]");
            }
        }

        [TestMethod]
        public void ThisNamespace_InTopLevelResourceProperties_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: this.exists() ? 'testStorage' : 'defaultStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.exists()
  }
}
");

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Storage/storageAccounts\" type, which requires a value that can be calculated at the start of the deployment."),
                ("BCP437", DiagnosticLevel.Error, "The \"this.exists()\" function can only be used within resource property expressions.")

            });
        }

        [TestMethod]
        public void ThisNamespace_InModuleParameters_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, ("main.bicep", @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: true
  }
}

module testModule 'storage.bicep' = {
  name: 'testModule'
  params: {
    storageExists: this.exists()
  }
}
"), ("storage.bicep", @"
param storageExists bool
output sto bool = storageExists
"));

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP437", DiagnosticLevel.Error, "The \"this.exists()\" function can only be used within resource property expressions.")
            });
        }

        [TestMethod]
        public void ThisNamespace_InNestedAndResourceProperty_ShouldSucceed()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: this.exists() ? {
    allowBlobPublicAccess: true
    encryption: {
      keySource: 'Microsoft.KeyVault'
      services: {
        blob: {
          enabled: this.exists()
        }
      }
    }
  } : {}
}
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources.testResource.properties",
                "[if(targetExists(), createObject('allowBlobPublicAccess', true(), 'encryption', createObject('keySource', 'Microsoft.KeyVault', 'services', createObject('blob', createObject('enabled', targetExists())))), createObject())]");
        }

        [TestMethod]
        public void ThisNamespace_MultiplePropertiesInSameResource_ShouldSucceed()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.exists()
    supportsHttpsTrafficOnly: this.exists()
    minimumTlsVersion: this.exists() ? 'TLS1_2' : 'TLS1_0'
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that all usages compile to targetExists()
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[targetExists()]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.supportsHttpsTrafficOnly", "[targetExists()]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[if(targetExists(), 'TLS1_2', 'TLS1_0')]");
            }
        }
    }
}
