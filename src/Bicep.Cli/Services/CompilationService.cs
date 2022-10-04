// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Cli.Services
{
    public class CompilationService
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IConfigurationManager configurationManager;
        private readonly InvocationContext invocationContext;
        private readonly Workspace workspace;
        private readonly TemplateDecompiler decompiler;
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IBicepAnalyzer bicepAnalyzer;

        public CompilationService(
            IDiagnosticLogger diagnosticLogger,
            IFileResolver fileResolver,
            InvocationContext invocationContext,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            TemplateDecompiler decompiler,
            IApiVersionProviderFactory apiVersionProviderFactory,
            IFeatureProviderFactory featureProviderFactory,
            IBicepAnalyzer bicepAnalyzer)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.invocationContext = invocationContext;
            this.workspace = new Workspace();
            this.decompiler = decompiler;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.featureProviderFactory = featureProviderFactory;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public async Task RestoreAsync(string inputPath, bool forceModulesRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var configuration = this.configurationManager.GetConfiguration(inputUri);

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri, forceModulesRestore);
            var originalModulesToRestore = sourceFileGrouping.GetModulesToRestore().ToImmutableHashSet();

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in processing and logging
            var modulesToRestoreReferences = this.moduleDispatcher.GetValidModuleReferences(originalModulesToRestore)
                .Distinct()
                .OrderBy(key => key.FullyQualifiedReference);

            // restore is supposed to only restore the module references that are syntactically valid
            await moduleDispatcher.RestoreModules(modulesToRestoreReferences, forceModulesRestore);

            // update the errors based on restore status
            sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(this.moduleDispatcher, this.workspace, sourceFileGrouping);

            LogDiagnostics(GetModuleRestoreDiagnosticsByBicepFile(sourceFileGrouping, originalModulesToRestore, forceModulesRestore));
        }

        public async Task<Compilation> CompileAsync(string inputPath, bool skipRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            return await CompileAsync(inputUri, skipRestore);
        }

        public async Task<Compilation> CompileAsync(Uri inputUri, bool skipRestore)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri);
            if (!skipRestore)
            {
                // module references in the file may be malformed
                // however we still want to surface as many errors as we can for the module refs that are valid
                // so we will try to restore modules with valid refs and skip everything else
                // (the diagnostics will be collected during compilation)
                if (await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(sourceFileGrouping.GetModulesToRestore())))
                {
                    // modules had to be restored - recompile
                    sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, this.workspace, sourceFileGrouping);
                }
            }

            var compilation = new Compilation(featureProviderFactory, this.invocationContext.NamespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
            LogDiagnostics(compilation);

            return compilation;
        }

        public async Task<ParamsSemanticModel> CompileParams(string inputPath, bool skipRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri);
            if (!skipRestore)
            {
                // module references in the file may be malformed
                // however we still want to surface as many errors as we can for the module refs that are valid
                // so we will try to restore modules with valid refs and skip everything else
                // (the diagnostics will be collected during compilation)
                if (await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(sourceFileGrouping.GetModulesToRestore())))
                {
                    // modules had to be restored - recompile
                    sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, this.workspace, sourceFileGrouping);
                }
            }

            var model = new ParamsSemanticModel(sourceFileGrouping, configurationManager.GetConfiguration(inputUri), featureProviderFactory.GetFeatureProvider(inputUri), file => {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);


                return new Compilation(featureProviderFactory, this.invocationContext.NamespaceProvider, compilationGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
            });
            LogParamDiagnostics(model);

            return model;
        }

        public async Task<(Uri, ImmutableDictionary<Uri, string>)> DecompileAsync(string inputPath, string outputPath)
        {
            inputPath = PathHelper.ResolvePath(inputPath);
            Uri inputUri = PathHelper.FilePathToFileUrl(inputPath);

            Uri outputUri = PathHelper.FilePathToFileUrl(outputPath);

            var decompilation = decompiler.DecompileFileWithModules(inputUri, outputUri);

            foreach (var (fileUri, bicepOutput) in decompilation.filesToSave)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(fileUri, bicepOutput));
            }

            // to verify success we recompile and check for syntax errors.
            await CompileAsync(decompilation.entrypointUri.LocalPath, skipRestore: true);

            return decompilation;
        }

        private static ImmutableDictionary<BicepFile, ImmutableArray<IDiagnostic>> GetModuleRestoreDiagnosticsByBicepFile(SourceFileGrouping sourceFileGrouping, ImmutableHashSet<ModuleSourceResolutionInfo> originalModulesToRestore, bool forceModulesRestore)
        {
            static IDiagnostic? DiagnosticForModule(SourceFileGrouping grouping, ModuleDeclarationSyntax module)
                => grouping.TryGetErrorDiagnostic(module) is {} errorBuilder ? errorBuilder(DiagnosticBuilder.ForPosition(module.Path)) : null;

            static IEnumerable<(BicepFile, IDiagnostic)> GetDiagnosticsForModulesToRestore(SourceFileGrouping grouping, ImmutableHashSet<ModuleSourceResolutionInfo> originalModulesToRestore)
            {
                foreach (var (module, sourceFile) in originalModulesToRestore)
                {
                    if (sourceFile is BicepFile bicepFile && DiagnosticForModule(grouping, module) is {} diagnostic)
                    {
                        yield return (bicepFile, diagnostic);
                    }
                }
            }

            static IEnumerable<(BicepFile, IDiagnostic)> GetDiagnosticsForAllModules(SourceFileGrouping grouping)
            {
                foreach (var bicepFile in grouping.SourceFiles.OfType<BicepFile>())
                {
                    foreach (var module in bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>())
                    {
                        if (DiagnosticForModule(grouping, module) is {} diagnostic)
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

        private void LogDiagnostics(Compilation compilation)
        {
            if (compilation is null)
            {
                throw new Exception("Compilation is null. A compilation must exist before logging the diagnostics.");
            }

            LogDiagnostics(compilation.GetAllDiagnosticsByBicepFile());
        }

        private void LogDiagnostics(ImmutableDictionary<BicepFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile)
        {
            foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
            {
                foreach (var diagnostic in diagnostics)
                {
                    diagnosticLogger.LogDiagnostic(bicepFile.FileUri, diagnostic, bicepFile.LineStarts);
                }
            }
        }

        private void LogParamDiagnostics(ParamsSemanticModel paramSemanticModel)
        {
            foreach (var diagnostic in paramSemanticModel.GetAllDiagnostics())
            {
                diagnosticLogger.LogDiagnostic(paramSemanticModel.BicepParamFile.FileUri, diagnostic, paramSemanticModel.BicepParamFile.LineStarts);
            };
        }
    }
}
