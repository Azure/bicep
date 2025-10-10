// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFile
    {
        // TODO(file-io-abstraction): This is now only referenced in tests. Remove after migration is done.
        Uri Uri { get; }

        IFileHandle FileHandle { get; }

        string Text { get; }
    }
}
