// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Extensions;
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
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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

                // Check that this.exists() is compiled to not(empty(target('full')))
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[not(empty(target('full')))]");
            }
        }

        [TestMethod]
        public void ThisNamespace_InTopLevelResourceProperties_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Storage/storageAccounts\" type, which requires a value that can be calculated at the start of the deployment.")
            });
        }

        [TestMethod]
        public void ThisNamespace_InModuleParameters_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });
        }

        [TestMethod]
        public void ThisNamespace_InNestedAndResourceProperty_ShouldSucceed()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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
                "[if(not(empty(target('full'))), createObject('allowBlobPublicAccess', true(), 'encryption', createObject('keySource', 'Microsoft.KeyVault', 'services', createObject('blob', createObject('enabled', not(empty(target('full'))))))), createObject())]");
        }

        [TestMethod]
        public void ThisNamespace_MultiplePropertiesInSameResource_ShouldSucceed()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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

                // Check that all usages compile to not(empty(target('full')))
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[not(empty(target('full')))]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.supportsHttpsTrafficOnly", "[not(empty(target('full')))]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[if(not(empty(target('full'))), 'TLS1_2', 'TLS1_0')]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_ShouldOnlyWorkInResourcePropertiesWhenFeatureEnabled()
        {
            // Test that this.existingProperties() function fails outside of resource properties when feature is enabled
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true)),
                @"
                output result object = this.existingProperties()
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });

            // Test that this.existingProperties() function works inside resource properties when feature is enabled
            var result2 = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true)),
                @"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    allowBlobPublicAccess: this.existingProperties().allowBlobPublicAccess
                  }
                }
                ");
            // Should not have the BCP441 error when used inside resource properties
            result2.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_CompilationGeneratesCorrectArmFunctions()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.existingProperties().allowBlobPublicAccess
    minimumTlsVersion: this.existingProperties().minimumTlsVersion
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.existingProperties() is compiled to target() and property access works
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[target().allowBlobPublicAccess]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[target().minimumTlsVersion]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_CompilationGeneratesCorrectArmFunctionsWithTryGet()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.existingProperties().?allowBlobPublicAccess ?? false
    minimumTlsVersion: this.existingProperties().?minimumTlsVersion ?? 'TLS1_0'
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.existingProperties() is compiled to target() and property access works
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[coalesce(tryGet(target(), 'allowBlobPublicAccess'), false())]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[coalesce(tryGet(target(), 'minimumTlsVersion'), 'TLS1_0')]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_DirectUsage_ShouldWork()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: this.existingProperties()
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.existingProperties() is compiled to target()
                template.Should().HaveValueAtPath("$.resources.testResource.properties", "[target()]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistsAndExistingProperties_DirectUsage_ShouldWork()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: this.exists() ? this.existingProperties().minimumTlsVersion : 'TLS1_0'
    allowBlobPublicAccess: this.exists() ? this.existingProperties().allowBlobPublicAccess : false
    sasPolicy: {
      expirationAction: this.exists() ? this.existingProperties().sasPolicy.expirationAction : 'Block'
      sasExpirationPeriod: this.exists() ? this.existingProperties().sasPolicy.sasExpirationPeriod : '2'
    }
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.existingProperties() is compiled to target()
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[if(not(empty(target('full'))), target().minimumTlsVersion, 'TLS1_0')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[if(not(empty(target('full'))), target().allowBlobPublicAccess, false())]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy.expirationAction", "[if(not(empty(target('full'))), target().sasPolicy.expirationAction, 'Block')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy.sasExpirationPeriod", "[if(not(empty(target('full'))), target().sasPolicy.sasExpirationPeriod, '2')]");
            }
        }

        [TestMethod]
        public void ThisNamespace_AllSupportedFunctions_ShouldWork()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
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
    sasPolicy: this.existingProperties().?sasPolicy
    accessTier: this.existingResource().?properties.accessTier
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2024-12-01-preview' = {
  name: 'vault/secret'
  properties: {
    value: this.existingResource().tags.secretValue
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that all three supported functions compile correctly
                template.Should().HaveValueAtPath("$.resources.testResource.properties.accessTier", "[tryGet(target('full'), 'properties', 'accessTier')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[not(empty(target('full')))]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy", "[tryGet(target(), 'sasPolicy')]");
                template.Should().HaveValueAtPath("$.resources.secret.properties.value", "[target('full').tags.secretValue]");
            }
        }

        [TestMethod]
        public async Task ThisNamespace_ExtensibleResource()
        {
            var typesTgz = ExtensionResourceTypeHelper.GetTestTypesTgz();
            var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));

            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));

            var result = await CompilationHelper.RestoreAndCompile(services,
              ("main.bicep", new("""
                  extension '../extension.tgz'

                  resource fooRes 'fooType@v1' = {
                    identifier: 'foobar'
                    properties: {
                      required: this.existingProperties().properties.required
                    }
                  }
                  """)), ("../extension.tgz", extensionTgz));

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that all three supported functions compile correctly
                template.Should().HaveValueAtPath("$.resources.fooRes.properties.properties.required", "[target().properties.required]");
            }
        }
    }
}
