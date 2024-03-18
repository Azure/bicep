// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Syntax
{
    public class SyntaxHierarchy : ISyntaxHierarchy
    {
        private readonly BicepSourceFile? sourceFile;
        private readonly IReadOnlyDictionary<SyntaxBase, SyntaxBase?> parentMap;

        private SyntaxHierarchy(BicepSourceFile? sourceFile, IReadOnlyDictionary<SyntaxBase, SyntaxBase?> parentMap)
        {
            this.sourceFile = sourceFile;
            this.parentMap = parentMap;
        }

        public static ISyntaxHierarchy Build(SyntaxBase root, BicepSourceFile? sourceFile = null)
        {
            var parentMap = new Dictionary<SyntaxBase, SyntaxBase?>();
            var visitor = new ParentTrackingVisitor(parentMap);
            visitor.Visit(root);

            return new SyntaxHierarchy(sourceFile, parentMap);
        }

        /// <summary>
        /// Gets the parent of the specified node. Returns null for root nodes. Throws an exception for nodes that have not been indexed.
        /// </summary>
        /// <param name="node">The node</param>
        public SyntaxBase? GetParent(SyntaxBase node)
        {
            if (this.parentMap.TryGetValue(node, out var parent) == false)
            {
                var additionalInfo = "";
                if (sourceFile is {})
                {
                    var position = TextCoordinateConverter.GetPosition(sourceFile.LineStarts, node.Span.Position);
                    additionalInfo = $" Source file: {sourceFile.FileUri}, line: {position.line}, char: {position.character}. Node contents: {node.ToString()}.";
                }

                throw new ArgumentException($"Unable to determine parent of specified node of type '{node.GetType().Name}' at span '{node.Span}' because it has not been indexed.{additionalInfo}");
            }

            return parent;
        }

        public bool IsDescendant(SyntaxBase node, SyntaxBase potentialAncestor)
        {
            var current = node;
            while (current != null)
            {
                current = this.GetParent(current);
                if (ReferenceEquals(current, potentialAncestor))
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class ParentTrackingVisitor : CstVisitor
        {
            private readonly Dictionary<SyntaxBase, SyntaxBase?> parentMap;
            private readonly Stack<SyntaxBase> currentParents = new();

            public ParentTrackingVisitor(Dictionary<SyntaxBase, SyntaxBase?> parentMap)
            {
                this.parentMap = parentMap;
            }

            protected override void VisitInternal(SyntaxBase node)
            {
                var parent = currentParents.Count <= 0 ? null : currentParents.Peek();
                parentMap.Add(node, parent);

                currentParents.Push(node);
                base.VisitInternal(node);
                currentParents.Pop();
            }
        }
    }
}
