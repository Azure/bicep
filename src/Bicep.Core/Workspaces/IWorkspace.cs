// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Workspaces
{
    public interface IWorkspace : IReadOnlyWorkspace
    {
        (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void RemoveSourceFiles(IEnumerable<ISourceFile> sourceFiles);
    }
}
