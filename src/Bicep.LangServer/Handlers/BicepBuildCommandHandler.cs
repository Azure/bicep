// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepBuildCommandHandler : ExecuteTypedCommandHandlerBase<string>
    {
        private readonly ICompilationManager CompilationManager;

        public BicepBuildCommandHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base("build", serializer)
        {
            CompilationManager = compilationManager;
        }

        public override Task<Unit> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            DocumentUri uri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var context = CompilationManager.GetCompilation(uri);

            if (context == null)
            {
                throw new InvalidOperationException($"Unable to get compilaction context");
            }

            string? compiledFilePath = GetCompiledFilePath(bicepFilePath);

            if (!string.IsNullOrWhiteSpace(compiledFilePath))
            {
                var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using (fileStream)
                {
                    var emitter = new TemplateEmitter(context.Compilation.GetEntrypointSemanticModel(), string.Empty);
                    var result = emitter.Emit(fileStream);

                    if (result.Diagnostics.Any())
                    {
                        throw new Exception("Stuff went wrong");
                    }
                }
            }

            return Unit.Task;
        }

        private string? GetCompiledFilePath(string bicepFilePath)
        {
            string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(bicepFilePath);

            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                string? folder = Path.GetDirectoryName(bicepFilePath);

                if (!string.IsNullOrWhiteSpace(folder))
                {
                    return Path.Combine(folder, fileNameWithoutExtension + ".json");
                }
            }

            return null;
        }
    }
}
