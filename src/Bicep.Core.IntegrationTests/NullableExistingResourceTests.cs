// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class NullableExistingResourceTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void NullableExisting_ShouldNotAllowWhitespaceBetweenExistingAndQuestionMark()
        {
            // Test that whitespace between 'existing' and '?' is not recognized as nullable existing
            // The parser only recognizes 'existing?' when there's no whitespace between them
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
#disable-next-line no-unused-existing-resources
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing ? = {
  name: 'testStorage'
}
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                // With whitespace, '?' is not recognized as part of 'existing', so parser expects '=' after 'existing'
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location.")
                });
            }
        }

        [TestMethod]
        public void NullableExisting_ShouldNotBeRecognizedWhenFeatureDisabled()
        {
            // Test that existing? syntax produces an error when feature is disabled
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
#disable-next-line no-unused-existing-resources
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP444", DiagnosticLevel.Error, "Using nullable existing resources (existing?) requires enabling EXPERIMENTAL feature \"NullableExisting\".")
                });
            }
        }

        [TestMethod]
        public void NullableExisting_ResourceTypeShouldBehaveAsConditional()
        {
            // Nullable existing resources should behave like conditional resources:
            // - Direct access to runtime properties should produce a warning
            // - Access to compile-time properties (id, name, type, apiVersion) should not produce warnings
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

// Access to compile-time properties - no warning expected
output resourceType string = storageAccount.type
output resourceId string = storageAccount.id
output resourceName string = storageAccount.name
output resourceApiVersion string = storageAccount.apiVersion
");
            using (new AssertionScope())
            {
                // No warnings for compile-time properties (same as conditional resources)
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                // Template should still be generated
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Verify nullableExisting is added to @options
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify compile-time property output expressions
                template.Should().HaveValueAtPath("$.outputs['resourceType'].value", "Microsoft.Storage/storageAccounts");
                template.Should().HaveValueAtPath("$.outputs['resourceId'].value", "[resourceId('Microsoft.Storage/storageAccounts', 'testStorage')]");
                template.Should().HaveValueAtPath("$.outputs['resourceName'].value", "testStorage");
                template.Should().HaveValueAtPath("$.outputs['resourceApiVersion'].value", "2021-04-01");
            }
        }

        [TestMethod]
        public void NullableExisting_RuntimePropertyAccess_ShouldProduceWarning()
        {
            // Access to runtime properties should produce a warning (same as conditional resources)
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

output skuName string = storageAccount.sku.name
");
            using (new AssertionScope())
            {
                // Should have warning because storageAccount might not exist at deployment time
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP318", DiagnosticLevel.Warning, "The value of type \"Microsoft.Storage/storageAccounts | null\" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail.")
                });
                // Template should still be generated (warning, not error)
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Verify nullableExisting is added to @options
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify the output expression for runtime property access
                template.Should().HaveValueAtPath("$.outputs['skuName'].value", "[reference('storageAccount', '2021-04-01', 'full').sku.name]");
            }
        }

        [TestMethod]
        public void NullableExisting_RegularExistingShouldStillWork()
        {
            // Regular existing (without ?) should still work the same way
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: 'testStorage'
}

output accountId string = storageAccount.id
");
            using (new AssertionScope())
            {
                // Regular existing should still be non-nullable
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Regular existing should NOT have nullableExisting in @options
                template.Should().NotHaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting");
                // Verify the output expression for regular existing resource
                template.Should().HaveValueAtPath("$.outputs['accountId'].value", "[resourceId('Microsoft.Storage/storageAccounts', 'testStorage')]");
            }
        }

        [TestMethod]
        public void NullableExisting_NullComparisonShouldWork()
        {
            // Null comparison should work with nullable existing resources
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

output exists bool = storageAccount != null
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Verify nullableExisting is added to @options
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify the output expression for null comparison
                template.Should().HaveValueAtPath("$.outputs['exists'].value", "[not(equals(reference('storageAccount', '2021-04-01', 'full'), null()))]");
            }
        }

        [TestMethod]
        public void NullableExisting_CannotBeUsedWithNewResource()
        {
            // The ? without existing should produce a parse error
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' ? = {
  name: 'testStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().ContainDiagnostic("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location.");
            }
        }

        [TestMethod]
        public void NullableExisting_PropertyAccess_RequiresSafeAccessForRuntimeProperties()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

output location string = storageAccount.location
");
            using (new AssertionScope())
            {
                // Direct property access on nullable resource should produce a warning but still compile
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP318", DiagnosticLevel.Warning, "The value of type \"Microsoft.Storage/storageAccounts | null\" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail.")
                });
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Verify nullableExisting is added to @options
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify the output expression for direct property access (no safe access)
                template.Should().HaveValueAtPath("$.outputs['location'].value", "[reference('storageAccount', '2021-04-01', 'full').location]");
            }
        }

        [TestMethod]
        public void NullableExisting_PropertyAccess_WithSafeAccess_ShouldWork()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

output location string? = storageAccount.?location
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                // Verify nullableExisting is added to @options
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify the output expression uses tryGet for safe access to location
                template.Should().HaveValueAtPath("$.outputs['location'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'location')]");
            }
        }

        [TestMethod]
        public void NullableExisting_SafeAccessAndNullCoalescing_OnRuntimeProperties()
        {
            // Test safe access (.?) and null coalescing (??) on runtime properties that would produce warnings without them
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

// Safe access on runtime properties - these would produce BCP318 warnings without .?
output safeLocation string? = storageAccount.?location
output safeSkuName string? = storageAccount.?sku.name
output safeSkuTier string? = storageAccount.?sku.tier
output safeAccessTier string? = storageAccount.?properties.accessTier
output safePrimaryEndpoints object? = storageAccount.?properties.primaryEndpoints

// Null coalescing with safe access - provides fallback values
output locationWithDefault string = storageAccount.?location ?? 'westus'
output skuNameWithDefault string = storageAccount.?sku.name ?? 'Standard_LRS'
output accessTierWithDefault string = storageAccount.?properties.accessTier ?? 'Hot'
");
            using (new AssertionScope())
            {
                // No warnings when using safe access operators
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());

                // Verify safe access outputs use tryGet
                template.Should().HaveValueAtPath("$.outputs['safeLocation'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'location')]");
                template.Should().HaveValueAtPath("$.outputs['safeSkuName'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'sku', 'name')]");
                template.Should().HaveValueAtPath("$.outputs['safeSkuTier'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'sku', 'tier')]");
                template.Should().HaveValueAtPath("$.outputs['safeAccessTier'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'properties', 'accessTier')]");
                template.Should().HaveValueAtPath("$.outputs['safePrimaryEndpoints'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'properties', 'primaryEndpoints')]");

                // Verify null coalescing outputs use coalesce with tryGet
                template.Should().HaveValueAtPath("$.outputs['locationWithDefault'].value", "[coalesce(tryGet(reference('storageAccount', '2021-04-01', 'full'), 'location'), 'westus')]");
                template.Should().HaveValueAtPath("$.outputs['skuNameWithDefault'].value", "[coalesce(tryGet(reference('storageAccount', '2021-04-01', 'full'), 'sku', 'name'), 'Standard_LRS')]");
                template.Should().HaveValueAtPath("$.outputs['accessTierWithDefault'].value", "[coalesce(tryGet(reference('storageAccount', '2021-04-01', 'full'), 'properties', 'accessTier'), 'Hot')]");
            }
        }

        [TestMethod]
        public void NullableExisting_DependsOn_ShouldBePopulated()
        {
            // Test that dependsOn is correctly populated when a new resource references a nullable existing resource
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource existingStorage 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'existingStorageAccount'
}

resource newStorage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'newStorageAccount'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: existingStorage.?properties.accessTier
  }
}
");
            using (new AssertionScope())
            {
                // Should compile without errors (warning is acceptable for the coalesce pattern)
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();

                // Verify the nullable existing resource has the correct options
                template.Should().HaveValueAtPath("$.resources['existingStorage'].existing", true);
                template.Should().HaveValueAtPath("$.resources['existingStorage']['@options'].nullableExisting", new JArray());

                // Verify the new resource has dependsOn referencing the nullable existing resource
                template.Should().HaveValueAtPath("$.resources['newStorage'].dependsOn[0]", "existingStorage");

                // Verify expression
                template.Should().HaveValueAtPath("$.resources['newStorage'].properties.accessTier", "[tryGet(reference('existingStorage', '2021-04-01', 'full'), 'properties', 'accessTier')]");
            }
        }

        [TestMethod]
        public void NullableExisting_ConditionalListKeys_WithNullCheck()
        {
            // Test using null check with ternary to conditionally call listKeys on a nullable existing resource
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
}

output keys object = storageAccount != null ? storageAccount!.listKeys() : {}
output firstKey string = storageAccount != null ? storageAccount!.listKeys().keys[0].value : ''
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());

                // Verify the conditional listKeys output expression
                template.Should().HaveValueAtPath("$.outputs['keys'].value", "[if(not(equals(reference('storageAccount', '2021-04-01', 'full'), null())), listKeys('storageAccount', '2021-04-01'), createObject())]");
                template.Should().HaveValueAtPath("$.outputs['firstKey'].value", "[if(not(equals(reference('storageAccount', '2021-04-01', 'full'), null())), listKeys('storageAccount', '2021-04-01').keys[0].value, '')]");
            }
        }

        [TestMethod]
        public void NullableExisting_WithForLoop_ShouldCompile()
        {
            // Test that nullable existing resources work with for loops
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
param storageNames array = ['storage1', 'storage2', 'storage3']

resource storageAccounts 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = [for name in storageNames: {
  name: name
}]

// Access a specific item by index and use safe access for nullable type
output firstAccountId string? = storageAccounts[0].?id
output secondAccountLocation string? = storageAccounts[1].?location
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                // Verify the resource is marked as existing with nullable option
                template.Should().HaveValueAtPath("$.resources['storageAccounts'].existing", true);
                template.Should().HaveValueAtPath("$.resources['storageAccounts']['@options'].nullableExisting", new JArray());
                // Verify copy loop is present
                template.Should().HaveValueAtPath("$.resources['storageAccounts'].copy.name", "storageAccounts");
            }
        }

        [TestMethod]
        public void NullableExisting_AsParentForChildResource_Compiles()
        {
            // Test using a nullable existing resource (key vault) as parent for a child resource (secret)
            // Note: The parent property accepts the nullable resource and compiles successfully.
            // The deployment may fail at runtime if the key vault doesn't exist.
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing? = {
  name: 'myKeyVault'
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'mySecret'
  properties: {
    value: 'secretValue'
  }
}

output secretId string = secret.id
");
            using (new AssertionScope())
            {
                // The parent property accepts the nullable resource without warning
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                // Verify the nullable existing key vault
                template.Should().HaveValueAtPath("$.resources['keyVault'].existing", true);
                template.Should().HaveValueAtPath("$.resources['keyVault']['@options'].nullableExisting", new JArray());
                // Verify the secret references the key vault as parent
                template.Should().HaveValueAtPath("$.resources['secret'].name", "[format('{0}/{1}', 'myKeyVault', 'mySecret')]");
            }
        }

        [TestMethod]
        public void NullableExisting_WithScope_ShouldCompile()
        {
            // Test that nullable existing resources work with scope property
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, NullableExistingEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
param subscriptionId string
param resourceGroupName string

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' existing = {
  name: resourceGroupName
  scope: subscription(subscriptionId)
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' existing? = {
  name: 'testStorage'
  scope: rg
}

output accountId string = storageAccount.id
output accountLocation string? = storageAccount.?location
");
            using (new AssertionScope())
            {
                // Should compile without warnings
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull();
                // Verify the nullable existing resource has correct options
                template.Should().HaveValueAtPath("$.resources['storageAccount'].existing", true);
                template.Should().HaveValueAtPath("$.resources['storageAccount']['@options'].nullableExisting", new JArray());
                // Verify subscriptionId and resourceGroup are correctly set (not a scope property)
                template.Should().HaveValueAtPath("$.resources['storageAccount'].subscriptionId", "[parameters('subscriptionId')]");
                template.Should().HaveValueAtPath("$.resources['storageAccount'].resourceGroup", "[parameters('resourceGroupName')]");
                // Verify output expressions (note the leading / in the format string for extensionResourceId)
                template.Should().HaveValueAtPath("$.outputs['accountId'].value", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', parameters('subscriptionId'), parameters('resourceGroupName')), 'Microsoft.Storage/storageAccounts', 'testStorage')]");
                template.Should().HaveValueAtPath("$.outputs['accountLocation'].value", "[tryGet(reference('storageAccount', '2021-04-01', 'full'), 'location')]");
            }
        }
    }
}
