// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Utils.Snapshots;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class SnapshotResourceProvenanceTests
    {
        private const string EmptyParametersJson = """
            { "contentVersion": "1.0.0.0", "parameters": {} }
            """;

        private const string StorageProperties = """
              location: 'eastus'
              sku: { name: 'Standard_LRS' }
              kind: 'StorageV2'
            """;

        private const string HttpExtensionArtifact = "example.azurecr.io/extensions/http:1.2.3";

        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [TestMethod]
        public async Task AnalysisResources_ReturnCustomerJsonInSnapshotOrder()
        {
            var model = Services.BuildCompilation($$"""
                resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'provstorage'
                {{StorageProperties}}
                  tags: {
                    created: '2023-06-01T12:00:00+01:23'
                  }
                }
                """).GetEntrypointSemanticModel();
            var expansion = await ExpandAsync(model);

            var resources = SnapshotHelper.CreateAnalysisResources(expansion);
            var snapshot = SnapshotHelper.CreateSnapshot(expansion);

            resources.Select(resource => resource.Resource.GetRawText())
                .Should().Equal(snapshot.PredictedResources.Select(resource => resource.GetRawText()));
            var single = resources.Should().ContainSingle().Subject;
            single.Resource.TryGetProperty("bicepSymbolicName", out _).Should().BeFalse();
            single.Resource.GetProperty("tags").GetProperty("created").GetString().Should().Be("2023-06-01T12:00:00+01:23");
            single.Origin.SymbolicNamePath.Should().Equal("stg");
            single.Origin.TemplatePath.Should().Be("resources.stg");
            single.IsExtensible.Should().BeFalse();
        }

        [TestMethod]
        public async Task CustomerSnapshot_CarriesNoAnalyzerFields()
        {
            var model = Services.BuildCompilation($$"""
                resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = [for index in range(0, 2): {
                  name: 'storage${index}'
                {{StorageProperties}}
                }]
                """).GetEntrypointSemanticModel();
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames()!;

            var snapshot = await SnapshotHelper.GetSnapshot(
                model.TargetScope,
                templateJson,
                EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            snapshot.PredictedResources.Should().HaveCount(2);
            Serialize(snapshot).Should().NotContain("bicepSymbolicName").And.NotContain("symbolicNamePath");
        }

        [TestMethod]
        public async Task AnalysisResources_KeepRootLoopInstanceIdentities()
        {
            var model = Services.BuildCompilation($$"""
                resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = [for index in range(0, 2): {
                  name: 'storage${index}'
                {{StorageProperties}}
                }]
                """).GetEntrypointSemanticModel();

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            resources.Select(resource => Format(resource.Origin.SymbolicNamePath))
                .Should().BeEquivalentTo(["stg[0]", "stg[1]"]);
            resources.Should().OnlyContain(resource => resource.Origin.TemplatePath == "resources.stg");
        }

        [TestMethod]
        public async Task AnalysisResources_KeepNestedChildChains()
        {
            var model = Services.BuildCompilation("""
                resource vnet 'Microsoft.Network/virtualNetworks@2023-05-01' = {
                  name: 'vnet'
                  location: 'eastus'

                  resource subnet 'subnets' = {
                    name: 'sub'
                  }
                }

                resource topSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-05-01' = {
                  parent: vnet
                  name: 'sub2'
                }
                """).GetEntrypointSemanticModel();

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            resources.Select(resource => Format(resource.Origin.SymbolicNamePath))
                .Should().BeEquivalentTo(["vnet", "vnet::subnet", "topSubnet"]);
        }

        [TestMethod]
        public async Task AnalysisResources_KeepModuleChainsPerInstance()
        {
            var model = await CompileAsync(
                ("main.bicep", $$"""
                    resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'rootstorage'
                    {{StorageProperties}}
                    }

                    module child './child.bicep' = [for index in range(0, 2): {
                      name: 'child${index}'
                    }]
                    """),
                ("child.bicep", $$"""
                    resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'child${uniqueString(deployment().name)}'
                    {{StorageProperties}}
                    }
                    """));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            resources.Select(resource => Format(resource.Origin.SymbolicNamePath))
                .Should().BeEquivalentTo(["stg", "child[0]/stg", "child[1]/stg"]);
            resources.Where(resource => resource.Origin.SymbolicNamePath.Length == 2)
                .Should().OnlyContain(resource => resource.Origin.TemplatePath == "resources.child.properties.template.resources.stg");
        }

        [TestMethod]
        public async Task AnalysisResources_KeepTemplatePathWhereModuleTemplatesLackSymbolicNames()
        {
            var model = await CompileAsync(
                ("main.bicep", """
                    module composite './composite.json' = {
                      name: 'composite'
                    }
                    """),
                ("composite.json", """
                    {
                      "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                      "contentVersion": "1.0.0.0",
                      "resources": [
                        {
                          "type": "Microsoft.Storage/storageAccounts",
                          "apiVersion": "2023-01-01",
                          "name": "direct",
                          "location": "eastus",
                          "sku": { "name": "Standard_LRS" },
                          "kind": "StorageV2"
                        },
                        {
                          "type": "Microsoft.Resources/deployments",
                          "apiVersion": "2022-09-01",
                          "name": "inner",
                          "properties": {
                            "mode": "Incremental",
                            "expressionEvaluationOptions": { "scope": "inner" },
                            "template": {
                              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                              "contentVersion": "1.0.0.0",
                              "resources": [
                                {
                                  "type": "Microsoft.Storage/storageAccounts",
                                  "apiVersion": "2023-01-01",
                                  "name": "grandchild",
                                  "location": "eastus",
                                  "sku": { "name": "Standard_LRS" },
                                  "kind": "StorageV2"
                                }
                              ]
                            }
                          }
                        }
                      ]
                    }
                    """));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            var direct = resources.Single(resource => resource.Resource.GetProperty("name").GetString() == "direct");
            direct.Origin.SymbolicNamePath.Should().Equal("composite", null);
            direct.Origin.TemplatePath.Should().Be("resources.composite.properties.template.resources[0]");

            // The engine drops every ancestor above a deployment without a symbolic name; the chain is padded to the depth.
            var grandchild = resources.Single(resource => resource.Resource.GetProperty("name").GetString() == "grandchild");
            grandchild.Origin.SymbolicNamePath.Should().Equal([null, null, null]);
            grandchild.Origin.TemplatePath.Should().Be(
                "resources.composite.properties.template.resources[1].properties.template.resources[0]");
        }

        [TestMethod]
        public async Task AnalysisResources_KeepNestedExtensibleChains()
        {
            var model = await CompileWithHttpExtensionAsync(
                ("main.bicep", """
                    module outer './outer.bicep' = [for index in range(0, 2): {
                      name: 'outer-${index}'
                    }]
                    """),
                ("outer.bicep", """
                    module inner './inner.bicep' = {
                      name: 'inner'
                    }
                    """),
                ("inner.bicep", """
                    extension http

                    resource request 'request@v1' = [for index in range(0, 2): {
                      uri: 'https://example.com/forecast/${index}'
                      format: 'raw'
                      method: '2023-06-01T12:00:00+01:23'
                    }]
                    """));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            resources.Should().HaveCount(4).And.OnlyContain(resource => resource.IsExtensible);
            resources.Select(resource => Format(resource.Origin.SymbolicNamePath)).Should().BeEquivalentTo([
                "outer[0]/inner/request[0]",
                "outer[0]/inner/request[1]",
                "outer[1]/inner/request[0]",
                "outer[1]/inner/request[1]",
            ]);
            resources.Should().OnlyContain(resource => resource.Origin.TemplatePath == null);
            resources.Select(resource => resource.Resource.TryGetProperty("bicepSymbolicName", out _))
                .Should().OnlyContain(found => !found);
            resources.Select(resource => resource.Resource.GetProperty("properties").GetProperty("method").GetString())
                .Should().OnlyContain(method => method == "2023-06-01T12:00:00+01:23");
        }

        [TestMethod]
        public async Task AnalysisResources_KeepRootExtensibleLoopInstances()
        {
            var model = await CompileWithHttpExtensionAsync(
                ("main.bicep", """
                    extension http

                    resource request 'request@v1' = [for index in range(0, 2): {
                      uri: 'https://example.com/${index}'
                      format: 'raw'
                      method: 'GET'
                    }]
                    """));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            resources.Select(resource => Format(resource.Origin.SymbolicNamePath))
                .Should().BeEquivalentTo(["request[0]", "request[1]"]);
            resources.Should().OnlyContain(resource => resource.IsExtensible);
        }

        [TestMethod]
        public async Task AnalysisResources_PadChainsTruncatedByDeploymentsWithoutSymbolicNames()
        {
            var model = await CompileAsync(
                ("main.bicep", $$"""
                    resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'rootstorage'
                    {{StorageProperties}}
                    }

                    module legacy './legacy.json' = {
                      name: 'legacy'
                    }
                    """),
                ("legacy.json", """
                    {
                      "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                      "contentVersion": "1.0.0.0",
                      "resources": [
                        {
                          "type": "Microsoft.Resources/deployments",
                          "apiVersion": "2022-09-01",
                          "name": "inner",
                          "properties": {
                            "mode": "Incremental",
                            "expressionEvaluationOptions": { "scope": "inner" },
                            "template": {
                              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                              "contentVersion": "1.0.0.0",
                              "languageVersion": "2.0",
                              "resources": {
                                "stg": {
                                  "type": "Microsoft.Storage/storageAccounts",
                                  "apiVersion": "2023-01-01",
                                  "name": "innerstorage",
                                  "location": "eastus",
                                  "sku": { "name": "Standard_LRS" },
                                  "kind": "StorageV2"
                                }
                              }
                            }
                          }
                        }
                      ]
                    }
                    """));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            var root = resources.Single(resource => resource.Resource.GetProperty("name").GetString() == "rootstorage");
            var inner = resources.Single(resource => resource.Resource.GetProperty("name").GetString() == "innerstorage");
            root.Origin.SymbolicNamePath.Should().Equal("stg");
            inner.Origin.TemplatePath.Should().Be("resources.legacy.properties.template.resources[0].properties.template.resources.stg");
            inner.Origin.SymbolicNamePath.Should().Equal(null, null, "stg");
            inner.Origin.Should().NotBe(root.Origin);
        }

        [TestMethod]
        public async Task AnalysisResources_MarkExtensibleChainsTruncatedByDeploymentsWithoutSymbolicNames()
        {
            var innerModel = await CompileWithHttpExtensionAsync(
                ("main.bicep", """
                    extension http

                    resource request 'request@v1' = {
                      uri: 'https://example.com/inner'
                      format: 'raw'
                      method: 'GET'
                    }
                    """));
            var innerTemplate = JObject.Parse(new TemplateEmitter(innerModel).EmitWithForcedSymbolicNames()!);
            var legacyTemplate = new JObject
            {
                ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                ["contentVersion"] = "1.0.0.0",
                ["resources"] = new JArray(new JObject
                {
                    ["type"] = "Microsoft.Resources/deployments",
                    ["apiVersion"] = "2022-09-01",
                    ["name"] = "inner",
                    ["properties"] = new JObject
                    {
                        ["mode"] = "Incremental",
                        ["expressionEvaluationOptions"] = new JObject { ["scope"] = "inner" },
                        ["template"] = innerTemplate,
                    },
                }),
            };
            var model = await CompileWithHttpExtensionAsync(
                ("main.bicep", """
                    extension http

                    resource request 'request@v1' = {
                      uri: 'https://example.com/root'
                      format: 'raw'
                      method: 'GET'
                    }

                    module legacy './legacy.json' = {
                      name: 'legacy'
                    }
                    """),
                ("legacy.json", legacyTemplate.ToString()));

            var resources = SnapshotHelper.CreateAnalysisResources(await ExpandAsync(model));

            var root = resources.Single(resource => GetUri(resource) == "https://example.com/root");
            var inner = resources.Single(resource => GetUri(resource) == "https://example.com/inner");
            root.Origin.SymbolicNamePath.Should().Equal("request");
            inner.Origin.SymbolicNamePath.Should().Equal(null, "request");
            inner.Origin.Should().NotBe(root.Origin);
        }

        [TestMethod]
        public async Task AnalysisResources_FollowSnapshotOrderAcrossResourceKinds()
        {
            var model = await CompileWithHttpExtensionAsync(
                ("main.bicep", $$"""
                    extension http

                    resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = [for index in range(0, 2): {
                      name: 'root${index}'
                    {{StorageProperties}}
                    }]

                    resource request 'request@v1' = [for index in range(0, 2): {
                      uri: 'https://example.com/root/${index}'
                      format: 'raw'
                      method: 'GET'
                    }]

                    module child './child.bicep' = [for index in range(0, 2): {
                      name: 'child${index}'
                    }]
                    """),
                ("child.bicep", $$"""
                    extension http

                    resource stg 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'child${uniqueString(deployment().name)}'
                    {{StorageProperties}}
                    }

                    resource request 'request@v1' = {
                      uri: 'https://example.com/child/${deployment().name}'
                      format: 'raw'
                      method: 'GET'
                    }
                    """));
            var expansion = await ExpandAsync(model);

            var resources = SnapshotHelper.CreateAnalysisResources(expansion);
            var snapshot = SnapshotHelper.CreateSnapshot(expansion);

            resources.Select(resource => resource.Resource.GetRawText())
                .Should().Equal(snapshot.PredictedResources.Select(resource => resource.GetRawText()));
            resources.Select(resource => resource.IsExtensible)
                .Should().Equal(snapshot.PredictedResources.Select(resource => !resource.TryGetProperty("id", out _)));
            resources.Count(resource => resource.IsExtensible).Should().Be(4);
            resources.Count(resource => !resource.IsExtensible).Should().Be(4);
            resources.Single(resource => GetName(resource) == "root1").Origin.SymbolicNamePath.Should().Equal("stg[1]");
            resources.Single(resource => GetUri(resource) == "https://example.com/root/1").Origin.SymbolicNamePath.Should().Equal("request[1]");
            resources.Where(resource => GetName(resource)?.StartsWith("child", StringComparison.Ordinal) is true)
                .Select(resource => Format(resource.Origin.SymbolicNamePath))
                .Should().BeEquivalentTo(["child[0]/stg", "child[1]/stg"]);
        }

        [TestMethod]
        public async Task AnalysisResources_CountNestingCaseInsensitively()
        {
            const string templateJson = """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                  "contentVersion": "1.0.0.0",
                  "resources": [
                    {
                      "type": "Microsoft.Resources/deployments",
                      "apiVersion": "2022-09-01",
                      "name": "inner",
                      "Properties": {
                        "mode": "Incremental",
                        "expressionEvaluationOptions": { "scope": "inner" },
                        "Template": {
                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                          "contentVersion": "1.0.0.0",
                          "languageVersion": "2.0",
                          "resources": {
                            "stg": {
                              "type": "Microsoft.Storage/storageAccounts",
                              "apiVersion": "2023-01-01",
                              "name": "innerstorage",
                              "location": "eastus",
                              "sku": { "name": "Standard_LRS" },
                              "kind": "StorageV2"
                            }
                          }
                        }
                      }
                    }
                  ]
                }
                """;

            var expansion = await SnapshotHelper.ExpandAsync(
                ResourceScope.ResourceGroup,
                templateJson,
                EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            var inner = SnapshotHelper.CreateAnalysisResources(expansion).Should().ContainSingle().Subject;
            inner.Origin.TemplatePath.Should().ContainEquivalentOf(".properties.template.").And.NotContain(".properties.template.");
            inner.Origin.SymbolicNamePath.Should().Equal(null, "stg");
        }

        [TestMethod]
        public void SnapshotResourceOrigin_ComparesChainsByValue()
        {
            var origin = new SnapshotResourceOrigin(["child[0]", "stg"], "resources.child.properties.template.resources.stg");
            var same = new SnapshotResourceOrigin(["child[0]", "stg"], "resources.child.properties.template.resources.stg");
            var otherInstance = new SnapshotResourceOrigin(["child[1]", "stg"], "resources.child.properties.template.resources.stg");

            same.Should().Be(origin);
            same.GetHashCode().Should().Be(origin.GetHashCode());
            otherInstance.Should().NotBe(origin);
            new SnapshotResourceOrigin([null, "stg"], null).Should().NotBe(new SnapshotResourceOrigin(["stg"], null));

            var defaultPath = new SnapshotResourceOrigin(default, null);
            defaultPath.SymbolicNamePath.Should().BeEmpty();
            defaultPath.Should().Be(new SnapshotResourceOrigin([], null));
            defaultPath.GetHashCode().Should().Be(new SnapshotResourceOrigin([], null).GetHashCode());
            (defaultPath with { TemplatePath = "resources.stg" }).SymbolicNamePath.Should().BeEmpty();
        }

        private static async Task<SemanticModel> CompileAsync(params (string, TestFileData)[] files)
        {
            var fileSet = MockFileSystemTestFileSet.Create(files);
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            return compilation.GetEntrypointSemanticModel();
        }

        private static async Task<SemanticModel> CompileWithHttpExtensionAsync(params (string, TestFileData)[] files)
        {
            TestFileData bicepConfig = $$"""
                {
                  "extensions": {
                    "http": "br:{{HttpExtensionArtifact}}"
                  }
                }
                """;
            var fileSet = MockFileSystemTestFileSet.Create([("bicepconfig.json", bicepConfig), .. files]);
            var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(),
                new FeatureProviderOverrides(),
                fileSet.FileSystem,
                HttpExtensionArtifact);
            var compiler = services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
            return model;
        }

        private static Task<SnapshotExpansion> ExpandAsync(SemanticModel model)
        {
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
            templateJson.Should().NotBeNull();

            return SnapshotHelper.ExpandAsync(
                model.TargetScope,
                templateJson!,
                EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: model.TargetScope == ResourceScope.ResourceGroup ? "rg" : null,
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);
        }

        private static string? GetName(SnapshotAnalysisResource resource) =>
            resource.Resource.TryGetProperty("name", out var name) ? name.GetString() : null;

        private static string? GetUri(SnapshotAnalysisResource resource) =>
            resource.Resource.TryGetProperty("properties", out var properties) && properties.TryGetProperty("uri", out var uri)
                ? uri.GetString()
                : null;

        private static string Format(ImmutableArray<string?> symbolicNamePath) =>
            string.Join("/", symbolicNamePath.Select(segment => segment ?? "<unknown>"));

        private static string Serialize(Snapshot snapshot) => JsonSerializer.Serialize(snapshot, SnapshotSerializationContext.FileSerializer.Snapshot);
    }
}
