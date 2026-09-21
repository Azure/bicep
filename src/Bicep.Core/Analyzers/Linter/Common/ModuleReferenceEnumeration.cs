// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Analyzers.Linter.Common
{
    /// <summary>
    /// Helpers used by linter rules to enumerate local Bicep files reachable via <c>module</c>
    /// declarations, reusing the already-resolved files/models in <see cref="SemanticModel.SourceFileGrouping"/>.
    /// </summary>
    public static class ModuleReferenceEnumeration
    {
        /// <summary>
        /// The models of this file's own local <c>module</c> declarations. Registry
        /// modules and broken/unresolvable references are excluded.
        /// </summary>
        public static IEnumerable<SemanticModel> EnumerateLocalModuleModels(this SemanticModel model)
        {
            var grouping = model.SourceFileGrouping;

            foreach (var (syntax, reference) in grouping.ArtifactLookup)
            {
                if (reference.ReferencingFile != model.SourceFile ||
                    syntax is not ModuleDeclarationSyntax ||
                    reference.Reference is not { IsExternal: false })
                {
                    continue;
                }

                if (grouping.TryGetSourceFile(syntax).IsSuccess(out var file) &&
                    file is BicepFile &&
                    model.ModelLookup.GetSemanticModel(file) is SemanticModel child)
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Every local Bicep file reachable from this file, transitively, via local <c>module</c>
        /// declarations.
        /// </summary>
        public static IEnumerable<SemanticModel> EnumerateAllLocalModuleModelsTransitively(this SemanticModel entryPoint)
        {
            var visited = new HashSet<IOUri> { entryPoint.SourceFile.FileHandle.Uri };
            var toVisit = new Queue<SemanticModel>();
            toVisit.Enqueue(entryPoint);

            while (toVisit.TryDequeue(out var current))
            {
                foreach (var child in current.EnumerateLocalModuleModels())
                {
                    if (visited.Add(child.SourceFile.FileHandle.Uri))
                    {
                        yield return child;
                        toVisit.Enqueue(child);
                    }
                }
            }
        }
    }
}
