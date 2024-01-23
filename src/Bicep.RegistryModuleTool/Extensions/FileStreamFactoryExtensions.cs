// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class FileStreamFactoryExtensions
    {
        public static Stream CreateDeleteOnCloseStream(this IFileStreamFactory factory, string filePath) => factory.New(
            filePath,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.ReadWrite,
            bufferSize: 4096,
            FileOptions.DeleteOnClose);
    }
}
