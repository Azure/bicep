// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class FileExtensions
    {
        public static TempFile CreateTempFile(this IFile file) => new(file.FileSystem);
    }
}
