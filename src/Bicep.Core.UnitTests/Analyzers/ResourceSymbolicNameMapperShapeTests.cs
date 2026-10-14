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
        public async Task BuildMapping_ArrayShape_DoesNotMapModuleToExplicitDeploymentResource()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    resource prior 'Microsoft.Resources/deployments@2022-09-01' existing = {
                      name: 'child'
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

            result.Should().NotBeNull();
            var resources = result!.EnrichedResources.Should().BeOfType<JArray>().Subject;
            resources.OfType<JObject>()
                .Select(resource => resource[ResourceSymbolicNameMapper.BicepSymbolicNameProperty]?.ToString())
                .Should().BeEquivalentTo([null]);
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

        private static string EmitNormally(SemanticModel model)
        {
            using var writer = new StringWriter { NewLine = "\n" };
            var result = new TemplateEmitter(model).Emit(writer);
            result.Status.Should().Be(EmitStatus.Succeeded);
            return writer.ToString();
        }
    }
}