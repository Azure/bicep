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
                ("BCP441", DiagnosticLevel.Error, "The \"this\" namespace can only be used within resource property expressions.")
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
            // Should not have the BCP441 error when used inside resource properties
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

                // Check that this.exists() is compiled to target('exists')
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[target('exists')]");
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
                ("BCP441", DiagnosticLevel.Error, "The \"this\" namespace can only be used within resource property expressions.")

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
                ("BCP441", DiagnosticLevel.Error, "The \"this\" namespace can only be used within resource property expressions.")
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
                "[if(target('exists'), createObject('allowBlobPublicAccess', true(), 'encryption', createObject('keySource', 'Microsoft.KeyVault', 'services', createObject('blob', createObject('enabled', target('exists'))))), createObject())]");
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

                // Check that all usages compile to target('exists')
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[target('exists')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.supportsHttpsTrafficOnly", "[target('exists')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[if(target('exists'), 'TLS1_2', 'TLS1_0')]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_ShouldOnlyWorkInResourcePropertiesWhenFeatureEnabled()
        {
            // Test that this.existingProperties() function fails outside of resource properties when feature is enabled
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true)),
                @"
                output result object = this.existingProperties()
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP441", DiagnosticLevel.Error, "The \"this\" namespace can only be used within resource property expressions.")
            });

            // Test that this.existingProperties() function works inside resource properties when feature is enabled
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
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
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
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[if(target('exists'), target().minimumTlsVersion, 'TLS1_0')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[if(target('exists'), target().allowBlobPublicAccess, false())]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy.expirationAction", "[if(target('exists'), target().sasPolicy.expirationAction, 'Block')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy.sasExpirationPeriod", "[if(target('exists'), target().sasPolicy.sasExpirationPeriod, '2')]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingProperties_ShouldNotBeRecognizedWhenFeatureDisabled()
        {
            // Test that this.existingProperties() function is not recognized when feature is disabled
            var result = CompilationHelper.Compile(@"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    customProperty: this.existingProperties().someProperty
                  }
                }
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });
        }

        [TestMethod]
        public void ThisNamespace_ExistingTags_ShouldOnlyWorkInResourcePropertiesWhenFeatureEnabled()
        {
            // Test that this.existingTags() function fails outside of resource properties when feature is enabled
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true)),
                @"
                output result object = this.existingResource()
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP441", DiagnosticLevel.Error, "The \"this\" namespace can only be used within resource property expressions.")
            });

            // Test that this.existingTags() function works inside resource properties when feature is enabled
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
                  properties: this.existingResource().properties
                }
                ");
            // Should not have the BCP441 error when used inside resource properties
            result2.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void ThisNamespace_ExistingResourceProperties_CompilationGeneratesCorrectArmFunctions()
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

                // Check that the three supported functions compile correctly
                template.Should().HaveValueAtPath("$.resources.testResource.properties.accessTier", "[tryGet(target('full'), 'properties', 'accessTier')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[target('exists')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy", "[tryGet(target(), 'sasPolicy')]");
                template.Should().HaveValueAtPath("$.resources.secret.properties.value", "[target('full').tags.secretValue]");
            }
        }

        [TestMethod]
        public void ThisNamespace_ExistingResourceProperties_ShouldNotBeRecognizedWhenFeatureDisabled()
        {
            // Test that the functions are not recognized when feature is disabled
            var result = CompilationHelper.Compile(@"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  tags: this.existingTags()
                  properties: {
                    allowBlobPublicAccess: this.exists()
                  }
                }
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context."),
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });
        }

        [TestMethod]
        public void ThisNamespace_AllSupportedFunctions_ShouldWork()
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
    sasPolicy: this.existingProperties().sasPolicy
    accessTier: this.existingResource().properties.accessTier
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that all three supported functions compile correctly
                template.Should().HaveValueAtPath("$.resources.testResource.properties.accessTier", "[target('full').properties.accessTier]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[target('exists')]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy", "[target().sasPolicy]");
            }
        }
    }
}
