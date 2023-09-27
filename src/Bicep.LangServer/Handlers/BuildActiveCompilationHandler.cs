// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BuildActiveCompilationParams(DocumentUri BicepUri);
    public record BuildActiveCompilationResult(string? Output);

    public class BuildActiveCompilationHandler : ExecuteTypedResponseCommandHandlerBase<BuildActiveCompilationParams, BuildActiveCompilationResult>
    {
        private readonly ICompilationManager compilationManager;

        public BuildActiveCompilationHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base(LangServerConstants.BuildActiveCompilationCommand, serializer)
        {
            this.compilationManager = compilationManager;
        }

        public override async Task<BuildActiveCompilationResult> Handle(BuildActiveCompilationParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            if (compilationManager.GetCompilation(request.BicepUri) is not {} context) {
                return new(null);
            }

            var emitter = new TemplateEmitter(context.Compilation.GetEntrypointSemanticModel());

            var stringWriter = new StringWriter();
            var emitResult = emitter.Emit(stringWriter);

            var output = emitResult.Status != EmitStatus.Failed ? stringWriter.ToString() : null;

            return new(output);
        }
    }
}
