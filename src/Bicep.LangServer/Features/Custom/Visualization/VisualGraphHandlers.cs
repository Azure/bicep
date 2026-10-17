// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
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

        private readonly IVisualModuleSourceProvider moduleSources;

        public VisualGraphHandler(
            ILogger<VisualGraphHandler> logger,
            ICompilationManager compilationManager,
            IVisualModuleSourceProvider moduleSources)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.moduleSources = moduleSources;
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

            var graph = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri(), this.moduleSources);

            var model = context.Compilation.GetEntrypointSemanticModel();
            var targetScope = model.TargetScope switch
            {
                ResourceScope.ResourceGroup => LanguageConstants.TargetScopeTypeResourceGroup,
                ResourceScope.Subscription => LanguageConstants.TargetScopeTypeSubscription,
                ResourceScope.ManagementGroup => LanguageConstants.TargetScopeTypeManagementGroup,
                ResourceScope.Tenant => LanguageConstants.TargetScopeTypeTenant,
                _ => null,
            };

            return Task.FromResult(new VisualGraphResult(graph, targetScope, model.GetAllDiagnostics().Count(diagnostic => diagnostic.IsError())));
        }
    }

    /// <summary>
    /// Handles <c>textDocument/visualGraphLayout</c>: validates the measured graph against the live compilation,
    /// runs MSAGL with actual client-measured sizes, and returns node positions and the graph's bounds. With a root
    /// node, it lays out only that expanded module and its contents.
    /// </summary>
    public class VisualGraphLayoutHandler : IJsonRpcRequestHandler<VisualGraphLayoutParams, VisualGraphLayoutResult>
    {
        private readonly ILogger<VisualGraphLayoutHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualGraphLayoutEngine layoutEngine;

        private readonly IVisualModuleSourceProvider moduleSources;

        public VisualGraphLayoutHandler(
            ILogger<VisualGraphLayoutHandler> logger,
            ICompilationManager compilationManager,
            IVisualGraphLayoutEngine layoutEngine,
            IVisualModuleSourceProvider moduleSources)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.layoutEngine = layoutEngine;
            this.moduleSources = moduleSources;
        }

        public async Task<VisualGraphLayoutResult> Handle(VisualGraphLayoutParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph layout request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            var sourceGraph = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri(), this.moduleSources);
            var projected = VisualGraphTopology.Project(
                sourceGraph,
                request.CollapsedNodeIds?.ToHashSet(StringComparer.Ordinal) ?? new HashSet<string>(StringComparer.Ordinal));
            var target = request.RootNodeId is { } rootNodeId ? VisualGraphTopology.Subtree(projected, rootNodeId) : projected;

            if (target is null || !VisualGraphTopology.Matches(request.Graph, target))
            {
                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            // A subtree is laid out as a graph of its own, with the module at the top level.
            var layoutTarget = request.RootNodeId is null
                ? target
                : target with { Nodes = [.. target.Nodes.Select(node => node.Id == request.RootNodeId ? node with { ParentId = null } : node)] };

            var nodeSizes = request.Graph.Nodes.ToDictionary(node => node.Id, node => new NodeSize(node.Width, node.Height), StringComparer.Ordinal);
            var options = request.Options ?? VisualGraphLayoutOptions.Default;

            // Offload the CPU-bound MSAGL layout so a pathological graph cannot block the request dispatch
            // thread; cancellation flows through to abandon a layout the client no longer cares about.
            var layout = await Task.Run(() => this.layoutEngine.Layout(layoutTarget, nodeSizes, options, cancellationToken), cancellationToken);

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

        private readonly IVisualModuleSourceProvider moduleSources;

        public VisualGraphNodeSourceHandler(
            ILogger<VisualGraphNodeSourceHandler> logger,
            ICompilationManager compilationManager,
            IVisualModuleSourceProvider moduleSources)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.moduleSources = moduleSources;
        }

        public Task<VisualGraphNodeSourceResult> Handle(VisualGraphNodeSourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph node source request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new VisualGraphNodeSourceResult(Uri: null, Range: null));
            }

            var sources = VisualGraphBuilder.BuildWithSources(context, request.TextDocument.Uri.ToIOUri(), this.moduleSources).Sources;

            if (!sources.TryGetValue(request.NodeId, out var source))
            {
                return Task.FromResult(new VisualGraphNodeSourceResult(Uri: null, Range: null));
            }

            return Task.FromResult(new VisualGraphNodeSourceResult(source.Uri, source.Range));
        }
    }

    /// <summary>
    /// Handles <c>textDocument/visualGraphRestoreModuleSource</c>: restores the registry module behind a remote module
    /// node into the local module cache, so its published source can be shown. Restoring never edits the document.
    /// </summary>
    public class VisualGraphRestoreModuleSourceHandler : IJsonRpcRequestHandler<VisualGraphRestoreModuleSourceParams, VisualGraphRestoreModuleSourceResult>
    {
        private readonly ILogger<VisualGraphRestoreModuleSourceHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualModuleSourceProvider moduleSources;

        private readonly IModuleDispatcher moduleDispatcher;

        public VisualGraphRestoreModuleSourceHandler(
            ILogger<VisualGraphRestoreModuleSourceHandler> logger,
            ICompilationManager compilationManager,
            IVisualModuleSourceProvider moduleSources,
            IModuleDispatcher moduleDispatcher)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.moduleSources = moduleSources;
            this.moduleDispatcher = moduleDispatcher;
        }

        public async Task<VisualGraphRestoreModuleSourceResult> Handle(VisualGraphRestoreModuleSourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph module restore request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return new(VisualGraphRestoreModuleSourceStatus.NotFound);
            }

            var moduleReferences = VisualGraphBuilder.BuildWithSources(context, request.TextDocument.Uri.ToIOUri(), this.moduleSources).ModuleReferences;

            if (!moduleReferences.TryGetValue(request.NodeId, out var reference))
            {
                return new(VisualGraphRestoreModuleSourceStatus.NotFound);
            }

            // Not forced: a module already in the cache is left alone, and one whose restore failed is retried.
            await this.moduleDispatcher.RestoreArtifacts([reference], forceRestore: false);

            // The dispatcher keeps a failure on record for a while even after a later restore succeeds, so the cache
            // is the authority on the outcome.
            if (!reference.ModuleMainTemplateFile.Exists())
            {
                var message = this.moduleDispatcher.GetArtifactRestoreStatus(reference, out var failureBuilder) == ArtifactRestoreStatus.Failed &&
                    failureBuilder is not null
                    ? failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message
                    : $"Unable to restore module \"{reference.FullyQualifiedReference}\".";

                return new(VisualGraphRestoreModuleSourceStatus.Failed, message);
            }

            if (context.Compilation.SourceFileGrouping.ArtifactLookup.Values.Any(artifact => reference.Equals(artifact.Reference)))
            {
                // The document references the module itself, so its compilation (and diagnostics) depended on the restore.
                this.compilationManager.RefreshCompilation(request.TextDocument.Uri);
            }

            return new(VisualGraphRestoreModuleSourceStatus.Restored);
        }
    }
}
