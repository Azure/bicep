// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Workspaces
{
    public interface IReadOnlyWorkspace
    {
        bool TryGetSourceFile(Uri fileUri, [NotNullWhen(true)] out ISourceFile? sourceFile);

        IEnumerable<ISourceFile> GetSourceFilesForDirectory(Uri fileUri);

        ImmutableDictionary<Uri, ISourceFile> GetActiveSourceFilesByUri();
    }
}
