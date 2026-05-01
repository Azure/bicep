// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;

namespace Bicep.TextFixtures.IO
{
    public static class TestFileUri
    {
        private static readonly MockFileSystem MockFileSystem = new();

        public static IOUri FromInMemoryPath(string path) => IOUri.FromFilePath(NormalizePath(path));

        public static IOUri FromMockFileSystemPath(string path) => IOUri.FromFilePath(MockFileSystem.Path.GetFullPath(NormalizePath(path)));

        // Prepend "/path/to" to enable use of ".." in tests for convenience.
        private static string NormalizePath(string path) => "/path/to/" + path.TrimStart('/');
    }
}
