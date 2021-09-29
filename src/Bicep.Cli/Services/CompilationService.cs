// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using System;
using System.Collections.Immutable;
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

        public CompilationService(
            IDiagnosticLogger diagnosticLogger,
            IFileResolver fileResolver,
            InvocationContext invocationContext,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            TemplateDecompiler decompiler)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.invocationContext = invocationContext;
            this.workspace = new Workspace();
            this.decompiler = decompiler;
        }

        public async Task RestoreAsync(string inputPath)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri);

            // restore is supposed to only restore the module references that are valid
            // and not log any other errors
            await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore));
        }

        public async Task<Compilation> CompileAsync(string inputPath, bool skipRestore)
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, this.workspace, inputUri);
            if (!skipRestore)
            {
                // module references in the file may be malformed
                // however we still want to surface as many errors as we can for the module refs that are valid
                // so we will try to restore modules with valid refs and skip everything else
                // (the diagnostics will be collected during compilation)
                if (await moduleDispatcher.RestoreModules(moduleDispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
                {
                    // modules had to be restored - recompile
                    sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, this.workspace, sourceFileGrouping);
                }
            }

            var configuration = this.configurationManager.GetConfiguration(inputUri);
            var compilation = new Compilation(this.invocationContext.NamespaceProvider, sourceFileGrouping, configuration);

            LogDiagnostics(compilation);

            return compilation;
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

        private void LogDiagnostics(Compilation compilation)
        {
            if (compilation is null)
            {
                throw new Exception("Compilation is null. A compilation must exist before logging the diagnostics.");
            }

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                foreach (var diagnostic in diagnostics)
                {
                    diagnosticLogger.LogDiagnostic(bicepFile.FileUri, diagnostic, bicepFile.LineStarts);
                }
            }
        }
    }
}
