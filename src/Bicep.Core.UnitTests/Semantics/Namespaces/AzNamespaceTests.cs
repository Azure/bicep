// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class AzNamespaceTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DataRow("toLogicalZone")]
        [DataRow("toLogicalZones")]
        [DataRow("toPhysicalZone")]
        [DataRow("toPhysicalZones")]
        public void ZoneFunctions_ShouldExistAndRequireInlining(string functionName)
        {
            VerifyFunctionProperties(functionName, function =>
            {
                function.Name.Should().Be(functionName);
                function.Overloads.Should().HaveCount(1, $"Function '{functionName}' should have exactly one overload");
                function.Overloads[0].Flags.HasFlag(FunctionFlags.RequiresInlining).Should().BeTrue(
                    $"Function '{functionName}' should have the RequiresInlining flag set");
            });
        }

        [TestMethod]
        public void DeployerFunctionReturnType_ShouldHaveExpectedProperties()
        {
            var functionName = "deployer";
            VerifyFunctionProperties(functionName, function =>
            {
                function.Name.Should().Be(functionName);
                function.Overloads.Should().HaveCount(1, $"Function '{functionName}' should have exactly one overload");

                var overload = function.Overloads[0];
                overload.TypeSignatureSymbol.Should().BeOfType<ObjectType>();

                var overloadType = (ObjectType)overload.TypeSignatureSymbol;
                overloadType.Properties.Should().HaveCount(3, $"The return type for function '{functionName}' should have exactly three properties");

                overloadType.Properties.Should().ContainKey("objectId").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'objectId' property should be of type string");
                overloadType.Properties.Should().ContainKey("tenantId").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'tenantId' property should be of type string");
                overloadType.Properties.Should().ContainKey("userPrincipalName").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'userPrincipalName' property should be of type string");
            });
        }

        [TestMethod]
        public void ThisFunction_ShouldOnlyWorkInResourcePropertiesWhenFeatureEnabled()
        {
            // Test that this() function fails outside of resource properties when feature is enabled
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true)),
                @"
                output result object = this()
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP428", DiagnosticLevel.Error, "The \"this()\" function can only be used within resource property expressions.")
            });

            // Test that this() function works inside resource properties when feature is enabled
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
                    allowBlobPublicAccess: this().exists
                  }
                }
                ");
            // Should not have the BCP428 error when used inside resource properties
            result2.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void ThisFunction_ShouldNotBeRecognizedWhenFeatureDisabled()
        {
            // Test that this() function is not recognized when feature is disabled
            var result = CompilationHelper.Compile(@"
                resource test 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'test'
                  location: 'westus'
                  sku: {
                    name: 'Standard_LRS'
                  }
                  kind: 'StorageV2'
                  properties: {
                    customProperty: this().id
                  }
                }
                ");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP057", DiagnosticLevel.Error, "The name \"this\" does not exist in the current context.")
            });
        }

        [TestMethod]
        public void ThisFunction_CompilationGeneratesCorrectArmFunctions()
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
    allowBlobPublicAccess: this().exists
  }
}
");

            result.Should().NotHaveAnyDiagnostics();

            using (new AssertionScope())
            {
                var template = result.Template;

                // Check that this().exists is compiled to targetExists()
                template.Should().HaveValueAtPath("$.resources.testResource.properties.allowBlobPublicAccess", "[targetExists()]");
            }
        }

        [TestMethod]
        public void ThisFunction_InTopLevelResourceProperties_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, ThisExistsFunctionEnabled: true));
            var result = CompilationHelper.Compile(services, @"
resource testResource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: this().exists ? 'testStorage' : 'defaultStorage'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: this().exists
  }
}
");

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Storage/storageAccounts\" type, which requires a value that can be calculated at the start of the deployment.")
            });
        }


        [TestMethod]
        public void ThisFunction_InModuleParameters_ShouldFail()
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
    storageExists: this().exists
  }
}
"), ("storage.bicep", @"
param storageExists bool
output sto bool = storageExists
"));

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP428", DiagnosticLevel.Error, "The \"this()\" function can only be used within resource property expressions.")
            });
        }

        [TestMethod]
        public void ThisFunction_InNestedAndResourceProperty_ShouldSucceed()
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
  properties: this().exists ? {
    allowBlobPublicAccess: true
    encryption: {
      keySource: 'Microsoft.KeyVault'
      services: {
        blob: {
          enabled: this().exists
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
        public void ThisFunction_MultiplePropertiesInSameResource_ShouldSucceed()
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
    allowBlobPublicAccess: this().exists
    supportsHttpsTrafficOnly: this().exists
    minimumTlsVersion: this().exists ? 'TLS1_2' : 'TLS1_0'
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

        private static void VerifyFunctionProperties(string functionName, Action<FunctionSymbol> assertion)
        {
            var azNamespaceType = TestTypeHelper.GetBuiltInNamespaceType("az");
            var functions = azNamespaceType.MethodResolver.GetKnownFunctions()
                .Where(f => f.Key == functionName)
                .ToList();

            functions.Should().HaveCount(1, $"Function '{functionName}' should exist exactly once in the 'az' namespace");
            assertion(functions[0].Value);
        }
    }
}
