// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;

namespace Bicep.Cli.Services
{
    public class CompilationService
    {
        private readonly BicepCompiler bicepCompiler;
        private readonly BicepDecompiler decompiler;
        private readonly BicepparamDecompiler paramDecompiler;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IFileResolver fileResolver;

        public CompilationService(
            BicepCompiler bicepCompiler,
            BicepDecompiler decompiler,
            BicepparamDecompiler paramDecompiler,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IFileResolver fileResolver)
        {
            this.bicepCompiler = bicepCompiler;
            this.decompiler = decompiler;
            this.paramDecompiler = paramDecompiler;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.featureProviderFactory = featureProviderFactory;
            this.fileResolver = fileResolver;
        }

        public async Task<ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>>> RestoreAsync(string inputPath, bool forceModulesRestore)
        {
            var workspace = new Workspace();
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var configuration = this.configurationManager.GetConfiguration(inputUri);

            var compilation = await bicepCompiler.CreateCompilation(inputUri, workspace, skipRestore: true, forceModulesRestore: forceModulesRestore);
            var originalModulesToRestore = compilation.SourceFileGrouping.GetArtifactsToRestore().ToImmutableHashSet();

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in processing and logging
            var modulesToRestoreReferences = this.moduleDispatcher.GetValidModuleReferences(originalModulesToRestore)
                .Distinct()
                .OrderBy(key => key.FullyQualifiedReference);

            // restore is supposed to only restore the module references that are syntactically valid
            await moduleDispatcher.RestoreModules(modulesToRestoreReferences, forceModulesRestore);

            // update the errors based on restore status
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(featureProviderFactory, this.moduleDispatcher, workspace, compilation.SourceFileGrouping);

            return GetModuleRestoreDiagnosticsByBicepFile(sourceFileGrouping, originalModulesToRestore, forceModulesRestore);
        }

        public async Task<Compilation> CompileAsync(string inputPath, bool skipRestore, Workspace? workspace = null, Action<Compilation>? validateFunc = null)
        {
            workspace ??= new Workspace();
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            var compilation = await bicepCompiler.CreateCompilation(inputUri, workspace, skipRestore, forceModulesRestore: false);

            validateFunc?.Invoke(compilation);

            return compilation;
        }

        public async Task<(Compilation, DecompileResult)> DecompileAsync(string inputPath, string outputPath)
        {
            var workspace = new Workspace();
            inputPath = PathHelper.ResolvePath(inputPath);
            Uri inputUri = PathHelper.FilePathToFileUrl(inputPath);
            Uri outputUri = PathHelper.FilePathToFileUrl(outputPath);
            if (!fileResolver.TryRead(inputUri).IsSuccess(out var jsonContents))
            {
                throw new InvalidOperationException($"Failed to read {inputUri}");
            }

            var decompilation = await decompiler.Decompile(outputUri, jsonContents);

            foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(fileUri, bicepOutput));
            }

            // to verify success we recompile and check for syntax errors.
            var compilation = await CompileAsync(decompilation.EntrypointUri.LocalPath, skipRestore: true, workspace: workspace);

            return (compilation, decompilation);
        }

        public DecompileResult DecompileParams(string inputPath, string outputPath, string? bicepPath)
        {
            inputPath = PathHelper.ResolvePath(inputPath);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var outputUri = PathHelper.FilePathToFileUrl(outputPath);
            var bicepUri = bicepPath is { } ? PathHelper.FilePathToFileUrl(bicepPath) : null;

            return paramDecompiler.Decompile(inputUri, outputUri, bicepUri);
        }

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
}
