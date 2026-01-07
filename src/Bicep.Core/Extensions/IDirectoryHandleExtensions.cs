// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Extensions;

public static class IDirectoryHandleExtensions
{
    public static ResultWithDiagnosticBuilder<IFileHandle> TryGetRelativeFile(this IDirectoryHandle directoryHandle, RelativePath path)
    {
        try
        {
            var relativeDirectory = directoryHandle.GetDirectory(path);

            if (relativeDirectory.Exists())
            {
                var uri = path.AsSpan()[^1] == '/' ? relativeDirectory.Uri : relativeDirectory.Uri.ToString()[..^1];

                return new(x => x.FoundDirectoryInsteadOfFile(uri));
            }

            return new(directoryHandle.GetFile(path));
        }
        catch (IOException exception)
        {
            return new(x => x.ErrorOccurredReadingFile(exception.Message));
        }
    }
}
