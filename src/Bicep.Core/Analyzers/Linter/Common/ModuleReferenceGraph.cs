// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SemanticVersioning;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Analyzers.Linter.Common
{
    /// <summary>
    /// A single local Bicep file reachable from an entrypoint file via a chain of local <c>module</c>
    /// declarations.
    /// </summary>
    /// <param name="Model">The file's own semantic model.</param>
    /// <param name="Constraint">The file's own effective "bicep.version" constraint, or null if unconfigured.</param>
    /// <param name="Parent">The URI of the file that first referenced this one, or null for the entrypoint.</param>
    public sealed record ModuleReferenceGraphNode(SemanticModel Model, VersionRange? Constraint, IOUri? Parent);

    /// <summary>
    /// Every local Bicep file reachable from an entrypoint, transitively, via local <c>module</c> declarations.
    /// </summary>
    public sealed class ModuleReferenceGraph
    {
        private ModuleReferenceGraph(ImmutableDictionary<IOUri, ModuleReferenceGraphNode> nodes)
        {
            Nodes = nodes;
        }

        /// <summary>
        /// All reachable nodes, keyed by the file's own URI, including the entrypoint itself.
        /// </summary>
        public ImmutableDictionary<IOUri, ModuleReferenceGraphNode> Nodes { get; }

        /// <summary>
        /// Builds the graph reachable from <paramref name="entryPoint"/>. Each file is visited at most once.
        /// </summary>
        public static ModuleReferenceGraph Build(SemanticModel entryPoint)
        {
            var nodes = ImmutableDictionary.CreateBuilder<IOUri, ModuleReferenceGraphNode>();
            var toVisit = new Queue<SemanticModel>();

            var entryPointUri = entryPoint.SourceFile.FileHandle.Uri;
            nodes[entryPointUri] = new(entryPoint, entryPoint.Configuration.Compiler.Version, Parent: null);
            toVisit.Enqueue(entryPoint);

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();
                var currentUri = current.SourceFile.FileHandle.Uri;

                foreach (var moduleDeclaration in current.Root.ModuleDeclarations)
                {
                    if (!moduleDeclaration.TryGetSemanticModel().IsSuccess(out var referencedModel))
                    {
                        // Broken/unresolvable module reference - already reported as an ErrorType diagnostic
                        // on this module statement by DeclaredTypeManager.GetDeclaredModuleType().
                        continue;
                    }

                    if (referencedModel is not SemanticModel referencedBicepModel)
                    {
                        continue;
                    }

                    var referencedUri = referencedBicepModel.SourceFile.FileHandle.Uri;
                    if (nodes.ContainsKey(referencedUri))
                    {
                        // Already visited (diamond dependency, or would-be cycle) - don't reprocess.
                        continue;
                    }

                    nodes[referencedUri] = new(referencedBicepModel, referencedBicepModel.Configuration.Compiler.Version, Parent: currentUri);
                    toVisit.Enqueue(referencedBicepModel);
                }
            }

            return new(nodes.ToImmutable());
        }
    }

    /// <summary>
    /// Entry point used by linter rules to obtain the <see cref="ModuleReferenceGraph"/> for the file
    /// being analyzed.
    /// </summary>
    public static class ModuleReferenceGraphExtensions
    {
        /// <summary>
        /// Builds the graph reachable from this model, treating it as the entrypoint.
        /// </summary>
        public static ModuleReferenceGraph GetModuleReferenceGraph(this SemanticModel model)
            => ModuleReferenceGraph.Build(model);
    }
}
