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
        public void ThisNamespace_ShouldNotBreakExistingVariablesFeatureDisabled()
        {
            var result = CompilationHelper.Compile(@"
                var this = false
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    allowBlobPublicAccess: this
                  }
                }
                ");
            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.exists() is compiled to not(empty(target('full')))
                template.Should().HaveValueAtPath("$.resources[0].properties.allowBlobPublicAccess", "[variables('this')]");
            }
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
        public void ThisNamespace_CompilationGeneratesCorrectArmFunctionsInForLoops()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = [for i in range(0, 3): {
  name: 'testStorage-${i}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this.exists()
  }
}]
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
        public void ThisNamespace_CompilationGeneratesCorrectArmFunctionsNested()
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
  resource fileService 'fileServices' = {
    name: 'default'
    properties: {
      shareDeleteRetentionPolicy: this.existingResource().?properties.shareDeleteRetentionPolicy
    }

    resource fileShare 'shares' = {
      name: 'exampleshare'
      properties: {
        accessTier: this.existingResource().?properties.accessTier
      }
    }
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.exists() is compiled to not(empty(target('full')))
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[not(empty(target('full')))]");
                template.Should().HaveValueAtPath("$.resources.testResource::fileService.properties.shareDeleteRetentionPolicy", "[tryGet(target('full'), 'properties', 'shareDeleteRetentionPolicy')]");
                template.Should().HaveValueAtPath("$.resources.testResource::fileService::fileShare.properties.accessTier", "[tryGet(target('full'), 'properties', 'accessTier')]");
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
        public void ThisNamespace_ExistingResource_CompilationGeneratesCorrectArmFunctionsWithTryGet()
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
    allowBlobPublicAccess: this.existingResource().?properties.allowBlobPublicAccess ?? false
    minimumTlsVersion: this.existingResource().?properties.minimumTlsVersion ?? 'TLS1_0'
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this.existingResource().properties is compiled to target('full').properties and property access works
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[coalesce(tryGet(target('full'), 'properties', 'allowBlobPublicAccess'), false())]");
                template.Should().HaveValueAtPath("$.resources.testResource.properties.minimumTlsVersion", "[coalesce(tryGet(target('full'), 'properties', 'minimumTlsVersion'), 'TLS1_0')]");
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
    sasPolicy: this.existingResource().?properties.sasPolicy
    accessTier: this.existingResource().?properties.accessTier
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2024-12-01-preview' = {
  name: 'vault/secret'
  properties: {
    value: this.existingResource().?tags.secretValue
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
                template.Should().HaveValueAtPath("$.resources.testResource.properties.sasPolicy", "[tryGet(target('full'), 'properties', 'sasPolicy')]");
                template.Should().HaveValueAtPath("$.resources.secret.properties.value", "[tryGet(target('full'), 'tags', 'secretValue')]");
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
                      required: this.existingResource().?properties.required ?? 'defaultValue'
                      readwrite: this.existingResource().?identifier
                    }
                  }
                  """)), ("../extension.tgz", extensionTgz));

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that all three supported functions compile correctly
                template.Should().HaveValueAtPath("$.resources.fooRes.properties.properties.required", "[coalesce(tryGet(target(), 'properties', 'required'), 'defaultValue')]");
                template.Should().HaveValueAtPath("$.resources.fooRes.properties.properties.readwrite", "[tryGet(target(), 'identifier')]");
            }
        }

        [TestMethod]
        public async Task ThisNamespace_ExtensibleResource_Diagnostics()
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
                      required: this.existingResource().?properties.required ?? 'defaultValue'
                      readonly: this.existingResource().?properties.readonly
                      writeonly: this.existingResource().?properties.writeonly
                    }
                  }
                  """)), ("../extension.tgz", extensionTgz));

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP073", DiagnosticLevel.Warning, "The property \"readonly\" is read-only. Expressions cannot be assigned to read-only properties. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"writeonly\" expected a value of type \"null | string\" but the provided value is of type \"error | null\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
                ("BCP077", DiagnosticLevel.Error, "The property \"writeonly\" on type \"fooBody\" is write-only. Write-only properties cannot be accessed.")
            });
        }

        [TestMethod]
        public void ThisNamespace_VariableThisShouldRaiseDiagnosticWhenFeatureEnabled_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
var this = 'test'

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

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP084", DiagnosticLevel.Error, "The symbolic name \"this\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\", \"this\"."),
                ("no-unused-vars", DiagnosticLevel.Warning, "Variable \"this\" is declared but never used.")
            });
        }

        [TestMethod]
        public void ThisNamespace_ThisInExisting_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: this.exists() ? 'testStorage' : 'defaultStorage'
}

");

            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.");
        }

        [TestMethod]
        public void ThisNamespace_ForLoopNamedThis_ShouldSucceed()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisNamespaceEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = [for this in range(0,2): {
  name: 'testStorage${this}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: true
  }
}]

");

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                var template = result.Template;

                template.Should().HaveValueAtPath("$.resources.testResource.name", "[format('testStorage{0}', range(0, 2)[copyIndex()])]");
            }
        }
    }
}
