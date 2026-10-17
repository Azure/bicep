// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// The canonical graph with what is resolved from it on demand: each node's source location, and the registry
    /// module behind each remote module node, which restoring that node acts on.
    /// </summary>
    public record VisualGraphBuildResult(
        CanonicalGraph Graph,
        IReadOnlyDictionary<string, NodeSource> Sources,
        IReadOnlyDictionary<string, OciArtifactReference> ModuleReferences);

    /// <summary>
    /// Builds the canonical <see cref="CanonicalGraph"/> from a Bicep compilation. Nodes carry a <c>kind</c>,
    /// a <c>parentId</c>, and a symbol name, and edges carry a stable id. No layout is computed here; positions
    /// are added later by the layout engine.
    /// <para>
    /// Local modules expand into their declarations. Registry modules expand the same way when their published
    /// Bicep source is in the local module cache, given an <see cref="IVisualModuleSourceProvider"/>; otherwise
    /// the module is a leaf, marked <see cref="GraphNode.RequiresRestore"/> when restoring it may supply its source.
    /// </para>
    /// </summary>
    public static class VisualGraphBuilder
    {
        /// <summary>Where a file being visited comes from: the compilation, or a module's published source.</summary>
        private abstract record FileOrigin;

        private sealed record CompiledFile(IOUri FileUri) : FileOrigin;

        private sealed record ArchivedFile(ArchivedModuleSource Module, ArchivedBicepFile File) : FileOrigin;

        /// <summary>A Bicep file to visit, whose declarations become nodes under <paramref name="ParentId"/>.</summary>
        /// <param name="RegistryModuleChain">The registry modules enclosing this file, to stop a module from containing itself.</param>
        private sealed record PendingFile(
            SemanticModel Model,
            FileOrigin Origin,
            ImmutableHashSet<string> RegistryModuleChain,
            string? ParentId = null);

        /// <summary>
        /// What a module node resolves to: the file declaring its contents, or null when it shows none.
        /// <see cref="RequiresRestore"/> says whether restoring the module may supply them, and <see cref="Reference"/> is the
        /// registry module behind the node, which restoring acts on.
        /// </summary>
        private sealed record ResolvedModule(
            PendingFile? File,
            bool RequiresRestore = false,
            OciArtifactReference? Reference = null);

        private static readonly ResolvedModule Leaf = new(File: null);

        public static CanonicalGraph Build(CompilationContext context, IOUri entryFileUri, IVisualModuleSourceProvider? moduleSources = null) =>
            BuildWithSources(context, entryFileUri, moduleSources).Graph;

        /// <summary>
        /// Builds the canonical graph together with what is resolved on demand. The canonical graph itself carries
        /// no source location so that volatile range/file-path data does not change the graph on every edit.
        /// </summary>
        public static VisualGraphBuildResult BuildWithSources(
            CompilationContext context,
            IOUri entryFileUri,
            IVisualModuleSourceProvider? moduleSources = null)
        {
            var nodes = new List<GraphNode>();
            var edges = new List<GraphEdge>();
            var sources = new Dictionary<string, NodeSource>(StringComparer.Ordinal);
            var moduleReferences = new Dictionary<string, OciArtifactReference>(StringComparer.Ordinal);

            var queue = new Queue<PendingFile>();
            var entrySemanticModel = context.Compilation.GetEntrypointSemanticModel();

            queue.Enqueue(new(entrySemanticModel, new CompiledFile(entryFileUri), []));

            while (queue.Count > 0)
            {
                var (semanticModel, origin, registryModuleChain, parentId) = queue.Dequeue();
                var nodesBySymbol = new Dictionary<DeclaredSymbol, GraphNode>();
                var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(semanticModel)
                    .Where(x => x.Key.Name != LanguageConstants.MissingName && x.Key.Name != LanguageConstants.ErrorName)
                    .ToImmutableDictionary(x => x.Key, x => x.Value);

                // A module's published source is not the user's to fix, and compiled alone it reports errors (for
                // example, for the files beside it) that the module does not have.
                var errors = origin is CompiledFile
                    ? semanticModel.GetAllDiagnostics().Where(x => x.IsError()).ToList()
                    : [];

                foreach (var symbol in dependenciesBySymbol.Keys)
                {
                    var id = parentId is null ? symbol.Name : $"{parentId}::{symbol.Name}";

                    if (symbol is ResourceSymbol resourceSymbol)
                    {
                        var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FormatType() ?? "<unknown>";
                        var resourceSpan = resourceSymbol.DeclaringResource.Span;
                        var hasError = errors.Any(error => ContainsDiagnostic(resourceSpan, error.Span));

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Resource,
                            ParentId: parentId,
                            Type: resourceType,
                            SymbolName: symbol.Name,
                            IsCollection: resourceSymbol.IsCollection,
                            HasChildren: false,
                            RequiresRestore: false,
                            HasError: hasError);
                        sources[id] = GetNodeSource(origin, semanticModel, resourceSpan);
                    }

                    if (symbol is ModuleSymbol moduleSymbol)
                    {
                        var moduleSpan = moduleSymbol.DeclaringModule.Span;
                        var hasError = errors.Any(error => ContainsDiagnostic(moduleSpan, error.Span));
                        var module = origin switch
                        {
                            CompiledFile => ResolveCompiledModule(moduleSymbol, semanticModel, registryModuleChain, moduleSources),
                            ArchivedFile archivedFile => ResolveArchivedModule(moduleSymbol, semanticModel, archivedFile, entrySemanticModel.SourceFile, registryModuleChain, moduleSources),
                            _ => Leaf,
                        };

                        if (module.File is { } moduleFile)
                        {
                            queue.Enqueue(moduleFile with { ParentId = id });
                        }

                        if (module.Reference is not null)
                        {
                            moduleReferences[id] = module.Reference;
                        }

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Module,
                            ParentId: parentId,
                            Type: "<module>",
                            SymbolName: symbol.Name,
                            // A module compiled alone cannot resolve its target's type, so the loop is read from syntax.
                            IsCollection: moduleSymbol.IsCollection || moduleSymbol.DeclaringModule.Value is ForSyntax,
                            HasChildren: module.File is not null,
                            RequiresRestore: module.RequiresRestore,
                            HasError: hasError);
                        sources[id] = GetNodeSource(origin, semanticModel, moduleSpan);
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
                Edges: edges.OrderBy(edge => edge.Id, StringComparer.Ordinal).ToImmutableArray());

            return new(graph, sources, moduleReferences);
        }

        private static ResolvedModule ResolveCompiledModule(
            ModuleSymbol moduleSymbol,
            SemanticModel semanticModel,
            ImmutableHashSet<string> registryModuleChain,
            IVisualModuleSourceProvider? moduleSources)
        {
            var reference = semanticModel.SourceFileGrouping.ArtifactLookup.TryGetValue(moduleSymbol.DeclaringModule, out var artifact)
                ? artifact.Reference
                : null;

            if (reference is OciArtifactReference ociReference)
            {
                return ResolveRegistryModule(ociReference, registryModuleChain, moduleSources);
            }

            // A template spec holds only compiled JSON, so it is a leaf.
            if (reference is not TemplateSpecModuleReference &&
                moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel, out var _) &&
                moduleSemanticModel is SemanticModel bicepModel &&
                HasDeclarations(bicepModel))
            {
                return new(new PendingFile(bicepModel, new CompiledFile(bicepModel.SourceFile.FileHandle.Uri), registryModuleChain));
            }

            return Leaf;
        }

        private static ResolvedModule ResolveArchivedModule(
            ModuleSymbol moduleSymbol,
            SemanticModel semanticModel,
            ArchivedFile origin,
            BicepSourceFile entryFile,
            ImmutableHashSet<string> registryModuleChain,
            IVisualModuleSourceProvider? moduleSources)
        {
            // An archived file is compiled without the user's bicepconfig.json, so registry modules it references are
            // re-created with the entry file's: restoring them uses its cloud, credentials, and module cache.
            OciArtifactReference CreateReference(IOciArtifactAddressComponents address) =>
                new(entryFile.LoadFeatures(), entryFile.LoadConfiguration(), ArtifactType.Module, address);

            // The archive links each module path to the archived file it resolved to when the module was published.
            if (TryGetLinkTarget(moduleSymbol.DeclaringModule, semanticModel, origin) is { } target)
            {
                if (target.Metadata.ArtifactAddress is { } artifactAddress)
                {
                    return ResolveRegistryModule(CreateReference(artifactAddress), registryModuleChain, moduleSources);
                }

                if (target.Metadata.Kind == ArchivedSourceFileKind.Bicep)
                {
                    return origin.Module.GetBicepFile(target.Metadata.Path) is { } file && HasDeclarations(file.Model)
                        ? new(new PendingFile(file.Model, origin with { File = file }, registryModuleChain))
                        : Leaf;
                }
            }

            // No usable link: a registry module published without source, or a template spec (a leaf).
            return semanticModel.SourceFileGrouping.ArtifactLookup.TryGetValue(moduleSymbol.DeclaringModule, out var artifact) &&
                artifact.Reference is OciArtifactReference ociReference
                ? ResolveRegistryModule(CreateReference(ociReference), registryModuleChain, moduleSources)
                : Leaf;
        }

        private static ResolvedModule ResolveRegistryModule(
            OciArtifactReference reference,
            ImmutableHashSet<string> registryModuleChain,
            IVisualModuleSourceProvider? moduleSources)
        {
            var key = reference.FullyQualifiedReference;

            if (moduleSources is null || registryModuleChain.Contains(key))
            {
                return Leaf;
            }

            var result = moduleSources.GetModuleSource(reference);

            if (result.Source is not { } source)
            {
                return new(File: null, result.RequiresRestore, reference);
            }

            if (source.EntryPoint is not { } entryPoint || !HasDeclarations(entryPoint.Model))
            {
                return new(File: null, Reference: reference);
            }

            return new(new PendingFile(entryPoint.Model, new ArchivedFile(source, entryPoint), registryModuleChain.Add(key)), Reference: reference);
        }

        private static ArchivedSourceFile? TryGetLinkTarget(ModuleDeclarationSyntax module, SemanticModel semanticModel, ArchivedFile origin)
        {
            var start = new TextPosition(TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, module.Path.Span.Position));
            var link = origin.File.Links.FirstOrDefault(link => link.Range.Start == start);

            return link is null ? null : origin.Module.Archive.TryFindSourceFile(link.Target);
        }

        private static NodeSource GetNodeSource(FileOrigin origin, SemanticModel semanticModel, TextSpan span)
        {
            var range = span.ToRange(semanticModel.SourceFile.LineStarts);

            return origin switch
            {
                CompiledFile compiledFile => new(compiledFile.FileUri.ToDocumentUri().ToString(), range),
                ArchivedFile archivedFile => new(archivedFile.Module.GetDocumentUri(archivedFile.File.Path), range),
                _ => throw new InvalidOperationException($"Unexpected file origin {origin.GetType().Name}."),
            };
        }

        private static bool HasDeclarations(SemanticModel model) =>
            model.Root.ResourceDeclarations.Any() || model.Root.ModuleDeclarations.Any();

        private static bool ContainsDiagnostic(TextSpan declarationSpan, TextSpan diagnosticSpan) =>
            TextSpan.AreOverlapping(declarationSpan, diagnosticSpan) ||
            (diagnosticSpan.Length == 0 &&
                diagnosticSpan.Position >= declarationSpan.Position &&
                diagnosticSpan.Position <= declarationSpan.Position + declarationSpan.Length);
    }
}
