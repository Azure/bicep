// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
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

    public static ServiceBuilder WithNamespaceProvider(this ServiceBuilder serviceBuilder, INamespaceProvider namespaceProvider)
        => serviceBuilder.WithRegistration(x => x.WithNamespaceProvider(namespaceProvider));

    public static ServiceBuilder WithConfigurationPatch(this ServiceBuilder serviceBuilder, Func<RootConfiguration, RootConfiguration> patchFunc)
        => serviceBuilder.WithRegistration(x => x.WithConfigurationPatch(patchFunc));

    public static ServiceBuilder WithDisabledAnalyzersConfiguration(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithRegistration(x => x.WithDisabledAnalyzersConfiguration());

    public static ServiceBuilder WithBicepAnalyzer(this ServiceBuilder serviceBuilder, IBicepAnalyzer bicepAnalyzer)
        => serviceBuilder.WithRegistration(x => x.WithBicepAnalyzer(bicepAnalyzer));

    public static ServiceBuilder WithAzResources(this ServiceBuilder serviceBuilder, IEnumerable<ResourceTypeComponents> resourceTypes)
        => serviceBuilder.WithRegistration(x => x.WithAzResources(resourceTypes));

    public static ServiceBuilder WithAzResourceTypeLoader(this ServiceBuilder serviceBuilder, IAzResourceTypeLoader azResourceTypeLoader)
        => serviceBuilder.WithRegistration(x => x.WithAzResourceTypeLoaderFactory(azResourceTypeLoader));

    public static ServiceBuilder WithEmptyAzResources(this ServiceBuilder serviceBuilder)
        => serviceBuilder.WithRegistration(x => x.WithEmptyAzResources());

    public static ServiceBuilder WithWorkspaceFiles(this ServiceBuilder serviceBuilder, IReadOnlyDictionary<Uri, string> fileContentsByUri)
        => serviceBuilder.WithRegistration(x => x.WithWorkspaceFiles(fileContentsByUri));

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
