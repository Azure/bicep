// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            public ResourceAncestor(ResourceAncestorType ancestorType, ResourceSymbol resource, SyntaxBase? indexExpression)
            {
                AncestorType = ancestorType;
                Resource = resource;
                IndexExpression = indexExpression;
            }

            public ResourceAncestorType AncestorType { get; }

            public ResourceSymbol Resource { get; }

            public SyntaxBase? IndexExpression { get; }
        }

        private readonly ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceAncestor>> data;

        public ResourceAncestorGraph(ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceAncestor>> data)
        {
            this.data = data;
        }

        // Gets the ordered list of ancestors of this resource in order from 'oldest' to 'youngest'
        // this is the same order we need to compute the name of a resource using `/` separated segments in a string.
        public ImmutableArray<ResourceAncestor> GetAncestors(ResourceSymbol resource)
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

        private static IEnumerable<ResourceAncestor> GetAncestorsYoungestToOldest(ImmutableDictionary<ResourceSymbol, ResourceAncestor> hierarchy, ResourceSymbol resource)
        {
            var visited = new HashSet<ResourceSymbol>();
            while (hierarchy.TryGetValue(resource, out var ancestor) && !visited.Contains(ancestor.Resource))
            {
                visited.Add(ancestor.Resource);
                yield return ancestor;

                resource = ancestor.Resource;
            }
        }

        public static ResourceAncestorGraph Compute(BicepFile bicepFile, IBinder binder)
        {
            var visitor = new ResourceAncestorVisitor(binder);
            visitor.Visit(bicepFile.ProgramSyntax);

            var ancestry = visitor.Ancestry.Keys
                .ToImmutableDictionary(
                    child => child,
                    child => GetAncestorsYoungestToOldest(visitor.Ancestry, child).Reverse().ToImmutableArray());
            
            return new ResourceAncestorGraph(ancestry);
        }
    }
}
