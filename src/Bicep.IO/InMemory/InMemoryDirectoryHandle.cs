// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class InMemoryDirectoryHandle : InMemoryIOHandle, IDirectoryHandle
    {
        public InMemoryDirectoryHandle(InMemoryFileExplorer.FileStore fileStore, IOUri uri)
            : base(fileStore, EnsureTrailingSlash(uri))
        {
        }

        public void Delete() => this.FileStore.DeleteDirectory(this);

        public IDirectoryHandle EnsureExists()
        {
            this.FileStore.CreateDirectoryIfNotExists(this);

            return this;
        }

        public IEnumerable<IDirectoryHandle> EnumerateDirectories(string searchPattern = "")
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                return this.FileStore.FindDirectories(directory => directory.GetParent() is { } parent && parent.Equals(this));
            }

            ValidateSearchPattern(searchPattern);

            return this.FileStore.FindDirectories(directory =>
                directory.GetParent() is { } parent &&
                parent.Equals(this) &&
                WildcardMatch(directory.Uri.PathSegments.Last(), searchPattern));
        }

        public IEnumerable<IFileHandle> EnumerateFiles(string searchPattern = "")
        {
            if (searchPattern is "")
            {
                return this.FileStore.FindFiles(file => file.GetParent().Equals(this));
            }

            ValidateSearchPattern(searchPattern);

            return this.FileStore.FindFiles(file =>
                file.GetParent().Equals(this) &&
                WildcardMatch(file.Uri.PathSegments.Last(), searchPattern));
        }

        public override bool Exists() => this.FileStore.DirectoryExists(this);

        public IDirectoryHandle GetDirectory(string relativePath)
        {
            var directoryUri = this.Uri.Resolve(relativePath);

            return new InMemoryDirectoryHandle(this.FileStore, directoryUri);
        }

        public IFileHandle GetFile(string relativePath)
        {
            var fileUri = this.Uri.Resolve(relativePath);

            return new InMemoryFileHandle(this.FileStore, fileUri);
        }

        public IDirectoryHandle? GetParent()
        {
            var parentUri = this.Uri.Resolve("..");
            return this.Uri == parentUri ? null : new InMemoryDirectoryHandle(this.FileStore, parentUri);
        }

        private static IOUri EnsureTrailingSlash(IOUri uri) => uri.Path.EndsWith('/') ? uri : new IOUri(uri.Scheme, uri.Authority, uri.Path + "/");

        private static void ValidateSearchPattern(string searchPattern)
        {
            if (searchPattern.EndsWith("..") ||
                searchPattern.Contains("../") ||
                searchPattern.Contains(@"..\") ||
                Path.GetInvalidPathChars().Any(searchPattern.Contains))
            {
                throw new ArgumentException("Invalid search pattern", nameof(searchPattern));
            }
        }

        private static bool WildcardMatch(string value, string pattern)
        {
            var matchMatrix = new bool[value.Length + 1, pattern.Length + 1];
            matchMatrix[0, 0] = true;

            for (var i = 1; i <= pattern.Length; i++)
            {
                if (pattern[i - 1] == '*')
                {
                    matchMatrix[0, i] = matchMatrix[0, i - 1];
                }
            }

            for (int i = 1; i <= value.Length; i++)
            {
                for (int j = 1; j <= pattern.Length; j++)
                {
                    if (pattern[j - 1] == '*')
                    {
                        matchMatrix[i, j] = matchMatrix[i - 1, j] || matchMatrix[i, j - 1];
                    }
                    else if (pattern[j - 1] == '?' || value[i - 1] == pattern[j - 1])
                    {
                        matchMatrix[i, j] = matchMatrix[i - 1, j - 1];
                    }
                }
            }

            return matchMatrix[value.Length, pattern.Length];
        }
    }
}
