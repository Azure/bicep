// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;

namespace Bicep.TextFixtures.IO
{
    public static class TestFileUri
    {
        private static readonly MockFileSystem MockFileSystem = new();

        // Set by TestFileSetScope before each compilation; Not cleared on dispose so test assertion code can still read it.
        internal static readonly AsyncLocal<string?> CurrentScopePrefix = new();

        public static IOUri FromInMemoryPath(string path) => IOUri.FromFilePath(NormalizePath(ApplyScopePrefix(path)));

        public static IOUri FromMockFileSystemPath(string path) => IOUri.FromFilePath(MockFileSystem.Path.GetFullPath(NormalizePath(ApplyScopePrefix(path))));

        // Prepend "/path/to" to enable use of ".." in tests for convenience.
        private static string NormalizePath(string path) => "/path/to/" + path.TrimStart('/');

        // If a scope prefix is active and the path is not already prefixed, apply
        // "{prefix}/files/{path}" — the same layout TestFileSetScope uses when storing files.
        private static string ApplyScopePrefix(string path)
        {
            var prefix = CurrentScopePrefix.Value;
            if (string.IsNullOrEmpty(prefix) || path.StartsWith(prefix + "/", StringComparison.Ordinal))
            {
                return path;
            }
            return $"{prefix}/files/{path.TrimStart('/')}";
        }
    }
}
