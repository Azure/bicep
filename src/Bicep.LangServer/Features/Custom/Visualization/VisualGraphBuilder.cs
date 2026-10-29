// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using Bicep.LanguageServer.Features.Language.DocumentLink;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// The canonical graph and each node's source location, resolved on demand. The canonical graph itself carries no
    /// locations, so volatile range data does not change the graph on every edit.
    /// </summary>
    public record VisualGraphBuildResult(
        CanonicalGraph Graph,
        IReadOnlyDictionary<string, NodeSource> Sources);

    /// <summary>
    /// Builds the canonical <see cref="CanonicalGraph"/> from a Bicep compilation. Nodes carry a <c>kind</c>,
    /// a <c>parentId</c>, and a symbol name, and edges carry a stable id. No layout is computed here; positions
    /// are added later by the layout engine.
    /// <para>
    /// A module whose Bicep source is available expands into its declarations: a local module from the compilation,
    /// and a registry module from the source it was published with, read from the local module cache. Any other remote
    /// module is a leaf whose <see cref="GraphNode.SourceStatus"/> says why.
    /// </para>
    /// </summary>
    public sealed class VisualGraphBuilder(BicepCompiler compiler, IModuleDispatcher moduleDispatcher)
    {
        /// <summary>A Bicep file whose declarations become nodes under <paramref name="ParentId"/>.</summary>
        /// <param name="Archive">Where the file is in a registry module's published source; null for a file in the compilation.</param>
        /// <param name="EnclosingRegistryModules">The registry modules the file is inside, by fully qualified reference.</param>
        private sealed record PendingFile(
            SemanticModel Model,
            string? ParentId,
            ArchivedFile? Archive,
            ImmutableHashSet<string> EnclosingRegistryModules);

        private sealed record ArchivedFile(OciArtifactReference RegistryReference, SourceArchive Archive, string Path);

        /// <summary>
        /// What a module node expands into: the declarations of a Bicep file, as its children, or why it has none. Each case
        /// other than <see cref="Children"/> and <see cref="Leaf"/> is one <see cref="GraphNodeSourceStatus"/>.
        /// </summary>
        private abstract record ModuleExpansion
        {
            /// <summary>The module's file declares resources or modules, which become its children.</summary>
            public sealed record Children(PendingFile File) : ModuleExpansion;

            /// <summary>The module has no children and nothing to explain.</summary>
            public sealed record Leaf : ModuleExpansion;

            /// <summary>The module has no Bicep source to show. See <see cref="GraphNodeSourceStatus.Unavailable"/>.</summary>
            public sealed record Unavailable : ModuleExpansion;

            /// <summary>A registry module not in the local module cache yet. See <see cref="GraphNodeSourceStatus.Unrestored"/>.</summary>
            public sealed record Unrestored(OciArtifactReference RegistryReference) : ModuleExpansion;

            /// <summary>A registry module whose source could not be loaded. See <see cref="GraphNodeSourceStatus.Error"/>.</summary>
            public sealed record Failed(OciArtifactReference RegistryReference, string? Error) : ModuleExpansion;
        }

        /// <summary>
        /// Published source files compiled on their own, by text. A compiled file depends only on its text, so an entry
        /// never goes stale, and a module restored again with different source simply uses new entries.
        /// </summary>
        private readonly ConcurrentDictionary<string, SemanticModel> archivedFileModels = new(StringComparer.Ordinal);

        public VisualGraphBuildResult Build(CompilationContext context)
        {
            var nodes = new List<GraphNode>();
            var edges = new List<GraphEdge>();
            var sources = new Dictionary<string, NodeSource>(StringComparer.Ordinal);

            var entryModel = context.Compilation.GetEntrypointSemanticModel();
            var queue = new Queue<PendingFile>();
            queue.Enqueue(new(entryModel, ParentId: null, Archive: null, EnclosingRegistryModules: []));

            while (queue.TryDequeue(out var file))
            {
                var semanticModel = file.Model;
                var parentId = file.ParentId;
                var documentUri = GetDocumentUri(file);
                var nodesBySymbol = new Dictionary<DeclaredSymbol, GraphNode>();
                var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(semanticModel)
                    .Where(x => x.Key.Name != LanguageConstants.MissingName && x.Key.Name != LanguageConstants.ErrorName)
                    .ToImmutableDictionary(x => x.Key, x => x.Value);

                // Published source is not the user's to fix, and compiled on its own it reports errors (such as module
                // paths that do not resolve) that the module does not have.
                var errors = file.Archive is null
                    ? semanticModel.GetAllDiagnostics().Where(x => x.IsError()).ToList()
                    : [];

                foreach (var symbol in dependenciesBySymbol.Keys)
                {
                    var id = parentId is null ? symbol.Name : $"{parentId}::{symbol.Name}";

                    if (symbol is ResourceSymbol resourceSymbol)
                    {
                        var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FormatType() ?? "<unknown>";
                        var resourceSpan = resourceSymbol.DeclaringResource.Span;
                        var range = resourceSpan.ToRange(semanticModel.SourceFile.LineStarts);
                        var hasError = errors.Any(error => ContainsDiagnostic(resourceSpan, error.Span));

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Resource,
                            ParentId: parentId,
                            Type: resourceType,
                            SymbolName: symbol.Name,
                            IsCollection: resourceSymbol.IsCollection,
                            HasChildren: false,
                            HasError: hasError);
                        sources[id] = new NodeSource(documentUri, range);
                    }

                    if (symbol is ModuleSymbol moduleSymbol)
                    {
                        var moduleSpan = moduleSymbol.DeclaringModule.Span;
                        var range = moduleSpan.ToRange(semanticModel.SourceFile.LineStarts);
                        var hasError = errors.Any(error => ContainsDiagnostic(moduleSpan, error.Span));
                        var expansion = this.GetModuleExpansion(moduleSymbol, id, file, entryModel.SourceFile);
                        var (sourceStatus, sourceError) = expansion switch
                        {
                            ModuleExpansion.Unavailable => (GraphNodeSourceStatus.Unavailable, null),
                            ModuleExpansion.Unrestored => (GraphNodeSourceStatus.Unrestored, null),
                            ModuleExpansion.Failed failed => (GraphNodeSourceStatus.Error, failed.Error),
                            _ => ((string?)null, (string?)null),
                        };

                        if (expansion is ModuleExpansion.Children children)
                        {
                            queue.Enqueue(children.File);
                        }

                        nodesBySymbol[symbol] = new GraphNode(
                            Id: id,
                            Kind: GraphNodeKind.Module,
                            ParentId: parentId,
                            Type: "<module>",
                            SymbolName: symbol.Name,
                            // A module whose path does not resolve has no type to say it is a loop, as in published source.
                            IsCollection: moduleSymbol.IsCollection || moduleSymbol.DeclaringModule.Value is ForSyntax,
                            HasChildren: expansion is ModuleExpansion.Children,
                            HasError: hasError,
                            SourceStatus: sourceStatus,
                            SourceError: sourceError);
                        sources[id] = new NodeSource(documentUri, range);
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

            return new(graph, sources);
        }

        /// <summary>
        /// The registry reference behind the node <paramref name="nodeId"/> when its <see cref="GraphNode.SourceStatus"/>
        /// is <see cref="GraphNodeSourceStatus.Unrestored"/> or <see cref="GraphNodeSourceStatus.Error"/>, which expanding
        /// it restores; otherwise null. A node id is the path of symbol names to the node, so this follows that path
        /// alone, expanding each module on it as <see cref="Build"/> does, rather than building the whole graph.
        /// </summary>
        public OciArtifactReference? TryGetRestorableReference(CompilationContext context, string nodeId)
        {
            var entryModel = context.Compilation.GetEntrypointSemanticModel();
            var file = new PendingFile(entryModel, ParentId: null, Archive: null, EnclosingRegistryModules: []);
            var names = nodeId.Split("::");

            for (var index = 0; index < names.Length; index++)
            {
                var module = file.Model.Root.ModuleDeclarations.FirstOrDefault(module => module.Name == names[index]);
                if (module is null)
                {
                    return null;
                }

                var id = string.Join("::", names, 0, index + 1);
                var expansion = this.GetModuleExpansion(module, id, file, entryModel.SourceFile);

                if (index == names.Length - 1)
                {
                    return expansion switch
                    {
                        ModuleExpansion.Unrestored unrestored => unrestored.RegistryReference,
                        ModuleExpansion.Failed failed => failed.RegistryReference,
                        _ => null,
                    };
                }

                if (expansion is not ModuleExpansion.Children children)
                {
                    return null;
                }

                file = children.File;
            }

            return null;
        }

        private ModuleExpansion GetModuleExpansion(
            ModuleSymbol module,
            string nodeId,
            PendingFile file,
            BicepSourceFile entryFile)
        {
            var reference = file.Model.SourceFileGrouping.ArtifactLookup.TryGetValue(module.DeclaringModule, out var artifact)
                ? artifact.Reference
                : null;

            if (file.Archive is not { } archive)
            {
                return reference switch
                {
                    OciArtifactReference registryReference => this.GetRegistryModuleExpansion(registryReference, nodeId, file),
                    TemplateSpecModuleReference => new ModuleExpansion.Unavailable(),
                    _ when module.TryGetSemanticModel().IsSuccess(out var model, out _) && model is SemanticModel bicepModel =>
                        ExpandInto(new(bicepModel, nodeId, Archive: null, file.EnclosingRegistryModules)),
                    _ => new ModuleExpansion.Leaf(),
                };
            }

            // Published source is compiled on its own, so its module paths do not resolve. The archive links each one to
            // the file it resolved to when the module was published, as the editor does for the published source.
            var target = TryGetLinkTarget(module.DeclaringModule, file.Model, archive);

            if (target?.Metadata.Kind == ArchivedSourceFileKind.Bicep)
            {
                var path = target.Metadata.Path;

                return ExpandInto(new(this.CompileArchivedFile(target.Contents), nodeId, archive with { Path = path }, file.EnclosingRegistryModules));
            }

            // A registry module published with its source is named by its link, which also covers aliases only the
            // publisher had configured. It is restored and cached with the user's configuration.
            var address = target?.Metadata.ArtifactAddress ?? (reference as OciArtifactReference)?.AddressComponents;

            if (address is not null)
            {
                var registryReference = new OciArtifactReference(entryFile.LoadFeatures(), entryFile.LoadConfiguration(), ArtifactType.Module, address);

                return this.GetRegistryModuleExpansion(registryReference, nodeId, file);
            }

            return reference is TemplateSpecModuleReference ? new ModuleExpansion.Unavailable() : new ModuleExpansion.Leaf();
        }

        private ModuleExpansion GetRegistryModuleExpansion(OciArtifactReference registryReference, string nodeId, PendingFile file)
        {
            var key = registryReference.FullyQualifiedReference;

            // Tags can be published again, so a registry module can end up inside itself. It is shown once.
            if (file.EnclosingRegistryModules.Contains(key))
            {
                return new ModuleExpansion.Leaf();
            }

            switch (moduleDispatcher.GetArtifactRestoreStatus(registryReference, out var failureBuilder))
            {
                case ArtifactRestoreStatus.Unknown:
                    return new ModuleExpansion.Unrestored(registryReference);

                case ArtifactRestoreStatus.Failed:
                    return new ModuleExpansion.Failed(registryReference, failureBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart()).Message);
            }

            if (!registryReference.TryLoadSourceArchive().IsSuccess(out var archive, out var exception))
            {
                // An archive that cannot be read, for example from an incompatible Bicep version, may restore again.
                return exception is SourceNotAvailableException
                    ? new ModuleExpansion.Unavailable()
                    : new ModuleExpansion.Failed(registryReference, exception.Message);
            }

            var entryPoint = archive.FindSourceFile(archive.EntrypointRelativePath);

            return ExpandInto(new(
                this.CompileArchivedFile(entryPoint.Contents),
                nodeId,
                new ArchivedFile(registryReference, archive, entryPoint.Metadata.Path),
                file.EnclosingRegistryModules.Add(key)));
        }

        /// <summary>Compiles a published source file on its own, as the definition handler does for imported symbols.</summary>
        private SemanticModel CompileArchivedFile(string text) =>
            this.archivedFileModels.GetOrAdd(text, text =>
            {
                var bicepFile = compiler.SourceFileFactory.CreateBicepFile(DummyFileHandle.Default, text);
                var workspace = new ActiveSourceFileSet();
                workspace.UpsertSourceFile(bicepFile);

                return compiler.CreateCompilationWithoutRestore(bicepFile.FileHandle.Uri, workspace).GetEntrypointSemanticModel();
            });

        private static ArchivedSourceFile? TryGetLinkTarget(ModuleDeclarationSyntax module, SemanticModel model, ArchivedFile file)
        {
            var start = new TextPosition(TextCoordinateConverter.GetPosition(model.SourceFile.LineStarts, module.Path.Span.Position));
            var link = file.Archive.FindDocumentLinks(file.Path).FirstOrDefault(link => link.Range.Start == start);

            return link is null ? null : file.Archive.FindSourceFile(link.Target);
        }

        /// <summary>A module expands into its file's resources and modules; one whose file declares none is a leaf.</summary>
        private static ModuleExpansion ExpandInto(PendingFile file) =>
            file.Model.Root.ResourceDeclarations.Any() || file.Model.Root.ModuleDeclarations.Any()
                ? new ModuleExpansion.Children(file)
                : new ModuleExpansion.Leaf();

        /// <summary>The document that reveals a file: the file itself, or a <c>bicep-extsrc:</c> view of published source.</summary>
        private static string GetDocumentUri(PendingFile file) => file.Archive is { } archived
            ? new ExternalSourceReference(archived.RegistryReference.AddressComponents, archived.Archive).WithRequestForSourceFile(archived.Path).ToUri().ToString()
            : file.Model.SourceFile.FileHandle.Uri.ToDocumentUri().ToString();

        private static bool ContainsDiagnostic(TextSpan declarationSpan, TextSpan diagnosticSpan) =>
            TextSpan.AreOverlapping(declarationSpan, diagnosticSpan) ||
            (diagnosticSpan.Length == 0 &&
                diagnosticSpan.Position >= declarationSpan.Position &&
                diagnosticSpan.Position <= declarationSpan.Position + declarationSpan.Length);
    }
}
