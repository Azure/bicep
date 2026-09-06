// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public class VisualResourceTypeNamespacesHandler : IJsonRpcRequestHandler<VisualResourceTypeNamespacesParams, VisualResourceTypeNamespacesResult>
    {
        private readonly ILogger<VisualResourceTypeNamespacesHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualResourceCreationService visualResourceCreationService;

        public VisualResourceTypeNamespacesHandler(
            ILogger<VisualResourceTypeNamespacesHandler> logger,
            ICompilationManager compilationManager,
            IVisualResourceCreationService visualResourceCreationService)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.visualResourceCreationService = visualResourceCreationService;
        }

        public Task<VisualResourceTypeNamespacesResult> Handle(VisualResourceTypeNamespacesParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual resource type namespaces request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, $"The document \"{request.TextDocument.Uri}\" is not currently compiled.");
            }

            var model = context.Compilation.GetEntrypointSemanticModel();
            return Task.FromResult(this.visualResourceCreationService.GetResourceTypeNamespaces(model, request.IncludePreview));
        }
    }
}
