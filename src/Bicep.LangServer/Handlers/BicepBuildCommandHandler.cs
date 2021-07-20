// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepBuildCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly ICompilationManager CompilationManager;

        public BicepBuildCommandHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base("build", serializer)
        {
            CompilationManager = compilationManager;
        }

        public override Task<string> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Build started...");

            string? compiledFilePath = GetCompiledFilePath(bicepFilePath, sb, out string compiledFileName);

            DocumentUri uri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var context = CompilationManager.GetCompilation(uri);

            if (context == null)
            {
                throw new InvalidOperationException($"Unable to get compilation context");
            }

            if (!string.IsNullOrWhiteSpace(compiledFilePath))
            {
                var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using (fileStream)
                {
                    var emitter = new TemplateEmitter(context.Compilation.GetEntrypointSemanticModel(), string.Empty);
                    var result = emitter.Emit(fileStream);

                    if (result.Diagnostics.Any())
                    {
                        sb.AppendLine("Build failed. Input file has errors. Please fix the errors and try build command again");
                    }
                    else
                    {
                        sb.AppendLine("Build succeeded. Created compiled file: " + compiledFileName);
                    }
                }
            }

            return Task.FromResult<string>(sb.ToString());
        }

        private string? GetCompiledFilePath(string bicepFilePath, StringBuilder sb, out string compiledFileName)
        {
            string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(bicepFilePath);
            string? fileName = Path.GetFileName(bicepFilePath);
            compiledFileName = string.Empty;

            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension) || string.IsNullOrWhiteSpace(fileName))
            {
                compiledFileName = fileNameWithoutExtension + ".json";
                sb.AppendLine("bicep build " + fileName);
                string? folder = Path.GetDirectoryName(bicepFilePath);

                if (!string.IsNullOrWhiteSpace(folder))
                {
                    return Path.Combine(folder, compiledFileName);
                }
            }

            sb.AppendLine("Unable to find file name or folder of the specified path " + bicepFilePath);
            return null;
        }
    }
}
