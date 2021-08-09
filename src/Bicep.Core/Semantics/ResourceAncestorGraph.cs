// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public sealed class ResourceAncestorGraph
    {
        public enum ResourceAncestorType
        {
            Nested,

            ParentProperty,
        }

        public class ResourceAncestor
        {
            public ResourceAncestor(ResourceAncestorType ancestorType, ResourceMetadata resource, SyntaxBase? indexExpression)
            {
                AncestorType = ancestorType;
                Resource = resource;
                IndexExpression = indexExpression;
            }

            public ResourceAncestorType AncestorType { get; }

            public ResourceMetadata Resource { get; }

            public SyntaxBase? IndexExpression { get; }
        }

        private readonly ImmutableDictionary<ResourceMetadata, ImmutableArray<ResourceAncestor>> data;

        public ResourceAncestorGraph(ImmutableDictionary<ResourceMetadata, ImmutableArray<ResourceAncestor>> data)
        {
            this.data = data;
        }

        // Gets the ordered list of ancestors of this resource in order from 'oldest' to 'youngest'
        // this is the same order we need to compute the name of a resource using `/` separated segments in a string.
        public ImmutableArray<ResourceAncestor> GetAncestors(ResourceMetadata resource)
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

        private static IEnumerable<ResourceAncestor> GetAncestorsYoungestToOldest(ImmutableDictionary<ResourceMetadata, ResourceAncestor> hierarchy, ResourceMetadata resource)
        {
            var visited = new HashSet<ResourceMetadata>();
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
