// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFile
    {
        // CONSIDER: Uri Uri => this.FileHandle.Uri.ToUri();
        Uri Uri { get; }

        IFileHandle FileHandle { get; }

        string Text { get; }
    }
}
