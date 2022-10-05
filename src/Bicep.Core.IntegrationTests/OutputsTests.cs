// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class OutputsTests
    {
        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder().WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, resourceTypedParamsAndOutputsEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithExtensibility => new ServiceBuilder()
            .WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, importsEnabled: true, resourceTypedParamsAndOutputsEnabled: true))
            .WithNamespaceProvider(new TestExtensibilityNamespaceProvider(BicepTestConstants.AzResourceTypeLoader));

        [TestMethod]
        public void Output_can_have_inferred_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: 'Cool'
  }
}
output out resource = resource
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            var @out = model.Root.OutputDeclarations.Should().ContainSingle().Subject;
            var typeInfo = model.GetTypeInfo(@out.DeclaringSyntax);
            typeInfo.Should().BeOfType<ResourceType>().Which.TypeReference.FormatName().Should().BeEquivalentTo("Microsoft.Storage/storageAccounts@2019-06-01");

            result.Template.Should().HaveValueAtPath("$.outputs.out", new JObject()
            {
                ["type"] = "string",
                ["value"] = "[resourceId('Microsoft.Storage/storageAccounts', 'test')]",
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                },
            });
        }

        [TestMethod]
        public void Output_can_have_specified_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: 'Cool'
  }
}
output out resource 'Microsoft.Storage/storageAccounts@2019-06-01' = resource
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            var @out = model.Root.OutputDeclarations.Should().ContainSingle().Subject;
            var typeInfo = model.GetTypeInfo(@out.DeclaringSyntax);
            typeInfo.Should().BeOfType<ResourceType>().Which.TypeReference.FormatName().Should().BeEquivalentTo("Microsoft.Storage/storageAccounts@2019-06-01");

            result.Template.Should().HaveValueAtPath("$.outputs.out", new JObject()
            {
                ["type"] = "string",
                ["value"] = "[resourceId('Microsoft.Storage/storageAccounts', 'test')]",
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                },
            });
        }

        // Object-typed outputs should work the same way regardless of whether resource-typed outputs are enabled.
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Output_can_have_object_type(bool enableResourceTypeParameters)
        {
            var services = enableResourceTypeParameters ? ServicesWithResourceTyped : new ServiceBuilder();
            var result = CompilationHelper.Compile(services, @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: 'Cool'
  }
}
output out object = resource
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.out", new JObject()
            {
                ["type"] = "object",
                ["value"] = "[reference(resourceId('Microsoft.Storage/storageAccounts', 'test'), '2019-06-01', 'full')]",
            });
        }

        [TestMethod]
        public void Output_can_have_decorators()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: 'Cool'
  }
}

@description('cool beans')
output out resource 'Microsoft.Storage/storageAccounts@2019-06-01' = resource
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.out", new JObject()
            {
                ["type"] = "string",
                ["value"] = "[resourceId('Microsoft.Storage/storageAccounts', 'test')]",
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                    ["description"] = new JValue("cool beans"),
                },
            });
        }

        [TestMethod]
        public void Output_can_have_warnings_for_missing_type()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
resource resource 'Some.Fake/Type@2019-06-01' = {
  name: 'test'
}
output out resource 'Some.Fake/Type@2019-06-01' = resource
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                // There are two warnings because there are two places in code to correct the missing type.
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available."),
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available."),
            });
        }

        [TestMethod]
        public void Output_can_have_warnings_for_missing_type_but_we_dont_duplicate_them_when_type_is_inferred()
        {
            // As a special case we don't show a warning on the output when the type is inferred
            // the user only has one location in code to correct.
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
resource resource 'Some.Fake/Type@2019-06-01' = {
  name: 'test'
}
output out resource = resource
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available."),
            });
        }

        [TestMethod]
        public void Output_cannot_use_extensibility_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithExtensibility, @"
import storage as stg {
  connectionString: 'asdf'
}

resource container 'stg:container' = {
  name: 'myblob'
}
output out resource = container
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP228", DiagnosticLevel.Error, "The type \"container\" cannot be used as an output type. Extensibility types are currently not supported as parameters or outputs."),
            });
        }
    }
}
