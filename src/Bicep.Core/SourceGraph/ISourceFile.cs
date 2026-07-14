// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFile
    {
        IFileHandle FileHandle { get; }

        string Text { get; }
    }
}
