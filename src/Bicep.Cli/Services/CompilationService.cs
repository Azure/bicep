// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core.Analyzers.Linter;
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
        private readonly IApiVersionProvider apiVersionProvider;
        private readonly IFeatureProviderManager featureProviderManager;

        public CompilationService(
            IDiagnosticLogger diagnosticLogger,
            IFileResolver fileResolver,
            InvocationContext invocationContext,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            TemplateDecompiler decompiler,
            IApiVersionProvider apiVersionProvider,
            IFeatureProviderManager featureProviderManager)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.invocationContext = invocationContext;
            this.workspace = new Workspace();
            this.decompiler = decompiler;
            this.apiVersionProvider = apiVersionProvider;
            this.featureProviderManager = featureProviderManager;
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

            var compilation = new Compilation(featureProviderManager, this.invocationContext.NamespaceProvider, sourceFileGrouping, configurationManager, apiVersionProvider, new LinterAnalyzer());
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

            var model = new ParamsSemanticModel(sourceFileGrouping, configurationManager.GetConfiguration(inputUri), featureProviderManager.GetFeatureProvider(inputUri), file => {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);


                return new Compilation(featureProviderManager, this.invocationContext.NamespaceProvider, compilationGrouping, configurationManager, apiVersionProvider, new LinterAnalyzer());
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
            await CompileAsync(decompilation.entrypointUri.AbsolutePath, skipRestore: true);

            return decompilation;
        }

        private static ImmutableDictionary<BicepFile, ImmutableArray<IDiagnostic>> GetModuleRestoreDiagnosticsByBicepFile(SourceFileGrouping sourceFileGrouping, ImmutableHashSet<(ISourceFile, ModuleDeclarationSyntax)> originalModulesToRestore, bool forceModulesRestore)
        {
            static IEnumerable<IDiagnostic> GetModuleDiagnosticsPerFile(SourceFileGrouping grouping, BicepFile bicepFile)
            {
                foreach (var module in bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>())
                {
                    if (grouping.TryGetErrorDiagnostic(module) is {} errorBuilder)
                    {
                        yield return errorBuilder(DiagnosticBuilder.ForPosition(module.Path));
                    }
                }
            }

            if (forceModulesRestore)
            {
                return sourceFileGrouping.SourceFiles
                    .OfType<BicepFile>()
                    .ToImmutableDictionary(bicepFile => bicepFile, bicepFile => GetModuleDiagnosticsPerFile(sourceFileGrouping, bicepFile).ToImmutableArray());
            }

            return originalModulesToRestore.SelectMany(t => t.Item1 is BicepFile bicepFile ? new [] { (bicepFile, t.Item2) } : Enumerable.Empty<(BicepFile, ModuleDeclarationSyntax)>())
                .SelectMany(t => sourceFileGrouping.TryGetErrorDiagnostic(t.Item2)?.Invoke(DiagnosticBuilder.ForPosition(t.Item2.Path)) is IDiagnostic diagnostic ? new [] { (t.Item1, diagnostic) } : Enumerable.Empty<(BicepFile, IDiagnostic)>())
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
