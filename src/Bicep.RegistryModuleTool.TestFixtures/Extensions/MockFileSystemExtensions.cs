// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.TestFixtures.Extensions
{
    public static class MockFileSystemExtensions
    {
        public static void SetTempFile(this MockFileSystem fileSystem, MockFileData data)
        {
            var tempFilePath = fileSystem.Directory.GetFiles(fileSystem.Path.GetTempPath()).Single();
            fileSystem.AddFile(tempFilePath, data);
        }
    }
}
