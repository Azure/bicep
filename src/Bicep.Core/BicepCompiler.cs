// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Interfaces;
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
    private readonly IBicepAnalyzer bicepAnalyzer;
    private readonly IFileResolver fileResolver;
    private readonly IArtifactDispatcher artifactDispatcher;

    public BicepCompiler(
        IFeatureProviderFactory featureProviderFactory,
        INamespaceProvider namespaceProvider,
        IConfigurationManager configurationManager,
        IBicepAnalyzer bicepAnalyzer,
        IFileResolver fileResolver,
        IArtifactDispatcher artifactDispatcher)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.namespaceProvider = namespaceProvider;
        this.configurationManager = configurationManager;
        this.bicepAnalyzer = bicepAnalyzer;
        this.fileResolver = fileResolver;
        this.artifactDispatcher = artifactDispatcher;
    }

    public async Task<Compilation> CreateCompilation(Uri bicepUri, IReadOnlyWorkspace? workspace = null, bool skipRestore = false, bool forceModulesRestore = false)
    {
        workspace ??= new Workspace();
        var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, artifactDispatcher, workspace, bicepUri, forceModulesRestore);

        if (!skipRestore)
        {
            // module references in the file may be malformed
            // however we still want to surface as many errors as we can for the module refs that are valid
            // so we will try to restore modules with valid refs and skip everything else
            // (the diagnostics will be collected during compilation)
            if (await artifactDispatcher.RestoreModules(artifactDispatcher.GetValidModuleReferences(sourceFileGrouping.GetArtifactsToRestore())))
            {
                // modules had to be restored - recompile
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(artifactDispatcher, workspace, sourceFileGrouping);
            }
            //TODO(asilverman): I want to inject here the logic that restores the providers
        }

        return new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, bicepAnalyzer);
    }
}
