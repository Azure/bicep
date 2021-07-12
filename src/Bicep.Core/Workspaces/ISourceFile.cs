// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Workspaces
{
    public interface ISourceFile
    {
        Uri FileUri { get; }
    }
}
