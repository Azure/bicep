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
    public class BicepBuildCommandHandler : ExecuteTypedCommandHandlerBase<string, Stream>
    {
        private readonly ICompilationManager CompilationManager;

        public BicepBuildCommandHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base("build", serializer)
        {
            CompilationManager = compilationManager;
        }

        public override Task<Unit> Handle(string documentUri, Stream stream, CancellationToken cancellationToken)
        {
            var context = CompilationManager.GetCompilation(DocumentUri.Parse(documentUri));

            if (context == null)
            {
                throw new InvalidOperationException($"Unable to get compilaction context");
            }

            var emitter = new TemplateEmitter(context.Compilation.GetEntrypointSemanticModel(), string.Empty);
            var result = emitter.Emit(stream);

            if (result.Diagnostics.Any())
            {
                throw new Exception("Stuff went wrong");
            }

            return Unit.Task;
        }
    }
}
