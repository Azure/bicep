// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    internal static class IDirectoryExtensions
    {
        public static string GetCurrentDirectoryName(this IDirectory directory)
        {
            var currentDirectory = directory.GetCurrentDirectory();

            return directory.FileSystem.DirectoryInfo.FromDirectoryName(currentDirectory).Name;
        }
    }
}
