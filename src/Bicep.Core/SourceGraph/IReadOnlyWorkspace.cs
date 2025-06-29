// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.SourceGraph
{
    public interface IReadOnlyWorkspace
    {
        bool TryGetSourceFile(Uri fileUri, [NotNullWhen(true)] out ISourceFile? sourceFile);

        IEnumerable<ISourceFile> GetSourceFilesForDirectory(Uri fileUri);

        ImmutableDictionary<Uri, ISourceFile> GetActiveSourceFilesByUri();
    }
}
