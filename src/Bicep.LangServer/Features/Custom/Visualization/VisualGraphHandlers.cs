// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

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

        public VisualGraphHandler(
            ILogger<VisualGraphHandler> logger,
            ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<VisualGraphResult> Handle(VisualGraphParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                // The client keeps what it has; the next document change reconciles once compilation is ready.
                return Task.FromResult(new VisualGraphResult(null, null));
            }

            var graph = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri());

            var targetScope = context.Compilation.GetEntrypointSemanticModel().TargetScope switch
            {
                ResourceScope.ResourceGroup => LanguageConstants.TargetScopeTypeResourceGroup,
                ResourceScope.Subscription => LanguageConstants.TargetScopeTypeSubscription,
                ResourceScope.ManagementGroup => LanguageConstants.TargetScopeTypeManagementGroup,
                ResourceScope.Tenant => LanguageConstants.TargetScopeTypeTenant,
                _ => null,
            };

            return Task.FromResult(new VisualGraphResult(graph, targetScope));
        }
    }

    /// <summary>
    /// Handles <c>textDocument/visualGraphLayout</c>: validates the measured graph against the live compilation,
    /// runs MSAGL with actual client-measured sizes, and returns node positions and the graph's bounds.
    /// </summary>
    public class VisualGraphLayoutHandler : IJsonRpcRequestHandler<VisualGraphLayoutParams, VisualGraphLayoutResult>
    {
        private readonly ILogger<VisualGraphLayoutHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualGraphLayoutEngine layoutEngine;

        public VisualGraphLayoutHandler(
            ILogger<VisualGraphLayoutHandler> logger,
            ICompilationManager compilationManager,
            IVisualGraphLayoutEngine layoutEngine)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.layoutEngine = layoutEngine;
        }

        public async Task<VisualGraphLayoutResult> Handle(VisualGraphLayoutParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph layout request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            var target = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri());

            if (!VisualGraphTopology.Matches(request.Graph, target))
            {
                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            var nodeSizes = request.Graph.Nodes.ToDictionary(node => node.Id, node => new NodeSize(node.Width, node.Height), StringComparer.Ordinal);
            var options = request.Options ?? VisualGraphLayoutOptions.Default;

            // Offload the CPU-bound MSAGL layout so a pathological graph cannot block the request dispatch
            // thread; cancellation flows through to abandon a layout the client no longer cares about.
            var layout = await Task.Run(() => this.layoutEngine.Layout(target, nodeSizes, options, cancellationToken), cancellationToken);

            if (target.Nodes.Count > 0 && layout.Positions.Count == 0)
            {
                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.LayoutFailed, [], null);
            }

            var positions = target.Nodes
                .OrderBy(node => node.Id, StringComparer.Ordinal)
                .Where(node => layout.Positions.ContainsKey(node.Id))
                .Select(node => new NodePosition(node.Id, layout.Positions[node.Id].X, layout.Positions[node.Id].Y))
                .ToArray();

            return new VisualGraphLayoutResult(VisualGraphLayoutStatus.Ok, positions, layout.Bounds);
        }
    }

    /// <summary>
    /// Handles <c>textDocument/visualGraphNodeSource</c>: maps a node id to its source location from the live
    /// compilation, so the webview can reveal a node on demand without the graph carrying volatile range/file
    /// data. Returns a null location when the node no longer exists.
    /// </summary>
    public class VisualGraphNodeSourceHandler : IJsonRpcRequestHandler<VisualGraphNodeSourceParams, VisualGraphNodeSourceResult>
    {
        private readonly ILogger<VisualGraphNodeSourceHandler> logger;

        private readonly ICompilationManager compilationManager;

        public VisualGraphNodeSourceHandler(
            ILogger<VisualGraphNodeSourceHandler> logger,
            ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<VisualGraphNodeSourceResult> Handle(VisualGraphNodeSourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph node source request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new VisualGraphNodeSourceResult(FilePath: null, Range: null));
            }

            var (_, sources) = VisualGraphBuilder.BuildWithSources(context, request.TextDocument.Uri.ToIOUri());

            if (!sources.TryGetValue(request.NodeId, out var source))
            {
                return Task.FromResult(new VisualGraphNodeSourceResult(FilePath: null, Range: null));
            }

            return Task.FromResult(new VisualGraphNodeSourceResult(source.FilePath, source.Range));
        }
    }
}
