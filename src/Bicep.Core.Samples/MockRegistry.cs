// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Samples;

public class MockRegistry
{
    private record MockRegistryIndex(
        ImmutableDictionary<string, string> modules);

    public record ClientFactories(
        IContainerRegistryClientFactory ContainerRegistry,
        ITemplateSpecRepositoryFactory TemplateSpec);

    public static async Task<ClientFactories> Build()
        => new(
            await CreateMockBicepRegistry(),
            CreateMockTemplateSpecRegistry());

    private static async Task<IContainerRegistryClientFactory> CreateMockBicepRegistry()
    {
        var registryFiles = EmbeddedFile.LoadAll(typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly, "mockregistry", _ => true);
        var index = registryFiles.First(x => x.StreamPath == "Files/mockregistry/index.json").Contents.FromJson<MockRegistryIndex>();

        var modules = new Dictionary<string, DataSet.ExternalModuleInfo>();
        foreach (var (registryPath, filePath) in index.modules.Where(x => x.Key.StartsWith("br:")))
        {
            var sourceFile = registryFiles.First(x => x.StreamPath == $"Files/mockregistry/{filePath}");

            modules[registryPath] = new(sourceFile.Contents, new(registryPath));
        }

        var clientFactory = DataSetsExtensions.CreateMockRegistryClients(modules.ToImmutableDictionary()).Object;
        await DataSetsExtensions.PublishModulesToRegistryAsync(modules.ToImmutableDictionary(), clientFactory);

        return clientFactory;
    }

    private static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRegistry()
    {
        var registryFiles = EmbeddedFile.LoadAll(typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly, "mockregistry", _ => true);
        var index = registryFiles.First(x => x.StreamPath == "Files/mockregistry/index.json").Contents.FromJson<MockRegistryIndex>();

        var modules = new Dictionary<string, DataSet.ExternalModuleInfo>();
        foreach (var (registryPath, filePath) in index.modules.Where(x => x.Key.StartsWith("ts:")))
        {
            var sourceFile = registryFiles.First(x => x.StreamPath == $"Files/mockregistry/{filePath}");

            var compilationResult = CompilationHelper.Compile(sourceFile.Contents);
            compilationResult.Template.Should().NotBeNull();

            var templateSpec = new JObject {
                ["id"] = "/subscriptions/<todo_fill_in>/resourceGroups/<todo_fill_in>/providers/Microsoft.Resources/templateSpecs/<todo_fill_in>/versions/<todo_fill_in>",
                ["properties"] = new JObject {
                    ["mainTemplate"] = compilationResult.Template,
                },
            };

            modules[registryPath] = new(templateSpec.ToJson(), new(registryPath));
        }

        return DataSetsExtensions.CreateMockTemplateSpecRepositoryFactory(modules.ToImmutableDictionary());
    }
}