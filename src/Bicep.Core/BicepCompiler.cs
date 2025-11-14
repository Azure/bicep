// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core;

public class BicepCompiler
{
    public static BicepCompiler Create(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        configureServices?.Invoke(services);

        services.AddBicepCore();

        return services
            .BuildServiceProvider()
            .GetRequiredService<BicepCompiler>();
    }

    private readonly IEnvironment environment;
    private readonly INamespaceProvider namespaceProvider;
    private readonly IBicepAnalyzer bicepAnalyzer;
    private readonly IFileExplorer fileExplorer;
    private readonly IModuleDispatcher moduleDispatcher;

    public BicepCompiler(
        IEnvironment environment,
        INamespaceProvider namespaceProvider,
        IBicepAnalyzer bicepAnalyzer,
        IFileExplorer fileExplorer,
        IModuleDispatcher moduleDispatcher,
        ISourceFileFactory sourceFileFactory)
    {
        this.environment = environment;
        this.namespaceProvider = namespaceProvider;
        this.bicepAnalyzer = bicepAnalyzer;
        this.fileExplorer = fileExplorer;
        this.moduleDispatcher = moduleDispatcher;
        this.SourceFileFactory = sourceFileFactory;
    }

    public ISourceFileFactory SourceFileFactory { get; }

    public Compilation CreateCompilationWithoutRestore(IOUri bicepUri, IActiveSourceFileLookup? workspace = null, bool markAllForRestore = false)
    {
        workspace ??= new ActiveSourceFileSet();
        var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileExplorer, moduleDispatcher, workspace, this.SourceFileFactory, bicepUri, markAllForRestore);

        return Create(sourceFileGrouping);
    }

    public async Task<Compilation> CreateCompilation(IOUri bicepUri, IActiveSourceFileLookup? workspace = null, bool skipRestore = false, bool forceRestore = false)
    {
        workspace ??= new ActiveSourceFileSet();
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
        var artifactsToRestore = sourceFileGrouping.GetArtifactsToRestore(forceRestore);

        if (await moduleDispatcher.RestoreArtifacts(ArtifactHelper.GetValidArtifactReferences(artifactsToRestore), forceRestore: forceRestore))
        {
            // modules had to be restored - recompile
            sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(fileExplorer, moduleDispatcher, workspace, this.SourceFileFactory, sourceFileGrouping);
        }
        return Create(sourceFileGrouping);
    }

    public async Task<ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>>> Restore(Compilation compilation, bool forceRestore)
    {
        var workspace = new ActiveSourceFileSet();
        var sourceFileGrouping = compilation.SourceFileGrouping;
        var artifactsToRestore = sourceFileGrouping.GetArtifactsToRestore(forceRestore);

        if (await moduleDispatcher.RestoreArtifacts(ArtifactHelper.GetValidArtifactReferences(artifactsToRestore), forceRestore))
        {
            // artifacts had to be restored - recompile
            sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(fileExplorer, moduleDispatcher, workspace, this.SourceFileFactory, sourceFileGrouping);
        }

        return GetModuleRestoreDiagnosticsByBicepFile(sourceFileGrouping, [.. artifactsToRestore], forceRestore);
    }

    private Compilation Create(SourceFileGrouping sourceFileGrouping)
        => new(
            environment,
            namespaceProvider,
            sourceFileGrouping,
            bicepAnalyzer,
            moduleDispatcher,
            this.SourceFileFactory,
            []);

    private static ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> GetModuleRestoreDiagnosticsByBicepFile(SourceFileGrouping sourceFileGrouping, ImmutableHashSet<ArtifactResolutionInfo> originalModulesToRestore, bool forceModulesRestore)
    {
        static IDiagnostic? DiagnosticForModule(SourceFileGrouping grouping, IArtifactReferenceSyntax moduleDeclaration)
            => grouping.TryGetSourceFile(moduleDeclaration).IsSuccess(out _, out var errorBuilder) ? null : errorBuilder(DiagnosticBuilder.ForPosition(moduleDeclaration.SourceSyntax));

        static IEnumerable<(BicepSourceFile, IDiagnostic)> GetDiagnosticsForModulesToRestore(SourceFileGrouping grouping, ImmutableHashSet<ArtifactResolutionInfo> originalArtifactsToRestore)
        {
            foreach (var artifact in originalArtifactsToRestore)
            {
                if (artifact.Syntax is not null and not ExtensionDeclarationSyntax &&
                    DiagnosticForModule(grouping, artifact.Syntax) is { } diagnostic)
                {
                    yield return (artifact.ReferencingFile, diagnostic);
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
