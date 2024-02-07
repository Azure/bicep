// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/getDeploymentData", Direction.ClientToServer)]
    public record GetDeploymentDataRequest(TextDocumentIdentifier TextDocument)
        : ITextDocumentIdentifierParams, IRequest<GetDeploymentDataResponse>;

    public record GetDeploymentDataResponse(string? TemplateJson = null, string? ParametersJson = null, string? ErrorMessage = null);

    public class GetDeploymentDataHandler(ICompilationManager compilationManager) : IJsonRpcRequestHandler<GetDeploymentDataRequest, GetDeploymentDataResponse>
    {
        private readonly ICompilationManager compilationManager = compilationManager;

        public async Task<GetDeploymentDataResponse> Handle(GetDeploymentDataRequest request, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (this.compilationManager.GetCompilation(request.TextDocument.Uri) is not { } context)
            {
                return new(ErrorMessage: $"Bicep compilation failed. An unexpected error occurred.");
            }

            var semanticModel = context.Compilation.GetEntrypointSemanticModel();
            if (semanticModel.Root.FileKind == BicepSourceFileKind.ParamsFile)
            {
                var result = context.Compilation.Emitter.Parameters();

                if (result.Parameters is null ||
                    result.Template?.Template is null)
                {
                    return new(ErrorMessage: $"Bicep compilation failed. The Bicep parameters file contains errors.");
                }

                return new(TemplateJson: result.Template.Template, ParametersJson: result.Parameters);
            }
            else
            {
                var result = context.Compilation.Emitter.Template();
                if (result.Template is null)
                {
                    return new(ErrorMessage: $"Bicep compilation failed. The Bicep file contains errors.");
                }

                return new(TemplateJson: result.Template);
            }
        }
    }
}
