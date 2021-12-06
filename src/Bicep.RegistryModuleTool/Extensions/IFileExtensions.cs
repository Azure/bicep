// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Utils;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    internal static class IFileExtensions
    {
        public static TempFile CreateTempFile(this IFile file) => new(file.FileSystem);
    }
}
