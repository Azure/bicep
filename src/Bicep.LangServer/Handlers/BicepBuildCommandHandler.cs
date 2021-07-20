// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Emit;
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

            FileStream fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            TemplateEmitter emitter = new TemplateEmitter(context.Compilation.GetEntrypointSemanticModel(), ThisAssembly.AssemblyFileVersion);
            EmitResult result = emitter.Emit(fileStream);

            if (result.Diagnostics.Any())
            {
                sb.AppendLine("Build failed. Please fix errors in " + bicepFile);
            }
            else
            {
                sb.AppendLine("Build succeeded. Created compiled file: " + compiledFile);
            }

            return sb.ToString();
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
