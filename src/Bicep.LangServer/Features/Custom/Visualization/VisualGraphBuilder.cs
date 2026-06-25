// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Builds the canonical <see cref="CanonicalGraph"/> from a Bicep compilation. The traversal mirrors
    /// <see cref="Bicep.LanguageServer.Handlers.BicepDeploymentGraphHandler"/> (the compatibility path) but
    /// emits the richer canonical model: nodes carry a <c>kind</c>, a <c>parentId</c>, and a symbol name, and
    /// edges carry a stable id. No layout is computed here; positions are added later by the layout engine.
    /// </summary>
    public static class VisualGraphBuilder
    {
        public static CanonicalGraph Build(CompilationContext context, IOUri entryFileUri) =>
            BuildWithSources(context, entryFileUri).Graph;

        /// <summary>
        /// Builds the canonical graph together with a map from node id to its source location. The source map
        /// is consumed only by the reveal-on-demand handler; the canonical graph itself carries no source
        /// location so that volatile range/file-path data never travels through the graph diff.
        /// </summary>
        public static (CanonicalGraph Graph, IReadOnlyDictionary<string, NodeSource> Sources) BuildWithSources(
            CompilationContext context,
            IOUri entryFileUri)
        {
            var nodes = new List<GraphNode>();
            var edges = new List<GraphEdge>();
            var sources = new Dictionary<string, NodeSource>(StringComparer.Ordinal);

            var queue = new Queue<(SemanticModel Model, IOUri FileUri, string? ParentId)>();
            var entrySemanticModel = context.Compilation.GetEntrypointSemanticModel();

            queue.Enqueue((entrySemanticModel, entryFileUri, null));

            while (queue.Count > 0)
            {
                var (semanticModel, fileUri, parentId) = queue.Dequeue();
                var nodesBySymbol = new Dictionary<DeclaredSymbol, GraphNode>();
                var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(semanticModel)
                    .Where(x => x.Key.Name != LanguageConstants.MissingName && x.Key.Name != LanguageConstants.ErrorName)
                    .ToImmutableDictionary(x => x.Key, x => x.Value);

                var errors = semanticModel.GetAllDiagnostics().Where(x => x.IsError()).ToList();

                foreach (var symbol in dependenciesBySymbol.Keys)
                {
                    var id = parentId is null ? symbol.Name : $"{parentId}::{symbol.Name}";

                    if (symbol is ResourceSymbol resourceSymbol)
                    {
                        var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FormatType() ?? "<unknown>";
                        var resourceSpan = resourceSymbol.DeclaringResource.Span;
                        var range = resourceSpan.ToRange(semanticModel.SourceFile.LineStarts);
                        var hasError = errors.Any(error => TextSpan.AreOverlapping(resourceSpan, error.Span));

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Resource,
                            ParentId: parentId,
                            Type: resourceType,
                            SymbolName: symbol.Name,
                            IsCollection: resourceSymbol.IsCollection,
                            HasChildren: false,
                            HasError: hasError);
                        sources[id] = new NodeSource(fileUri, range);
                    }

                    if (symbol is ModuleSymbol moduleSymbol)
                    {
                        var directoryUri = fileUri.Resolve(".");
                        var moduleRelativePath = moduleSymbol.DeclaringModule.TryGetPath()?.TryGetLiteralValue();
                        var moduleFileUri = moduleRelativePath is not null
                            ? directoryUri.Resolve(moduleRelativePath)
                            : null;

                        var moduleSpan = moduleSymbol.DeclaringModule.Span;
                        var range = moduleSpan.ToRange(semanticModel.SourceFile.LineStarts);
                        var hasError = errors.Any(error => TextSpan.AreOverlapping(moduleSpan, error.Span));

                        var hasChildren = false;

                        if (moduleFileUri is not null &&
                            moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel, out var _) &&
                            moduleSemanticModel is SemanticModel bicepModel &&
                            (bicepModel.Root.ResourceDeclarations.Any() || bicepModel.Root.ModuleDeclarations.Any()))
                        {
                            hasChildren = true;
                            queue.Enqueue((bicepModel, moduleFileUri, id));
                        }

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Module,
                            ParentId: parentId,
                            Type: "<module>",
                            SymbolName: symbol.Name,
                            IsCollection: moduleSymbol.IsCollection,
                            HasChildren: hasChildren,
                            HasError: hasError);
                        sources[id] = new NodeSource(fileUri, range);
                    }
                }

                nodes.AddRange(nodesBySymbol.Values);

                foreach (var (symbol, dependencies) in dependenciesBySymbol)
                {
                    if (!nodesBySymbol.TryGetValue(symbol, out var source))
                    {
                        continue;
                    }

                    foreach (var dependency in dependencies)
                    {
                        if (!nodesBySymbol.TryGetValue(dependency.Resource, out var target))
                        {
                            continue;
                        }

                        edges.Add(new GraphEdge(
                            Id: $"{source.Id}->{target.Id}",
                            SourceId: source.Id,
                            TargetId: target.Id));
                    }
                }
            }

            var graph = new CanonicalGraph(
                Nodes: nodes.OrderBy(node => node.Id, StringComparer.Ordinal).ToImmutableArray(),
                Edges: edges.OrderBy(edge => edge.Id, StringComparer.Ordinal).ToImmutableArray(),
                ErrorCount: entrySemanticModel.GetAllDiagnostics().Count(x => x.IsError()));

            return (graph, sources);
        }
    }
}
