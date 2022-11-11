// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
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
        private readonly BicepCompiler bicepCompiler;
        private readonly BicepDecompiler decompiler;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IConfigurationManager configurationManager;
        private readonly Workspace workspace;

        public CompilationService(
            BicepCompiler bicepCompiler,
            BicepDecompiler decompiler,
            IDiagnosticLogger diagnosticLogger,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager)
        {
            this.bicepCompiler = bicepCompiler;
            this.decompiler = decompiler;
            this.diagnosticLogger = diagnosticLogger;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.workspace = new Workspace();
        }

        public async Task RestoreAsync(string inputPath, bool forceModulesRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var configuration = this.configurationManager.GetConfiguration(inputUri);

            var compilation = await bicepCompiler.CreateCompilation(inputUri, skipRestore: true, this.workspace);
            var originalModulesToRestore = compilation.SourceFileGrouping.GetModulesToRestore().ToImmutableHashSet();

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in processing and logging
            var modulesToRestoreReferences = this.moduleDispatcher.GetValidModuleReferences(originalModulesToRestore)
                .Distinct()
                .OrderBy(key => key.FullyQualifiedReference);

            // restore is supposed to only restore the module references that are syntactically valid
            await moduleDispatcher.RestoreModules(modulesToRestoreReferences, forceModulesRestore);

            // update the errors based on restore status
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(this.moduleDispatcher, this.workspace, compilation.SourceFileGrouping);

            LogDiagnostics(GetModuleRestoreDiagnosticsByBicepFile(sourceFileGrouping, originalModulesToRestore, forceModulesRestore));
        }

        public async Task<Compilation> CompileAsync(string inputPath, bool skipRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            var compilation = await bicepCompiler.CreateCompilation(inputUri, skipRestore, this.workspace);
            LogDiagnostics(compilation);

            return compilation;
        }

        public async Task<DecompileResult> DecompileAsync(string inputPath, string outputPath)
        {
            inputPath = PathHelper.ResolvePath(inputPath);
            Uri inputUri = PathHelper.FilePathToFileUrl(inputPath);
            Uri outputUri = PathHelper.FilePathToFileUrl(outputPath);

            var decompilation = await decompiler.Decompile(inputUri, outputUri);

            foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(fileUri, bicepOutput));
            }

            // to verify success we recompile and check for syntax errors.
            await CompileAsync(decompilation.EntrypointUri.LocalPath, skipRestore: true);

            return decompilation;
        }

        private static ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> GetModuleRestoreDiagnosticsByBicepFile(SourceFileGrouping sourceFileGrouping, ImmutableHashSet<ModuleSourceResolutionInfo> originalModulesToRestore, bool forceModulesRestore)
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
                .ToImmutableDictionary(g => (BicepSourceFile)g.Key, g => g.ToImmutableArray());
        }

        private void LogDiagnostics(Compilation compilation)
        {
            if (compilation is null)
            {
                throw new Exception("Compilation is null. A compilation must exist before logging the diagnostics.");
            }

            LogDiagnostics(compilation.GetAllDiagnosticsByBicepFile());
        }

        private void LogDiagnostics(ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByBicepFile)
        {
            foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
            {
                foreach (var diagnostic in diagnostics)
                {
                    diagnosticLogger.LogDiagnostic(bicepFile.FileUri, diagnostic, bicepFile.LineStarts);
                }
            }
        }
    }
}
