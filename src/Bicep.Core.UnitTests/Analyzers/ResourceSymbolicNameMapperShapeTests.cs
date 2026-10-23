// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Analyzers
{
    [TestClass]
    public class ResourceSymbolicNameMapperShapeTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private const string TwoStorageBicep = """
            resource storageA 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'foo'
              location: 'eastus'
              sku: { name: 'Standard_LRS' }
              kind: 'StorageV2'
            }

            resource storageB 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'foobar'
              location: 'eastus'
              sku: { name: 'Standard_LRS' }
              kind: 'StorageV2'
              tags: {
                timestamp: '2023-06-01T12:00:00+05:00'
              }
            }
            """;

        [TestMethod]
        public void BuildMapping_ObjectShape_UsesResourceKeysAsSymbolicNames()
        {
            var model = Services.BuildCompilation(TwoStorageBicep).GetEntrypointSemanticModel();
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson!, model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JObject>().Subject;
            resources.Properties().Select(property => property.Name).Should().BeEquivalentTo(["storageA", "storageB"]);
            resources.Properties()
                .Select(property => property.Value[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo(["storageA", "storageB"]);
            result.SymbolicNameMap.Keys.Should().BeEquivalentTo(["storageA", "storageB"]);
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_UsesTypeAndNameHeuristic()
        {
            var model = Services.BuildCompilation(TwoStorageBicep).GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo(["storageA", "storageB"]);
            resources[1]!["tags"]!["timestamp"]!.ToString().Should().Be("2023-06-01T12:00:00+05:00");
            result.SymbolicNameMap.Keys.Should().BeEquivalentTo(["storageA", "storageB"]);
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_DoesNotUseLiteralSubstringInsideArmExpression()
        {
            var model = Services.BuildCompilation("""
                param foo string

                resource literalResource 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'foo'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                resource resource_1 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: foo
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """).GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo(["literalResource", null]);
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_DuplicateEmittedIdentityRemainsUnattributed()
        {
            var model = Services.BuildCompilation("""
                resource literalLock 'Microsoft.Authorization/locks@2020-05-01' = {
                  name: 'lock'
                  properties: {
                    level: 'CanNotDelete'
                  }
                }

                resource expressionLock 'Microsoft.Authorization/locks@2020-05-01' = {
                  name: any('lock')
                  properties: {
                    level: 'CanNotDelete'
                  }
                }
                """).GetEntrypointSemanticModel();
            const string templateJson = """
                {
                  "resources": [
                    {
                      "type": "Microsoft.Authorization/locks",
                      "name": "lock"
                    },
                    {
                      "type": "Microsoft.Authorization/locks",
                      "name": "lock"
                    }
                  ]
                }
                """;

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson, model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().Equal([null, null]);
        }

        [TestMethod]
        public async Task BuildMapping_ArrayShape_DoesNotMapModuleToExplicitDeploymentResource()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    resource explicitDeployment 'Microsoft.Resources/deployments@2022-09-01' = {
                      name: 'child'
                      properties: {
                        mode: 'Incremental'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: {}
                        }
                      }
                    }

                    module child './child.bicep' = {
                      name: 'child'
                      scope: resourceGroup('other-rg')
                    }
                    """),
                ("child.bicep", "metadata description = 'child'"));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo([null, null]);
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_DoesNotFallThroughAfterAmbiguousExactMatch()
        {
            var model = Services.BuildCompilation("""
                resource subA 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  name: 'vnet/sub'
                }

                resource subB 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  name: 'vnet/sub'
                }

                resource subC 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  name: 'sub'
                }
                """).GetEntrypointSemanticModel();
            const string templateJson = """
                {
                  "resources": [
                    {
                      "type": "Microsoft.Network/virtualNetworks/subnets",
                      "name": "vnet/sub"
                    }
                  ]
                }
                """;

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson, model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources[0]![ResourceSymbolicNameMapper.BicepSymbolicNameProperty].Should().BeNull();
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_DoesNotUseUnrelatedLiteralPathSegment()
        {
            var model = Services.BuildCompilation("""
                resource subA 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  name: 'sub'
                }

                resource subB 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  name: any('other/sub')
                }
                """).GetEntrypointSemanticModel();
            const string templateJson = """
                {
                  "resources": [
                    {
                      "type": "Microsoft.Network/virtualNetworks/subnets",
                      "name": "sub"
                    },
                    {
                      "type": "Microsoft.Network/virtualNetworks/subnets",
                      "name": "other/sub"
                    }
                  ]
                }
                """;

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson, model);

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo(["subA", null]);
        }

        [TestMethod]
        public void BuildMapping_PreservesCaseDistinctBicepSymbols()
        {
            var model = Services.BuildCompilation("""
                resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'foo'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                resource Storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'bar'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """).GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            result.Should().NotBeNull();
            result!.SymbolicNameMap.Should().HaveCount(2);
            result.SymbolicNameMap.Should().ContainKey("storage");
            result.SymbolicNameMap.Should().ContainKey("Storage");
        }

        [TestMethod]
        public async Task BuildMapping_ObjectShape_DoesNotStampUnmappedModuleKeys()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'deployed'
                      location: 'eastus'
                      sku: { name: 'Standard_LRS' }
                      kind: 'StorageV2'
                    }

                    module child './child.bicep' = {
                      name: 'child'
                    }
                    """),
                ("child.bicep", "metadata description = 'child'"));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson!, model);

            var resources = result!.EnrichedResources.Should().BeOfType<JObject>().Subject;
            resources["storage"]![ResourceSymbolicNameMapper.BicepSymbolicNameProperty]!.Value<string>()
                .Should().Be("storage");
            resources["child"]![ResourceSymbolicNameMapper.BicepSymbolicNameProperty].Should().BeNull();
        }

        [TestMethod]
        public void BuildMapping_PreservesHighPrecisionDecimalValues()
        {
            var model = Services.BuildCompilation(TwoStorageBicep).GetEntrypointSemanticModel();
            const string templateJson = """
                {
                  "resources": [
                    {
                      "type": "Microsoft.Storage/storageAccounts",
                      "name": "foo",
                      "properties": {
                        "ratio": 9007199254740993.0
                      }
                    }
                  ]
                }
                """;

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson, model);

            var resource = result!.EnrichedResources.Should().BeOfType<JArray>().Subject[0]!;
            resource["properties"]!["ratio"]!.ToString().Should().Be("9007199254740993.0");
        }

        [TestMethod]
        [DataRow("1e-29")]
        [DataRow("1e100")]
        public void BuildMapping_PreservesNumbersOutsideDecimalRange(string number)
        {
            var model = Services.BuildCompilation(TwoStorageBicep).GetEntrypointSemanticModel();
            var templateJson = $$"""
                {
                  "resources": [
                    {
                      "type": "Microsoft.Storage/storageAccounts",
                      "name": "foo",
                      "properties": {
                        "ratio": {{number}}
                      }
                    }
                  ]
                }
                """;

            var result = ResourceSymbolicNameMapper.BuildMapping(templateJson, model);

            var resource = result!.EnrichedResources.Should().BeOfType<JArray>().Subject[0]!;
            resource["properties"]!["ratio"]!.ToString().Should().Be(number);
        }

        [TestMethod]
        public void BuildMapping_ArrayShape_MapsSingleLiteralChildDeclaration()
        {
            var model = Services.BuildCompilation("""
                resource vnet 'Microsoft.Network/virtualNetworks@2023-05-01' = {
                  name: 'vnet'
                  location: 'eastus'

                  resource subnet 'subnets' = {
                    name: 'sub'
                  }
                }
                """).GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Single(resource => resource["type"]!.ToString().EndsWith("/subnets", StringComparison.OrdinalIgnoreCase))
                [ResourceSymbolicNameMapper.BicepSymbolicNameProperty]!.ToString()
                .Should().Be("vnet::subnet");
        }

        [TestMethod]
        public async Task BuildMapping_ArrayShape_MapsUniqueExplicitDeploymentAlongsideModule()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    resource explicitDeployment 'Microsoft.Resources/deployments@2022-09-01' = {
                      name: 'manual'
                      properties: {
                        mode: 'Incremental'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: {}
                        }
                      }
                    }

                    module child './child.bicep' = {
                      name: 'child'
                    }
                    """),
                ("child.bicep", "metadata description = 'child'"));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            var result = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);

            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Single(resource => resource["name"]!.ToString() == "manual")
                [ResourceSymbolicNameMapper.BicepSymbolicNameProperty]!.ToString()
                .Should().Be("explicitDeployment");
            resources.OfType<JObject>()
                .Single(resource => resource["name"]!.ToString() == "child")
                [ResourceSymbolicNameMapper.BicepSymbolicNameProperty].Should().BeNull();
        }

        [TestMethod]
        public void BuildMapping_CopiedResourcesRemainUnattributedOnlyInLegacyTemplateShape()
        {
            var model = Services.BuildCompilation("""
                resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = [for index in range(0, 2): {
                  name: 'storage${index}'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }]
                """).GetEntrypointSemanticModel();

            var arrayResult = ResourceSymbolicNameMapper.BuildMapping(EmitNormally(model), model);
            var objectResult = ResourceSymbolicNameMapper.BuildMapping(
                new TemplateEmitter(model).EmitWithForcedSymbolicNames()!,
                model);

            var copiedResource = arrayResult!.EnrichedResources.Should().BeOfType<JArray>().Subject
                .OfType<JObject>().Should().ContainSingle().Which;
            copiedResource["copy"].Should().NotBeNull();
            copiedResource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty].Should().BeNull();
            objectResult!.EnrichedResources.Should().BeOfType<JObject>().Subject["storage"]!
                [ResourceSymbolicNameMapper.BicepSymbolicNameProperty]!.ToString().Should().Be("storage");
        }

        private static string EmitNormally(SemanticModel model)
        {
            using var writer = new StringWriter { NewLine = "\n" };
            var result = new TemplateEmitter(model).Emit(writer);
            result.Status.Should().Be(EmitStatus.Succeeded);
            return writer.ToString();
        }
    }
}