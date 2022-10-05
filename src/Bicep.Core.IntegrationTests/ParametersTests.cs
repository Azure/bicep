// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Syntax;
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
    public class ParameterTests
    {
        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder().WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, resourceTypedParamsAndOutputsEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithExtensibility => new ServiceBuilder()
            .WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, importsEnabled: true, resourceTypedParamsAndOutputsEnabled: true))
            .WithNamespaceProvider(new TestExtensibilityNamespaceProvider(BicepTestConstants.AzResourceTypeLoader));

        [TestMethod]
        public void Parameter_can_have_resource_type()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

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
    accessTier: p.properties.accessTier
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            var parameterSymbol = model.Root.ParameterDeclarations.Should().ContainSingle().Subject;
            var typeInfo = model.GetTypeInfo(parameterSymbol.DeclaringSyntax);
            typeInfo.Should().BeOfType<ResourceType>().Which.TypeReference.FormatName().Should().BeEquivalentTo("Microsoft.Storage/storageAccounts@2019-06-01");

            var reference = model.FindReferences(parameterSymbol).OfType<VariableAccessSyntax>().Should().ContainSingle().Subject;
            model.GetDeclaredType(reference).Should().NotBeNull();
            model.GetTypeInfo(reference).Should().NotBeNull();

            result.Template.Should().HaveValueAtPath("$.resources[0].properties.accessTier", "[reference(parameters('p'), '2019-06-01').accessTier]");
            result.Template.Should().HaveValueAtPath("$.parameters.p", new JObject()
            {
                ["type"] = new JValue("string"),
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                },
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_can_have_decorators()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
@description('cool')
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

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
    accessTier: p.properties.accessTier
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.parameters.p", new JObject()
            {
                ["type"] = new JValue("string"),
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                    ["description"] = new JValue("cool"),
                },
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_can_have_properties_evaluated()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

output id string = p.id
output name string = p.name
output type string = p.type
output apiVersion string = p.apiVersion
output accessTier string = p.properties.accessTier
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.id", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[parameters('p')]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(parameters('p'), '/'))]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.type", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("Microsoft.Storage/storageAccounts"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.apiVersion", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("2019-06-01"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.accessTier", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[reference(parameters('p'), '2019-06-01').accessTier]"),
            });
        }

        [TestMethod]
        public void Parameter_can_have_warnings_for_missing_type()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Some.Fake/Type@2019-06-01'

output id string = p.id
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available."),
            });
        }

        [TestMethod]
        public void Parameter_cannot_use_extensibility_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithExtensibility, @"
import storage as stg {
  connectionString: 'asdf'
}

param container resource 'stg:container'
output name string = container.name // silence unused params warning
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP227", DiagnosticLevel.Error, "The type \"container\" cannot be used as a parameter type. Extensibility types are currently not supported as parameters or outputs."),
                ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"container\" is not valid."),
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_cannot_be_used_as_extension_scope()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'My.Rp/myResource@2020-01-01' = {
  scope: p
  name: 'resource'
}");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"My.Rp/myResource@2020-01-01\" does not have types available."),
                ("BCP229", DiagnosticLevel.Error, "The parameter \"p\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource."),
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_cannot_be_used_as_parent()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'Microsoft.Storage/storageAccounts/tableServices@2020-06-01' = {
  parent: p
  name: 'child1'
  properties: {
    cors: {
      corsRules: []
    }
  }
}");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new []
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Microsoft.Storage/storageAccounts/tableServices@2020-06-01\" does not have types available."),
                ("BCP229", DiagnosticLevel.Error, "The parameter \"p\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource."),
                ("BCP169", DiagnosticLevel.Error, "Expected resource name to contain 1 \"/\" character(s). The number of name segments must match the number of segments in the resource type."),
            });
        }
    }
}
