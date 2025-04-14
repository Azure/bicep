// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
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

    public record GetDeploymentDataResponse(bool LocalDeployEnabled, string? TemplateJson = null, string? ParametersJson = null, string? ErrorMessage = null);

    public class GetDeploymentDataHandler : IJsonRpcRequestHandler<GetDeploymentDataRequest, GetDeploymentDataResponse>
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public GetDeploymentDataHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public async Task<GetDeploymentDataResponse> Handle(GetDeploymentDataRequest request, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (this.compilationManager.GetCompilation(request.TextDocument.Uri) is not { } context)
            {
                return new(ErrorMessage: $"Bicep compilation failed. An unexpected error occurred.", LocalDeployEnabled: false);
            }

            var semanticModel = context.Compilation.GetEntrypointSemanticModel();
            var localDeployEnabled = false;

            string? paramsFile = null;
            string? templateFile = null;
            if (semanticModel.Root.FileKind == BicepSourceFileKind.ParamsFile)
            {
                var result = context.Compilation.Emitter.Parameters();

                if (!semanticModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
                {
                    return new(ErrorMessage: $"Compilation failed. Failed to find a file referenced via 'using'.", LocalDeployEnabled: localDeployEnabled);
                }

                paramsFile = result.Parameters;
                templateFile = result.Template?.Template;
                localDeployEnabled = usingModel.TargetScope == ResourceScope.Local;

                if (paramsFile is null ||
                    templateFile is null)
                {
                    return new(ErrorMessage: $"Compilation failed. The Bicep parameters file contains errors.", LocalDeployEnabled: localDeployEnabled);
                }
            }
            else if (semanticModel.Root.FileKind == BicepSourceFileKind.BicepFile)
            {
                var result = context.Compilation.Emitter.Template();
                templateFile = result.Template;
                localDeployEnabled = context.Compilation.GetEntrypointSemanticModel().TargetScope == ResourceScope.Local;

                if (result.Template is null)
                {
                    return new(ErrorMessage: $"Compilation failed. The Bicep file contains errors.", LocalDeployEnabled: localDeployEnabled);
                }
            }

            return new(TemplateJson: templateFile, ParametersJson: paramsFile, LocalDeployEnabled: localDeployEnabled);
        }
    }
}
