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
            StringBuilder sb = new StringBuilder();

            string? compiledFilePath = GetCompiledFilePath(bicepFilePath, sb, out string bicepFile, out string compiledFile);

            if (string.IsNullOrWhiteSpace(compiledFilePath))
            {
                throw new ArgumentException($"Invalid input file");
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
                AppendDiagnosticsMessage(diagnosticsByFile, bicepFile, sb);
                return sb.ToString();
            }

            using (FileStream fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                TemplateEmitter emitter = new TemplateEmitter(semanticModel, ThisAssembly.AssemblyFileVersion);
                EmitResult result = emitter.Emit(fileStream);

                sb.AppendLine("Build succeeded. Created transpiled ARM template: " + compiledFile);
            }

            return sb.ToString();
        }

        private void AppendDiagnosticsMessage(KeyValuePair<BicepFile, IEnumerable<IDiagnostic>> diagnosticsByFile, string bicepFile, StringBuilder sb)
        {
            sb.AppendLine("Build failed. Please fix below errors in " + bicepFile + " :");

            IEnumerable<IDiagnostic> diagnostics = diagnosticsByFile.Value;
            IReadOnlyList<int> lineStarts = diagnosticsByFile.Key.LineStarts;

            for (int i = 0; i < diagnostics.Count(); i++)
            {
                IDiagnostic diagnostic = diagnostics.ElementAt(i);
                var (startLine, startChar) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

                sb.AppendLine((i+1) + ". " + diagnostic.Message + " bicep(" + diagnostic.Code + "). " + "[" + (startLine + 1) + ", " + (startChar + 1) + "]");
            }
        }

        private string? GetCompiledFilePath(string bicepFilePath, StringBuilder sb, out string bicepFile, out string compiledFile)
        {
            compiledFile = string.Empty;
            bicepFile = Path.GetFileName(bicepFilePath);

            string? bicepFileWithoutExtension = Path.GetFileNameWithoutExtension(bicepFilePath);
            string? folder = Path.GetDirectoryName(bicepFilePath);

            if (string.IsNullOrWhiteSpace(bicepFileWithoutExtension) || string.IsNullOrWhiteSpace(folder))
            {
                return null;
            }

            sb.AppendLine("Build started...");
            sb.AppendLine("bicep build " + bicepFile);

            compiledFile = bicepFileWithoutExtension + ".json";

            return Path.Combine(folder, compiledFile);
        }
    }
}
