// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core;

public class BicepCompiler
{
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly IEnvironment environment;
    private readonly INamespaceProvider namespaceProvider;
    private readonly IConfigurationManager configurationManager;
    private readonly IBicepAnalyzer bicepAnalyzer;
    private readonly IFileResolver fileResolver;
    private readonly IModuleDispatcher moduleDispatcher;

    public BicepCompiler(
        IFeatureProviderFactory featureProviderFactory,
        IEnvironment environment,
        INamespaceProvider namespaceProvider,
        IConfigurationManager configurationManager,
        IBicepAnalyzer bicepAnalyzer,
        IFileResolver fileResolver,
        IModuleDispatcher moduleDispatcher)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.environment = environment;
        this.namespaceProvider = namespaceProvider;
        this.configurationManager = configurationManager;
        this.bicepAnalyzer = bicepAnalyzer;
        this.fileResolver = fileResolver;
        this.moduleDispatcher = moduleDispatcher;
    }

    public Compilation CreateCompilationWithoutRestore(Uri bicepUri, IReadOnlyWorkspace? workspace = null, bool markAllForRestore = false)
    {
        workspace ??= new Workspace();
        var sourceFileGrouping = SourceFileGroupingBuilder.Build(
            fileResolver,
            moduleDispatcher,
            configurationManager,
            workspace,
            bicepUri,
            featureProviderFactory,
            markAllForRestore);

        return Create(sourceFileGrouping);
    }

    public async Task<Compilation> CreateCompilation(Uri bicepUri, IReadOnlyWorkspace? workspace = null, bool skipRestore = false, bool forceRestore = false)
    {
        workspace ??= new Workspace();
        var compilation = CreateCompilationWithoutRestore(bicepUri, workspace, markAllForRestore: forceRestore);
        var sourceFileGrouping = compilation.SourceFileGrouping;

        if (skipRestore)
        {
            return compilation;
        }

        // module references in the file may be malformed
        // however we still want to surface as many errors as we can for the module refs that are valid
        // so we will try to restore modules with valid refs and skip everything else
        // (the diagnostics will be collected during compilation)
        var artifactsToRestore = moduleDispatcher.GetValidModuleReferences(
            sourceFileGrouping.GetExplicitArtifactsToRestore()).Concat(
                sourceFileGrouping.GetImplicitArtifactsToRestore());

        if (await moduleDispatcher.RestoreModules(artifactsToRestore, forceRestore: forceRestore))
        {
            // modules had to be restored - recompile
            sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(featureProviderFactory, moduleDispatcher, configurationManager, workspace, sourceFileGrouping);
        }
        return Create(sourceFileGrouping);
    }

    public async Task<ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>>> Restore(Compilation compilation, bool force)
    {
        var workspace = new Workspace();
        var sourceFileGrouping = compilation.SourceFileGrouping;
        var originalModulesToRestore = sourceFileGrouping.GetExplicitArtifactsToRestore().ToImmutableHashSet();
        if (await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(originalModulesToRestore), force))
        {
            // modules had to be restored - recompile
            sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(featureProviderFactory, moduleDispatcher, configurationManager, workspace, sourceFileGrouping);
        }

        return GetModuleRestoreDiagnosticsByBicepFile(sourceFileGrouping, originalModulesToRestore, forceModulesRestore: force);
    }

    private Compilation Create(SourceFileGrouping sourceFileGrouping)
        => new(
            featureProviderFactory,
            environment,
            namespaceProvider,
            sourceFileGrouping,
            configurationManager,
            bicepAnalyzer,
            moduleDispatcher,
            new AuxiliaryFileCache(fileResolver),
            ImmutableDictionary<ISourceFile, ISemanticModel>.Empty);

    private static ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> GetModuleRestoreDiagnosticsByBicepFile(SourceFileGrouping sourceFileGrouping, ImmutableHashSet<ArtifactResolutionInfo> originalModulesToRestore, bool forceModulesRestore)
    {
        static IDiagnostic? DiagnosticForModule(SourceFileGrouping grouping, IArtifactReferenceSyntax moduleDeclaration)
            => grouping.TryGetSourceFile(moduleDeclaration).IsSuccess(out _, out var errorBuilder) ? null : errorBuilder(DiagnosticBuilder.ForPosition(moduleDeclaration.SourceSyntax));

        static IEnumerable<(BicepSourceFile, IDiagnostic)> GetDiagnosticsForModulesToRestore(SourceFileGrouping grouping, ImmutableHashSet<ArtifactResolutionInfo> originalArtifactsToRestore)
        {
            var originalModulesToRestore = originalArtifactsToRestore.OfType<ArtifactResolutionInfo>();
            foreach (var (module, sourceFile) in originalModulesToRestore)
            {
                if (sourceFile is BicepSourceFile bicepFile &&
                    DiagnosticForModule(grouping, module) is { } diagnostic)
                {
                    yield return (bicepFile, diagnostic);
                }
            }
        }

        static IEnumerable<(BicepSourceFile, IDiagnostic)> GetDiagnosticsForAllModules(SourceFileGrouping grouping)
        {
            foreach (var bicepFile in grouping.SourceFiles.OfType<BicepSourceFile>())
            {
                foreach (var module in bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>())
                {
                    if (DiagnosticForModule(grouping, module) is { } diagnostic)
                    {
                        yield return (bicepFile, diagnostic);
                    }
                }
            }
        }

        var diagnosticsByFile = forceModulesRestore ? GetDiagnosticsForAllModules(sourceFileGrouping) : GetDiagnosticsForModulesToRestore(sourceFileGrouping, originalModulesToRestore);

        return diagnosticsByFile
            .ToLookup(t => t.Item1, t => t.Item2)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableArray());
    }
}
