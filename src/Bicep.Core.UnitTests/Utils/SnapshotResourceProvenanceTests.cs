// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Utils.Snapshots;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class SnapshotResourceProvenanceTests
    {
        private const string TemplateJson = """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "languageVersion": "2.0",
              "resources": {
                "stg": {
                  "type": "Microsoft.Storage/storageAccounts",
                  "apiVersion": "2023-01-01",
                  "name": "provstorage",
                  "location": "eastus",
                  "kind": "StorageV2",
                  "sku": { "name": "Standard_LRS" }
                }
              }
            }
            """;

        private const string EmptyParametersJson = """
            { "contentVersion": "1.0.0.0", "parameters": {} }
            """;

        [TestMethod]
        public async Task Snapshot_ResourceProvenanceIsIndexAlignedAndNotSerialized()
        {
            var result = await GetSnapshotAsync();

            result.Snapshot.PredictedResources.Should().ContainSingle();
            var provenance = result.ResourceProvenance.Should().ContainSingle().Subject;
            provenance.SymbolicName.Should().Be("stg");
            provenance.Path.Should().Contain("resources.stg");
            provenance.IsExtensible.Should().BeFalse();

            Serialize(result.Snapshot).Should().NotContain("resourceProvenance");
        }

        [TestMethod]
        public void Snapshot_AnalyzerOnlyExtensibleResourceDoesNotChangeCustomerSerialization()
        {
            var customerResource = JsonDocument.Parse("""{"type":"example/customer"}""").RootElement.Clone();
            var analyzerResource = JsonDocument.Parse("""{"type":"example/customer","bicepSymbolicName":"extensionResource"}""").RootElement.Clone();
            var baseline = new Snapshot([customerResource], [], []);
            var enriched = new SnapshotAnalysisResult(
                baseline,
                [analyzerResource],
                [new SnapshotResourceProvenance("extensionResource", null, null, null, "resources.extensionResource", 1, 1, true)]);

            enriched.ExtensibleResources.Should().ContainSingle().Which
                .GetProperty("bicepSymbolicName").GetString().Should().Be("extensionResource");
            Serialize(enriched.Snapshot).Should().Be(Serialize(baseline));
        }

        [TestMethod]
        public void Snapshot_AnalyzerMetadataPreservesHighPrecisionNumbers()
        {
            var enriched = SnapshotHelper.AddStringProperty(
                """{"ratio":9007199254740993.0}""",
                "bicepSymbolicName",
                "request");

            enriched.GetProperty("ratio").GetRawText().Should().Be("9007199254740993.0");
            enriched.GetProperty("bicepSymbolicName").GetString().Should().Be("request");
        }

        [TestMethod]
        [DataRow("request", "request")]
        [DataRow("request[0]", "request")]
        [DataRow("request[123]", "request")]
        [DataRow("request[name]", "request[name]")]
        public void Snapshot_CopyIndexesAreRemovedOnlyFromExpandedNames(string symbolicName, string expected)
        {
            SnapshotHelper.RemoveCopyIndex(symbolicName).Should().Be(expected);
        }

        [TestMethod]
        public async Task Snapshot_NestedExtensibleResourceCarriesCanonicalTemplatePath()
        {
            const string extensionArtifact = "example.azurecr.io/extensions/http:1.2.3";
            var fileSet = MockFileSystemTestFileSet.Create(
                ("bicepconfig.json", $$"""
                    {
                      "extensions": {
                        "http": "br:{{extensionArtifact}}"
                      }
                    }
                    """),
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
                      method: '2023-06-01T12:00:00+05:00'
                    }]
                    """));
            var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(),
                new FeatureProviderOverrides(),
                fileSet.FileSystem,
                extensionArtifact);
            var compiler = services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var result = await SnapshotHelper.GetSnapshotForAnalysis(
                targetScope: model.TargetScope,
                templateContent: templateJson!,
                parametersContent: EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            result.ExtensibleResources.Should().HaveCount(4);
            result.ExtensibleResources
                .Select(resource => resource.TryGetProperty("bicepSymbolicName", out var ignored))
                .Should().OnlyContain(found => !found);
            result.ExtensibleResources.Should().OnlyContain(resource =>
                resource.GetProperty("properties").GetProperty("method").GetString() == "2023-06-01T12:00:00+05:00");
            var provenance = result.ResourceProvenance.Should().HaveCount(4).And.Subject;
            provenance.Should().OnlyContain(item => item.SymbolicName == null);
            provenance.Should().OnlyContain(item => item.CopyLoopSymbolicName == null);
            provenance.Should().OnlyContain(item =>
                item.Path == "resources.outer.properties.template.resources.inner.properties.template.resources.request");
            provenance.Should().OnlyContain(item => item.IsExtensible);
        }

        [TestMethod]
        public async Task Snapshot_CopiedRootExtensibleResourcesUseDeclarationIdentity()
        {
            const string extensionArtifact = "example.azurecr.io/extensions/http:1.2.3";
            var fileSet = MockFileSystemTestFileSet.Create(
                ("bicepconfig.json", $$"""
                    {
                      "extensions": {
                        "http": "br:{{extensionArtifact}}"
                      }
                    }
                    """),
                ("main.bicep", """
                    extension http

                    resource request 'request@v1' = [for index in range(0, 2): {
                      uri: 'https://example.com/${index}'
                      format: 'raw'
                      method: 'GET'
                    }]
                    """));
            var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(),
                new FeatureProviderOverrides(),
                fileSet.FileSystem,
                extensionArtifact);
            var compiler = services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
            var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var result = await SnapshotHelper.GetSnapshotForAnalysis(
                targetScope: model.TargetScope,
                templateContent: templateJson!,
                parametersContent: EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            result.ExtensibleResources.Should().HaveCount(2);
            result.ExtensibleResources
                .Select(resource => resource.GetProperty("bicepSymbolicName").GetString())
                .Should().BeEquivalentTo(["request[0]", "request[1]"]);
            var provenance = result.ResourceProvenance.Should().HaveCount(2).And.Subject;
            provenance.Select(item => item.SymbolicName).Should().BeEquivalentTo(["request[0]", "request[1]"]);
            provenance.Should().OnlyContain(item => item.CopyLoopSymbolicName == "request");
            provenance.Should().OnlyContain(item => item.Path == "resources.request");
            provenance.Select(item => item.CopyIndex).Should().BeEquivalentTo([0, 1]);
        }

        [TestMethod]
        public async Task Snapshot_CopiedRootPreflightResourcesKeepDistinctInstanceIdentities()
        {
            const string templateJson = """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                  "contentVersion": "1.0.0.0",
                  "languageVersion": "2.0",
                  "resources": {
                    "storage": {
                      "copy": {
                        "name": "storage",
                        "count": 2
                      },
                      "type": "Microsoft.Storage/storageAccounts",
                      "apiVersion": "2023-01-01",
                      "name": "[format('storage{0}', copyIndex())]",
                      "location": "eastus",
                      "kind": "StorageV2",
                      "sku": { "name": "Standard_LRS" }
                    }
                  }
                }
                """;

            var result = await SnapshotHelper.GetSnapshotForAnalysis(
                targetScope: ResourceScope.ResourceGroup,
                templateContent: templateJson,
                parametersContent: EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            var provenance = result.ResourceProvenance.Should().HaveCount(2).And.Subject;
            provenance.Select(item => item.SymbolicName).Should().BeEquivalentTo(["storage[0]", "storage[1]"]);
            provenance.Should().OnlyContain(item => item.CopyLoopSymbolicName == "storage");
            provenance.Select(item => item.CopyIndex).Should().BeEquivalentTo([0, 1]);
        }

        [TestMethod]
        public async Task Snapshot_NestedPreflightResourcesDoNotExposeRootResolvableSymbols()
        {
            const string templateJson = """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                  "contentVersion": "1.0.0.0",
                  "languageVersion": "2.0",
                  "resources": {
                    "stg": {
                      "type": "Microsoft.Storage/storageAccounts",
                      "apiVersion": "2023-01-01",
                      "name": "rootstorage",
                      "location": "eastus",
                      "kind": "StorageV2",
                      "sku": { "name": "Standard_LRS" }
                    },
                    "child": {
                      "type": "Microsoft.Resources/deployments",
                      "apiVersion": "2022-09-01",
                      "name": "child",
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
                              "name": "childstorage",
                              "location": "eastus",
                              "kind": "StorageV2",
                              "sku": { "name": "Standard_LRS" }
                            }
                          }
                        }
                      }
                    }
                  }
                }
                """;

            var result = await SnapshotHelper.GetSnapshotForAnalysis(
                targetScope: ResourceScope.ResourceGroup,
                templateContent: templateJson,
                parametersContent: EmptyParametersJson,
                tenantId: null,
                managementGroupId: null,
                subscriptionId: "00000000-0000-0000-0000-000000000000",
                resourceGroup: "rg",
                deploymentName: null,
                location: "eastus",
                externalInputs: [],
                cancellationToken: default);

            var storageProvenance = result.ResourceProvenance
                .Where(item => item.Path?.EndsWith("resources.stg", StringComparison.Ordinal) is true)
                .Should().HaveCount(2).And.Subject;
            storageProvenance.Should().ContainSingle(item => item.SymbolicName == "stg");
            storageProvenance.Should().ContainSingle(item => item.SymbolicName == null);
        }

        private static Task<SnapshotAnalysisResult> GetSnapshotAsync() => SnapshotHelper.GetSnapshotForAnalysis(
            targetScope: ResourceScope.ResourceGroup,
            templateContent: TemplateJson,
            parametersContent: EmptyParametersJson,
            tenantId: null,
            managementGroupId: null,
            subscriptionId: "00000000-0000-0000-0000-000000000000",
            resourceGroup: "rg",
            deploymentName: null,
            location: "eastus",
            externalInputs: [],
            cancellationToken: default);

        private static string Serialize(Snapshot snapshot) => JsonSerializer.Serialize(snapshot, SnapshotSerializationContext.FileSerializer.Snapshot);
    }
}