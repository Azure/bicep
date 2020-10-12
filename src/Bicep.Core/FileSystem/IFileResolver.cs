// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem
{
    public interface IFileResolver
    {
        string? TryRead(string fileName, out string? failureMessage);

        string GetNormalizedFileName(string fileName);

        string? TryResolveModulePath(string childFileName, string parentFileName);
    }
}