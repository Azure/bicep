// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public sealed class ResourceAncestorGraph
    {
        public enum ResourceAncestorType
        {
            Nested,

            ParentProperty,
        }

        public record ResourceAncestor(ResourceAncestorType AncestorType, DeclaredResourceMetadata Resource, SyntaxBase? IndexExpression);

        private readonly ImmutableDictionary<DeclaredResourceMetadata, ImmutableArray<ResourceAncestor>> data;

        public ResourceAncestorGraph(ImmutableDictionary<DeclaredResourceMetadata, ImmutableArray<ResourceAncestor>> data)
        {
            this.data = data;
        }

        // Gets the ordered list of ancestors of this resource in order from 'oldest' to 'youngest'
        // this is the same order we need to compute the name of a resource using `/` separated segments in a string.
        public ImmutableArray<ResourceAncestor> GetAncestors(DeclaredResourceMetadata resource)
        {
            if (data.TryGetValue(resource, out var results))
            {
                return results;
            }
            else
            {
                return ImmutableArray<ResourceAncestor>.Empty;
            }
        }

        private static IEnumerable<ResourceAncestor> GetAncestorsYoungestToOldest(ImmutableDictionary<DeclaredResourceMetadata, ResourceAncestor> hierarchy, DeclaredResourceMetadata resource)
        {
            var visited = new HashSet<DeclaredResourceMetadata>();
            while (hierarchy.TryGetValue(resource, out var ancestor) && !visited.Contains(ancestor.Resource))
            {
                visited.Add(ancestor.Resource);
                yield return ancestor;

                resource = ancestor.Resource;
            }
        }

        public static ResourceAncestorGraph Compute(SemanticModel semanticModel)
        {
            var visitor = new ResourceAncestorVisitor(semanticModel);
            visitor.Visit(semanticModel.SourceFile.ProgramSyntax);

            var ancestry = visitor.Ancestry.Keys
                .ToImmutableDictionary(
                    child => child,
                    child => GetAncestorsYoungestToOldest(visitor.Ancestry, child).Reverse().ToImmutableArray());

            return new ResourceAncestorGraph(ancestry);
        }
    }
}
