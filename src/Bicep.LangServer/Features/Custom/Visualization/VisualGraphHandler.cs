// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualGraph</c>: builds the canonical graph from the live compilation. Layout is
    /// computed later by <see cref="VisualGraphLayoutHandler"/> after the client renders and measures nodes.
    /// </summary>
    public class VisualGraphHandler : IJsonRpcRequestHandler<VisualGraphParams, VisualGraphResult>
    {
        private readonly ILogger<VisualGraphHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly VisualGraphBuilder graphBuilder;

        public VisualGraphHandler(
            ILogger<VisualGraphHandler> logger,
            ICompilationManager compilationManager,
            VisualGraphBuilder graphBuilder)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.graphBuilder = graphBuilder;
        }

        public Task<VisualGraphResult> Handle(VisualGraphParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                // The client keeps what it has; the next document change reconciles once compilation is ready.
                return Task.FromResult(new VisualGraphResult(null, null, 0));
            }

            var graph = this.graphBuilder.Build(context).Graph;

            var model = context.Compilation.GetEntrypointSemanticModel();
            var targetScope = model.TargetScope switch
            {
                ResourceScope.ResourceGroup => LanguageConstants.TargetScopeTypeResourceGroup,
                ResourceScope.Subscription => LanguageConstants.TargetScopeTypeSubscription,
                ResourceScope.ManagementGroup => LanguageConstants.TargetScopeTypeManagementGroup,
                ResourceScope.Tenant => LanguageConstants.TargetScopeTypeTenant,
                _ => null,
            };

            return Task.FromResult(new VisualGraphResult(
                graph,
                targetScope,
                model.GetAllDiagnostics().Count(diagnostic => diagnostic.IsError()),
                graph.GetRevision()));
        }
    }
}
