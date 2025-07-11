// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.TypeSystem.Types;
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
        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithExtensions => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true))
            .WithConfigurationPatch(c => c.WithExtensions("""
            {
              "az": "builtin:",
              "kubernetes": "builtin:",
              "foo": "builtin:",
              "bar": "builtin:"
            }
            """))
            .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults());

        [TestMethod]
        public void Output_can_have_inferred_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped,
            """
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
""");

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
            var result = CompilationHelper.Compile(ServicesWithResourceTyped,
            """
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
""");

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
            var result = CompilationHelper.Compile(services,
            """
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
""");

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
            var result = CompilationHelper.Compile(ServicesWithResourceTyped,
            """
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
""");

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
            var result = CompilationHelper.Compile(ServicesWithResourceTyped,
            """
resource resource 'Some.Fake/Type@2019-06-01' = {
  name: 'test'
}
output out resource 'Some.Fake/Type@2019-06-01' = resource
""");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                // There are two warnings because there are two places in code to correct the missing type.
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available. Either the type or version does not exist, or Bicep does not yet know about it. This does not prevent attempting deployment, but does mean Bicep may not be able to provide IntelliSense or detect potential errors."),
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available. Either the type or version does not exist, or Bicep does not yet know about it. This does not prevent attempting deployment, but does mean Bicep may not be able to provide IntelliSense or detect potential errors."),
            });
        }

        [TestMethod]
        public void Output_can_have_warnings_for_missing_type_but_we_dont_duplicate_them_when_type_is_inferred()
        {
            // As a special case we don't show a warning on the output when the type is inferred
            // the user only has one location in code to correct.
            var result = CompilationHelper.Compile(ServicesWithResourceTyped,
            """
resource resource 'Some.Fake/Type@2019-06-01' = {
  name: 'test'
}
output out resource = resource
""");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available. Either the type or version does not exist, or Bicep does not yet know about it. This does not prevent attempting deployment, but does mean Bicep may not be able to provide IntelliSense or detect potential errors."),
            });
        }

        [TestMethod]
        public void Output_cannot_use_extension_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithExtensions,
            """
            extension bar with {
                connectionString: 'asdf'
            } as stg

            resource container 'stg:container' = {
                name: 'myblob'
            }

            output out resource = container
            """);

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP227", DiagnosticLevel.Error, "The type \"container\" cannot be used as a parameter or output type. Resource types from extensions are currently not supported as parameters or outputs."),
            });
        }
    }
}
