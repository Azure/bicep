// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Workspaces
{
    public interface ISourceFile
    {
        Uri Uri { get; }

        IFileHandle FileHandle { get; }

        string Text { get; }
    }
}
