// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualResourceTypes</c>: returns a paged, filtered catalog of the resource
    /// types available for the Az namespace in the live compilation of the active document.
    /// </summary>
    public class VisualResourceTypesHandler : IJsonRpcRequestHandler<VisualResourceTypesParams, VisualResourceTypesResult>
    {
        private readonly ILogger<VisualResourceTypesHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualResourceCreationService visualResourceCreationService;

        public VisualResourceTypesHandler(
            ILogger<VisualResourceTypesHandler> logger,
            ICompilationManager compilationManager,
            IVisualResourceCreationService visualResourceCreationService)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.visualResourceCreationService = visualResourceCreationService;
        }

        public Task<VisualResourceTypesResult> Handle(VisualResourceTypesParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual resource types request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, $"The document \"{request.TextDocument.Uri}\" is not currently compiled.");
            }

            var model = context.Compilation.GetEntrypointSemanticModel();
            var result = this.visualResourceCreationService.GetResourceTypes(
                model,
                request.ProviderNamespace,
                request.Query,
                request.IncludePreview,
                request.PageSize,
                request.ContinuationToken);

            return Task.FromResult(result);
        }
    }
}
