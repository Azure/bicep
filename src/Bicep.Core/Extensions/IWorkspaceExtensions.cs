// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Extensions
{
    public static class IWorkspaceExtensions
    {
        public static (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFile(this IActiveSourceFileSet workspace, ISourceFile sourceFile) =>
            workspace.UpsertSourceFiles(sourceFile.AsEnumerable());
    }
}
