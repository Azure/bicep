// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class DirectoryExtensions
    {
        public static string GetCurrentDirectoryName(this IDirectory directory)
        {
            var currentDirectory = directory.GetCurrentDirectory();

            return directory.FileSystem.DirectoryInfo.FromDirectoryName(currentDirectory).Name;
        }
    }
}
