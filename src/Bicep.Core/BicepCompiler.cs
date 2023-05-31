// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;

namespace Bicep.Core;

public class BicepCompiler
{
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly INamespaceProvider namespaceProvider;
    private readonly IConfigurationManager configurationManager;
    private readonly IApiVersionProviderFactory apiVersionProviderFactory;
    private readonly IBicepAnalyzer bicepAnalyzer;
    private readonly IFileResolver fileResolver;
    private readonly IModuleDispatcher moduleDispatcher;

    public BicepCompiler(
        IFeatureProviderFactory featureProviderFactory,
        INamespaceProvider namespaceProvider,
        IConfigurationManager configurationManager,
        IApiVersionProviderFactory apiVersionProviderFactory,
        IBicepAnalyzer bicepAnalyzer,
        IFileResolver fileResolver,
        IModuleDispatcher moduleDispatcher)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.namespaceProvider = namespaceProvider;
        this.configurationManager = configurationManager;
        this.apiVersionProviderFactory = apiVersionProviderFactory;
        this.bicepAnalyzer = bicepAnalyzer;
        this.fileResolver = fileResolver;
        this.moduleDispatcher = moduleDispatcher;
    }

    public async Task<Compilation> CreateCompilation(Uri bicepUri, IReadOnlyWorkspace? workspace = null, bool skipRestore = false, bool forceModulesRestore = false)
    {
        workspace ??= new Workspace();
        var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, bicepUri, forceModulesRestore);

        if (!skipRestore)
        {
            // module references in the file may be malformed
            // however we still want to surface as many errors as we can for the module refs that are valid
            // so we will try to restore modules with valid refs and skip everything else
            // (the diagnostics will be collected during compilation)
            if (await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(sourceFileGrouping.GetModulesToRestore())))
            {
                // modules had to be restored - recompile
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, sourceFileGrouping);
            }
        }

        return new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
    }
}
