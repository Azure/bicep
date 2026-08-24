// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.LanguageServer.Compilation;
using MediatR;
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
                request.Query,
                request.IncludePreview,
                request.PageSize,
                request.ContinuationToken);

            return Task.FromResult(result);
        }
    }

    /// <summary>
    /// Handles <c>textDocument/prepareVisualResource</c>: generates a new top-level resource declaration for
    /// the requested resource type and returns a versioned <see cref="OmniSharp.Extensions.LanguageServer.Protocol.Models.WorkspaceEdit"/>
    /// that inserts it into the active document. The client applies the edit itself (subject to its own version
    /// checks) rather than the server pushing it via <c>workspace/applyEdit</c>, since the visual designer needs
    /// the generated symbolic name and unresolved-property metadata alongside the edit.
    /// </summary>
    public class PrepareVisualResourceHandler : IJsonRpcRequestHandler<PrepareVisualResourceParams, PrepareVisualResourceResult>
    {
        private readonly ILogger<PrepareVisualResourceHandler> logger;

        private readonly BicepCompiler compiler;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualResourceCreationService visualResourceCreationService;

        public PrepareVisualResourceHandler(
            ILogger<PrepareVisualResourceHandler> logger,
            BicepCompiler compiler,
            ICompilationManager compilationManager,
            IVisualResourceCreationService visualResourceCreationService)
        {
            this.logger = logger;
            this.compiler = compiler;
            this.compilationManager = compilationManager;
            this.visualResourceCreationService = visualResourceCreationService;
        }

        public Task<PrepareVisualResourceResult> Handle(PrepareVisualResourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Prepare visual resource request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, $"The document \"{request.TextDocument.Uri}\" is not currently compiled.");
            }

            try
            {
                var result = this.visualResourceCreationService.PrepareResource(this.compiler, context, request);

                return Task.FromResult(result);
            }
            catch (VisualResourceCreationException exception)
            {
                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, exception.Message);
            }
        }
    }
}
