// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Mocks;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Samples;

public static class MockRegistry
{
    private record MockRegistryCatalog(
        ImmutableDictionary<string, string> modules
    );

    public static async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager()
    {
        var manager = new TestExternalArtifactManager();

        manager.UpsertTemplateSpecs(CreateDefaultMockTemplateSpecs());
        await manager.PublishRegistryModules(CreateDefaultMockModules());
        await manager.PublishExtensions(CreateDefaultMockExtensions());

        return manager;
    }

    public static IEnumerable<TestExternalArtifactManager.RegistryModulePublishArguments> CreateDefaultMockModules()
    {
        var registryFiles = EmbeddedFile.LoadAll(typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly, "mockregistry", _ => true).ToArray();
        var index = registryFiles.First(x => x.StreamPath == "Files/mockregistry/index.json").Contents.FromJson<MockRegistryCatalog>();

        foreach (var (registryPath, filePath) in index.modules.Where(x => x.Key.StartsWith(OciArtifactReferenceFacts.SchemeWithColon)))
        {
            var sourceFile = registryFiles.First(x => x.StreamPath == $"Files/mockregistry/{filePath}");

            yield return new TestExternalArtifactManager.RegistryModulePublishArguments(registryPath, sourceFile.Contents);
        }
    }

    public static IEnumerable<MockTemplateSpecData> CreateDefaultMockTemplateSpecs()
    {
        var registryFiles = EmbeddedFile.LoadAll(typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly, "mockregistry", _ => true).ToArray();
        var index = registryFiles.First(x => x.StreamPath == "Files/mockregistry/index.json").Contents.FromJson<MockRegistryCatalog>();

        foreach (var (registryPath, filePath) in index.modules.Where(x => x.Key.StartsWith("ts:")))
        {
            var sourceFile = registryFiles.First(x => x.StreamPath == $"Files/mockregistry/{filePath}");

            var compilationResult = CompilationHelper.Compile(sourceFile.Contents);
            compilationResult.Template.Should().NotBeNull();

            var referenceStr = registryPath[3..];

            if (!TemplateSpecModuleReference.TryParse(BicepTestConstants.DummyBicepFile, null, referenceStr).IsSuccess(out var specReference, out var error))
            {
                throw new InvalidOperationException($"Failed to parse template spec reference from {registryPath} ({filePath}): {error(DiagnosticBuilder.ForDocumentStart())}");
            }

            var templateSpec = new JObject
            {
                ["id"] = specReference.TemplateSpecResourceId,
                ["properties"] = new JObject
                {
                    ["mainTemplate"] = compilationResult.Template,
                },
            };

            yield return new(
                Id: specReference.TemplateSpecResourceId,
                Content: templateSpec.ToString());
        }
    }

    public static IEnumerable<MockExtensionData> CreateDefaultMockExtensions()
    {
        yield return MockExtensionFactory.CreateMockExtWithNoConfigType("noconfig");
        yield return MockExtensionFactory.CreateMockExtWithObjectConfigType("hasconfig");
        yield return MockExtensionFactory.CreateMockExtWithSecureConfigType("hassecureconfig");
        yield return MockExtensionFactory.CreateMockExtWithDiscriminatedConfigType("hasdiscrimconfig");
    }
}
