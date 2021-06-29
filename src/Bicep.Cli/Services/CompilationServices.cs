// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Collections.Immutable;
using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Newtonsoft.Json;

namespace Bicep.Cli.Services
{
    public class CompilationService
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly FileResolver fileResolver;
        private readonly InvocationContext invocationContext;
        private readonly Workspace workspace;

        public CompilationService(IDiagnosticLogger diagnosticLogger, InvocationContext invocationContext)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.fileResolver = new FileResolver();
            this.invocationContext = invocationContext;
            this.workspace = new Workspace();

            this.IsSuccess = true;
        }

        internal bool IsSuccess { get; set; }

        private Compilation? Compilation { get; set; }

        private (Uri, ImmutableDictionary<Uri, string>)? Decompilation { get; set; }

        public CompilationService Compile(string inputPath = "")
        {
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(this.fileResolver, this.workspace, inputUri);

            Compilation = new Compilation(this.invocationContext.ResourceTypeProvider, syntaxTreeGrouping);
            return this;
        }  

        public CompilationService PrintCompilationOnSuccess()
        {
            if (this.Compilation is null)
            {
                throw new Exception("Compilation is null. Please ensure the file has been compiled.");
            }

            if (this.IsSuccess is false)
            {
                return this; // don't print compilation is it wasn't a success.
            }

            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateEmitter(Compilation.GetEntrypointSemanticModel(), invocationContext.AssemblyFileVersion);

            emitter.Emit(writer);

            return this;
        }

        public CompilationService WriteCompilationFileOnSuccess(string outputPath)
        {
            if (this.Compilation is null)
            {
                throw new Exception("Compilation is null. Please ensure the file has been compiled.");
            }

            if (this.IsSuccess is false)
            {
                return this; // don't write file if compilation was not a success.
            }

            var emitter = new TemplateEmitter(Compilation.GetEntrypointSemanticModel(), invocationContext.AssemblyFileVersion);

            using var outputStream = CreateFileStream(outputPath);

            emitter.Emit(outputStream);

            return this;
        }

        public CompilationService Decompile(string inputPath, string outputPath = "")
        {
            Uri inputUri = PathHelper.FilePathToFileUrl(inputPath);
            
            Uri outputUri = string.IsNullOrEmpty(outputPath) 
                ? PathHelper.ChangeToBicepExtension(inputUri) 
                : PathHelper.FilePathToFileUrl(outputPath);
            
            Decompilation = TemplateDecompiler.DecompileFileWithModules(this.invocationContext.ResourceTypeProvider, new FileResolver(), inputUri, outputUri);

            return this;
        }

        public CompilationService CompileDecompilationOutput()
        {
            if (this.Decompilation is null)
            {
                throw new Exception("Compilation is null. Please ensure the file has been compiled.");
            }

            var (entryPointUri, _) = this.Decompilation.Value;

            this.Compile(entryPointUri.AbsolutePath);

            return this;
        }

        public CompilationService WriteDecompilationFile()
        {
            if (this.Decompilation is null)
            {
                throw new Exception("Decompilation is null. Please ensure the file has been decompiled.");
            }

            var (entryPointUri, filesToSave) = this.Decompilation.Value;

            foreach (var (fileUri, bicepOutput) in filesToSave)
            {
                File.WriteAllText(fileUri.LocalPath, bicepOutput);

                workspace.UpsertSyntaxTrees(SyntaxTree.Create(fileUri, bicepOutput).AsEnumerable());
            }

            return this;
        }

        public CompilationService PrintDecompilation()
        {
            if (this.Decompilation is null)
            {
                throw new Exception("Decompilation is null. Please ensure the file has been decompiled.");
            }

            var (entryPointUri, filesToSave) = this.Decompilation.Value;

            foreach (var (fileUri, bicepOutput) in filesToSave)
            {
                this.invocationContext.OutputWriter.Write(bicepOutput);

                workspace.UpsertSyntaxTrees(SyntaxTree.Create(fileUri, bicepOutput).AsEnumerable());
            }

            return this;
        }
        public bool GetResult()
        {
            return IsSuccess;
        }

        public CompilationService LogDiagnostics()
        {
            if (this.Compilation is null)
            {
                throw new Exception("Compilation is null. A compulation must exist before logging the diagnostics.");
            }

            foreach (var (syntaxTree, diagnostics) in this.Compilation.GetAllDiagnosticsBySyntaxTree())
            {
                foreach (var diagnostic in diagnostics)
                {
                    diagnosticLogger.LogDiagnostic(syntaxTree.FileUri, diagnostic, syntaxTree.LineStarts);
                    IsSuccess &= diagnostic.Level != DiagnosticLevel.Error;
                }
            }

            return this;
        }

        private static FileStream CreateFileStream(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }
        }
    }
}