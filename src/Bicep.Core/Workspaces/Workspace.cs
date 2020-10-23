// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    /// <summary>
    /// Represents the active set of files and shared data that can be utilized to compile one or more bicep files.
    /// </summary>
    public class Workspace : IWorkspace
    {
        private readonly IDictionary<string, SyntaxTree> activeSyntaxTrees = new Dictionary<string, SyntaxTree>();

        public bool TryGetSyntaxTree(string normalizedFileName, [NotNullWhen(true)] out SyntaxTree? syntaxTree)
            => activeSyntaxTrees.TryGetValue(normalizedFileName, out syntaxTree);

        public IEnumerable<SyntaxTree> GetSyntaxTreesForDirectory(string normalizedFilePath)
            => activeSyntaxTrees
                .Where(kvp => kvp.Key.Length > normalizedFilePath.Length && kvp.Key.StartsWith(normalizedFilePath))
                .Select(kvp => kvp.Value);

        public (ImmutableArray<SyntaxTree> added, ImmutableArray<SyntaxTree> removed) UpsertSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees)
        {
            var addedTrees = new List<SyntaxTree>();
            var removedTrees = new List<SyntaxTree>();

            foreach (var newSyntaxTree in syntaxTrees)
            {
                if (activeSyntaxTrees.TryGetValue(newSyntaxTree.FilePath, out var oldSyntaxTree))
                {
                    if (oldSyntaxTree == newSyntaxTree)
                    {
                        continue;
                    }

                    removedTrees.Add(oldSyntaxTree);
                }

                addedTrees.Add(newSyntaxTree);

                activeSyntaxTrees[newSyntaxTree.FilePath] = newSyntaxTree;
            }

            return (addedTrees.ToImmutableArray(), removedTrees.ToImmutableArray());
        }

        public void RemoveSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees)
        {
            foreach (var syntaxTree in syntaxTrees)
            {
                if (activeSyntaxTrees.TryGetValue(syntaxTree.FilePath, out var treeToRemove) && treeToRemove == syntaxTree)
                {
                    activeSyntaxTrees.Remove(syntaxTree.FilePath);
                }
            }
        }
    }
}