// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests.Utils;

public static class ServiceBuilderExtensions
{
    private static ServiceBuilder Register<TService>(ServiceBuilder serviceBuilder, TService service)
        where TService : class
        => serviceBuilder.WithRegistration(x => x.AddSingleton<TService>(service));

    public static ServiceBuilder WithFileResolver(this ServiceBuilder serviceBuilder, IFileResolver fileResolver)
        => Register(serviceBuilder, fileResolver);

    public static ServiceBuilder WithWorkspace(this ServiceBuilder serviceBuilder, IWorkspace workspace)
        => Register(serviceBuilder, workspace);

    public static ServiceBuilder WithFeatureOverrides(this ServiceBuilder serviceBuilder, FeatureProviderOverrides overrides)
    {
        Register(serviceBuilder, overrides);
        serviceBuilder.WithRegistration(x => x.AddSingleton<FeatureProviderFactory>());
        return serviceBuilder.WithRegistration(x => x.AddSingleton<IFeatureProviderFactory, OverriddenFeatureProviderFactory>());
    }

    public static ServiceBuilder WithNamespaceProvider(this ServiceBuilder serviceBuilder, INamespaceProvider namespaceProvider)
        => Register(serviceBuilder, namespaceProvider);

    public static ServiceBuilder WithConfigurationPatch(this ServiceBuilder serviceBuilder, Func<RootConfiguration, RootConfiguration> patchFunc)
    {
        Register(serviceBuilder, patchFunc);
        serviceBuilder.WithRegistration(x => x.AddSingleton<ConfigurationManager>());
        return serviceBuilder.WithRegistration(x => x.AddSingleton<IConfigurationManager, PatchingConfigurationManager>());
    }

    public static ServiceBuilder WithDisabledAnalyzersConfiguration(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithConfigurationPatch(c => c.WithAllAnalyzersDisabled());

    public static ServiceBuilder WithApiVersionProviderFactory(this ServiceBuilder serviceBuilder, IApiVersionProviderFactory apiVersionProviderFactory)
        => Register(serviceBuilder, apiVersionProviderFactory);

    public static ServiceBuilder WithApiVersionProvider(this ServiceBuilder serviceBuilder, IApiVersionProvider apiVersionProvider)
        => serviceBuilder.WithApiVersionProviderFactory(IApiVersionProviderFactory.WithStaticApiVersionProvider(apiVersionProvider));

    public static ServiceBuilder WithBicepAnalyzer(this ServiceBuilder serviceBuilder, IBicepAnalyzer bicepAnalyzer)
        => Register(serviceBuilder, bicepAnalyzer);

    public static ServiceBuilder WithAzResources(this ServiceBuilder serviceBuilder, IEnumerable<ResourceTypeComponents> resourceTypes)
        => serviceBuilder.WithAzResourceTypeLoader(TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(resourceTypes));

    public static ServiceBuilder WithAzResourceTypeLoader(this ServiceBuilder serviceBuilder, IAzResourceTypeLoader azResourceTypeLoader)
        => serviceBuilder.WithRegistration(x => x.AddSingleton<IAzResourceTypeLoader>(azResourceTypeLoader));

    public static ServiceBuilder WithEmptyAzResources(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithAzResources(Enumerable.Empty<ResourceTypeComponents>());

    public static ServiceBuilder WithWorkspaceFiles(this ServiceBuilder serviceBuilder, IReadOnlyDictionary<Uri, string> fileContentsByUri)
    {
        var workspace = new Workspace();
        var sourceFiles = fileContentsByUri.Select(kvp => SourceFileFactory.CreateSourceFile(kvp.Key, kvp.Value));
        workspace.UpsertSourceFiles(sourceFiles);

        return serviceBuilder.WithWorkspace(workspace);
    }

    public static Compilation BuildCompilation(this ServiceBuilder services, IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri)
    {
        var service = services.WithWorkspaceFiles(fileContentsByUri).Build();

        var sourceFileGrouping = service.BuildSourceFileGrouping(entryFileUri);
        return service.BuildCompilation(sourceFileGrouping);
    }

    public static Compilation BuildCompilation(this ServiceBuilder services, string text)
    {
        var entryFileUri = new Uri("file:///main.bicep");

        return BuildCompilation(services, new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri);
    }

    public static SourceFileGrouping BuildSourceFileGrouping(this ServiceBuilder services, IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri)
    {
        var service = services.WithWorkspaceFiles(fileContentsByUri).Build();

        return service.BuildSourceFileGrouping(entryFileUri);
    }

    public static SourceFileGrouping BuildSourceFileGrouping(this ServiceBuilder services, string text)
    {
        var entryFileUri = new Uri("file:///main.bicep");

        return BuildSourceFileGrouping(services, new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri);
    }
}
