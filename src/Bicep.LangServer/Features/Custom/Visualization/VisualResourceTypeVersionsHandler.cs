// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public class VisualResourceTypeVersionsHandler : IJsonRpcRequestHandler<VisualResourceTypeVersionsParams, VisualResourceTypeVersionsResult>
    {
        private readonly ILogger<VisualResourceTypeVersionsHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualResourceCreationService visualResourceCreationService;

        public VisualResourceTypeVersionsHandler(
            ILogger<VisualResourceTypeVersionsHandler> logger,
            ICompilationManager compilationManager,
            IVisualResourceCreationService visualResourceCreationService)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.visualResourceCreationService = visualResourceCreationService;
        }

        public Task<VisualResourceTypeVersionsResult> Handle(VisualResourceTypeVersionsParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual resource type versions request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);
                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, $"The document \"{request.TextDocument.Uri}\" is not currently compiled.");
            }

            try
            {
                var model = context.Compilation.GetEntrypointSemanticModel();
                return Task.FromResult(this.visualResourceCreationService.GetResourceTypeVersions(
                    model, request.FullyQualifiedType));
            }
            catch (VisualResourceCreationException exception)
            {
                this.logger.LogError(exception, "Visual resource type versions request failed for {ResourceType}.", request.FullyQualifiedType);
                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, exception.Message);
            }
        }
    }
}
