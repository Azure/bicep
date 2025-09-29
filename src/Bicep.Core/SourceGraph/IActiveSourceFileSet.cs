// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.SourceGraph
{
    public interface IActiveSourceFileSet : IActiveSourceFileLookup
    {
        (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void RemoveSourceFiles(IEnumerable<ISourceFile> sourceFiles);
    }
}
