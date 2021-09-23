// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    [Method("textDocument/deploymentGraph", Direction.ClientToServer)]
    public record BicepDeploymentGraphParams(TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<BicepDeploymentGraph?>;

    public record BicepDeploymentGraph(IEnumerable<BicepDeploymentGraphNode> Nodes, IEnumerable<BicepDeploymentGraphEdge> Edges, int ErrorCount);

    public record BicepDeploymentGraphNode(string Id, string Type, bool IsCollection, Range Range, bool HasChildren, bool HasError, string? FilePath);

    public record BicepDeploymentGraphEdge(string SourceId, string TargetId);

    public class BicepDeploymentGraphHandler : IJsonRpcRequestHandler<BicepDeploymentGraphParams, BicepDeploymentGraph?>
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;

        private readonly ICompilationManager compilationManager;

        public BicepDeploymentGraphHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<BicepDeploymentGraph?> Handle(BicepDeploymentGraphParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context == null)
            {
                this.logger.LogError("Dependency graph request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult<BicepDeploymentGraph?>(null);
            }

            var graph = CreateDeploymentGraph(context, Path.GetFullPath(request.TextDocument.Uri.GetFileSystemPath()));

            return Task.FromResult<BicepDeploymentGraph?>(graph);
        }

        public static BicepDeploymentGraph CreateDeploymentGraph(CompilationContext context, string entryFilePath)
        {
            var nodes = new List<BicepDeploymentGraphNode>();
            var edges = new List<BicepDeploymentGraphEdge>();

            var queue = new Queue<(SemanticModel, string, string?)>();
            var entrySemanticModel = context.Compilation.GetEntrypointSemanticModel();

            queue.Enqueue((entrySemanticModel, entryFilePath, null));

            while (queue.Count > 0)
            {
                var (semanticModel, filePath, parentId) = queue.Dequeue();
                var nodesBySymbol = new Dictionary<DeclaredSymbol, BicepDeploymentGraphNode>();
                var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(semanticModel)
                    .Where(x => x.Key.Name != LanguageConstants.MissingName && x.Key.Name != LanguageConstants.ErrorName)
                    .ToImmutableDictionary(x => x.Key, x => x.Value);

                var errors = semanticModel.GetAllDiagnostics().Where(x => x.Level == DiagnosticLevel.Error).ToList();

                // Create nodes.
                foreach (var symbol in dependenciesBySymbol.Keys)
                {
                    var id = parentId is null ? symbol.Name : $"{parentId}::{symbol.Name}";

                    if (symbol is ResourceSymbol resourceSymbol)
                    {
                        var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FullyQualifiedType ?? "<unknown>";
                        var isCollection = resourceSymbol.IsCollection;
                        var resourceSpan = resourceSymbol.DeclaringResource.Span;
                        var range = resourceSpan.ToRange(context.LineStarts);
                        var resourceHasError = errors.Any(error => TextSpan.AreOverlapping(resourceSpan, error.Span));

                        nodesBySymbol[symbol] = new BicepDeploymentGraphNode(id, resourceType, isCollection, range, false, resourceHasError, filePath);
                    }

                    if (symbol is ModuleSymbol moduleSymbol)
                    {
                        var directory = Path.GetDirectoryName(filePath);
                        var moduleRelativePath = moduleSymbol.DeclaringModule.TryGetPath()?.TryGetLiteralValue();
                        var moduleFilePath = directory is not null && moduleRelativePath is not null
                            ? Path.GetFullPath(Path.Combine(directory, moduleRelativePath))
                            : null;

                        var isCollection = moduleSymbol.IsCollection;
                        var moduleSpan = moduleSymbol.DeclaringModule.Span;
                        var range = moduleSpan.ToRange(context.LineStarts);
                        var moduleHasError = errors.Any(error => TextSpan.AreOverlapping(moduleSpan, error.Span));

                        var hasChildren = false;

                        if (moduleFilePath is not null &&
                            moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var _) &&
                            moduleSemanticModel is SemanticModel bicepModel &&
                            (bicepModel.Root.ResourceDeclarations.Any() || bicepModel.Root.ModuleDeclarations.Any()))
                        {
                            hasChildren = true;
                            queue.Enqueue((bicepModel, moduleFilePath, id));
                        }

                        nodesBySymbol[symbol] = new BicepDeploymentGraphNode(id, "<module>", isCollection, range, hasChildren, moduleHasError, moduleFilePath);
                    }
                }

                nodes.AddRange(nodesBySymbol.Values);

                // Create edges.
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

                        edges.Add(new BicepDeploymentGraphEdge(source.Id, target.Id));
                    }
                }
            }

            return new BicepDeploymentGraph(
                nodes.OrderBy(node => node.Id),
                edges.OrderBy(edge => $"{edge.SourceId}>{edge.TargetId}"),
                entrySemanticModel.GetAllDiagnostics().Count(x => x.Level == DiagnosticLevel.Error));
        }
    }
}
