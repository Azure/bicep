// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Mocks;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Samples;

public static class MockRegistry
{
    private record MockRegistryCatalog(
        ImmutableDictionary<string, string> modules
    );

    public static async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager(TestContext testContext)
        => await CreateDefaultExternalArtifactManager(new FeatureProviderOverrides(testContext));

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

            if (!TryParseTemplateSpecReference(registryPath, out var specReference, out var errorDiagnostic))
            {
                throw new InvalidOperationException($"Failed to parse template spec reference from {registryPath} ({filePath}): {errorDiagnostic}");
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

    public static bool TryParseTemplateSpecReference(
        string referenceStr,
        [NotNullWhen(true)] out TemplateSpecModuleReference? reference,
        [NotNullWhen(false)] out IDiagnostic? diagnostic)
    {
        reference = null;
        diagnostic = null;

        if (referenceStr.StartsWith("ts:"))
        {
            referenceStr = referenceStr[3..];
        }

        if (!TemplateSpecModuleReference.TryParse(BicepTestConstants.DummyBicepFile, null, referenceStr).IsSuccess(out var specReference, out var errorBuilder))
        {
            diagnostic = errorBuilder(DiagnosticBuilder.ForDocumentStart());

            return false;
        }

        reference = specReference;

        return true;
    }

    public static MockTemplateSpecData ConvertExternalModuleInfoToMockTemplateSpecData(DataSet.ExternalModuleInfo templateSpecModule)
    {
        if (!MockRegistry.TryParseTemplateSpecReference(templateSpecModule.Metadata.Target, out var specReference, out var errorBuilder))
        {
            throw new InvalidOperationException($"Failed to parse template spec reference from {templateSpecModule.Metadata.Target}: {errorBuilder}");
        }

        return new MockTemplateSpecData(specReference.TemplateSpecResourceId, templateSpecModule.ModuleSource);
    }

    public static IEnumerable<MockExtensionData> CreateDefaultMockExtensions()
    {
        yield return MockExtensionFactory.CreateMockExtWithNoConfigType("noconfig");
        yield return MockExtensionFactory.CreateMockExtWithObjectConfigType("hasconfig");
        yield return MockExtensionFactory.CreateMockExtWithSecureConfigType("hassecureconfig");
        yield return MockExtensionFactory.CreateMockExtWithDiscriminatedConfigType("hasdiscrimconfig");
    }

    private static async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager(TestCompiler compiler)
    {
        var manager = new TestExternalArtifactManager(compiler);

        manager.UpsertTemplateSpecs(CreateDefaultMockTemplateSpecs());
        await manager.PublishRegistryModules(CreateDefaultMockModules());
        await manager.PublishExtensions(CreateDefaultMockExtensions());

        return manager;
    }

    private static async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager(FeatureProviderOverrides overrides)
        => await CreateDefaultExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides(overrides));
}
