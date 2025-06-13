// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;

namespace Bicep.TextFixtures.Utils.IO
{
    public static class TestFileUri
    {
        private static readonly MockFileSystem MockFileSystem = new();

        public static IOUri FromInMemoryPath(string path) => IOUri.FromLocalFilePath(NormalizePath(path));

        public static IOUri FromMockFileSystemPath(string path) => IOUri.FromLocalFilePath(MockFileSystem.Path.GetFullPath(NormalizePath(path)));

        // Prepend "/path/to" to enable use of ".." in tests for convenience.
        private static string NormalizePath(string path) => "/path/to/" + path.TrimStart('/');
    }
}
