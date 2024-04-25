// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Workspaces;

namespace Bicep.Core.UnitTests.Utils;

public static class ServiceBuilderExtensions
{
    public static ServiceBuilder WithFileResolver(this ServiceBuilder serviceBuilder, IFileResolver fileResolver)
        => serviceBuilder.WithRegistration(x => x.WithFileResolver(fileResolver));

    public static ServiceBuilder WithWorkspace(this ServiceBuilder serviceBuilder, IWorkspace workspace)
        => serviceBuilder.WithRegistration(x => x.WithWorkspace(workspace));

    public static ServiceBuilder WithFeatureOverrides(this ServiceBuilder serviceBuilder, FeatureProviderOverrides overrides)
        => serviceBuilder.WithRegistration(x => x.WithFeatureOverrides(overrides));

    public static ServiceBuilder WithContainerRegistryClientFactory(this ServiceBuilder serviceBuilder, IContainerRegistryClientFactory containerRegistryClientFactory)
        => serviceBuilder.WithRegistration(x => x.WithContainerRegistryClientFactory(containerRegistryClientFactory));

    public static ServiceBuilder WithTemplateSpecRepositoryFactory(this ServiceBuilder serviceBuilder, ITemplateSpecRepositoryFactory factory)
        => serviceBuilder.WithRegistration(x => x.WithTemplateSpecRepositoryFactory(factory));

    public static ServiceBuilder WithFeatureProviderFactory(this ServiceBuilder serviceBuilder, IFeatureProviderFactory featureProviderFactory)
        => serviceBuilder.WithRegistration(x => x.WithFeatureProviderFactory(featureProviderFactory));

    public static ServiceBuilder WithNamespaceProvider(this ServiceBuilder serviceBuilder, INamespaceProvider namespaceProvider)
        => serviceBuilder.WithRegistration(x => x.WithNamespaceProvider(namespaceProvider));

    public static ServiceBuilder WithConfigurationPatch(this ServiceBuilder serviceBuilder, Func<RootConfiguration, RootConfiguration> patchFunc)
        => serviceBuilder.WithRegistration(x => x.WithConfigurationPatch(patchFunc));

    public static ServiceBuilder WithDisabledAnalyzersConfiguration(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithRegistration(x => x.WithDisabledAnalyzersConfiguration());

    public static ServiceBuilder WithConfiguration(this ServiceBuilder serviceBuilder, RootConfiguration configuration)
        => serviceBuilder.WithRegistration(x => x.WithConfiguration(configuration));

    public static ServiceBuilder WithBicepAnalyzer(this ServiceBuilder serviceBuilder, IBicepAnalyzer bicepAnalyzer)
        => serviceBuilder.WithRegistration(x => x.WithBicepAnalyzer(bicepAnalyzer));

    public static ServiceBuilder WithAzResources(this ServiceBuilder serviceBuilder, IEnumerable<ResourceTypeComponents> resourceTypes)
        => serviceBuilder.WithRegistration(x => x.WithAzResources(resourceTypes));

    public static ServiceBuilder WithAzResourceTypeLoader(this ServiceBuilder serviceBuilder, IResourceTypeLoader azResourceTypeLoader)
        => serviceBuilder.WithRegistration(x => x.WithAzResourceTypeLoaderFactory(azResourceTypeLoader));

    public static ServiceBuilder WithEmptyAzResources(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithRegistration(x => x.WithEmptyAzResources());

    public static ServiceBuilder WithEnvironmentVariables(this ServiceBuilder serviceBuilder, params (string key, string? value)[] variables)
        => serviceBuilder.WithRegistration(x => x.WithEnvironmentVariables(variables));

    public static ServiceBuilder WithFileSystem(this ServiceBuilder serviceBuilder, IFileSystem fileSystem)
        => serviceBuilder.WithRegistration(x => x.WithFileSystem(fileSystem));

    public static ServiceBuilder WithMockFileSystem(this ServiceBuilder serviceBuilder, IReadOnlyDictionary<string, string> fileLookup)
        => WithMockFileSystem(serviceBuilder, fileLookup.ToDictionary(x => PathHelper.FilePathToFileUrl(x.Key), x => x.Value));

    public static ServiceBuilder WithMockFileSystem(this ServiceBuilder serviceBuilder, IReadOnlyDictionary<Uri, string>? fileLookup = null)
        => serviceBuilder.WithFileSystem(new MockFileSystem(
            fileLookup?.ToDictionary(x => x.Key.LocalPath, x => new MockFileData(x.Value)) ?? new()));

    public static Compilation BuildCompilation(this ServiceBuilder services, IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri)
    {
        var compiler = services.Build().GetCompiler();
        var workspace = CompilationHelper.CreateWorkspace(fileContentsByUri);

        return compiler.CreateCompilationWithoutRestore(entryFileUri, workspace);
    }

    public static Compilation BuildCompilation(this ServiceBuilder services, string text)
    {
        var entryFileUri = new Uri("file:///main.bicep");

        return BuildCompilation(services, new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri);
    }
}
