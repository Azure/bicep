// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Workspaces
{
    public interface ISourceFile
    {
        Uri Uri { get; }

        string Text { get; }
    }
}
