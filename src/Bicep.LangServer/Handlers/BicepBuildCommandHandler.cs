// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to generate compiled .json file for given a bicep file path.
    // It returns build succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepBuildCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly ICompilationManager CompilationManager;

        public BicepBuildCommandHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base(LanguageConstants.Build, serializer)
        {
            CompilationManager = compilationManager;
        }

        public override Task<string> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            string buildOutput = GenerateCompiledFileAndReturnBuildOutputMessage(bicepFilePath, documentUri);

            return Task.FromResult(buildOutput);
        }

        private string GenerateCompiledFileAndReturnBuildOutputMessage(string bicepFilePath, DocumentUri documentUri)
        {
            string compiledFilePath = PathHelper.GetDefaultBuildOutputPath(bicepFilePath);
            string compiledFile = Path.GetFileName(compiledFilePath);

            if (File.Exists(compiledFilePath))
            {
                return "Build failed. " + compiledFile + " already exists.";
            }

            CompilationContext? context = CompilationManager.GetCompilation(documentUri);

            if (context is null)
            {
                throw new InvalidOperationException($"Unable to get compilation context");
            }

            SemanticModel semanticModel = context.Compilation.GetEntrypointSemanticModel();
            KeyValuePair<BicepFile, IEnumerable<IDiagnostic>> diagnosticsByFile = context.Compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == documentUri.ToUri());

            if (diagnosticsByFile.Value.Any())
            {
                return GetDiagnosticsMessage(diagnosticsByFile);
            }

            using (FileStream fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                TemplateEmitter emitter = new TemplateEmitter(semanticModel, ThisAssembly.AssemblyFileVersion);
                EmitResult result = emitter.Emit(fileStream);

                return "Build succeeded. Created file " + compiledFile;
            }
        }

        private string GetDiagnosticsMessage(KeyValuePair<BicepFile, IEnumerable<IDiagnostic>> diagnosticsByFile)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Build failed. Please fix below errors:");

            IReadOnlyList<int> lineStarts = diagnosticsByFile.Key.LineStarts;

            foreach (IDiagnostic diagnostic in diagnosticsByFile.Value)
            {
                (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

                // Build a code description link if the Uri is assigned
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                sb.AppendLine($"{diagnosticsByFile.Key.FileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
            }

            return sb.ToString();
        }
    }
}
