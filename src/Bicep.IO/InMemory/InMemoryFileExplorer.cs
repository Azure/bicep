// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class InMemoryFileExplorer : IFileExplorer
    {
        private readonly FileStore fileStore = new();

        public IDirectoryHandle GetDirectory(IOUri uri) => new InMemoryDirectoryHandle(this.fileStore, EnsureInMemoryUri(uri));

        public IFileHandle GetFile(IOUri uri) => new InMemoryFileHandle(this.fileStore, EnsureInMemoryUri(uri));

        private static IOUri EnsureInMemoryUri(IOUri uri)
        {
            if (!uri.Scheme.IsInMemory)
            {
                throw new ArgumentException($"The URI scheme '{uri.Scheme}' is not supported by the in-memory file explorer.");
            }
            return uri;
        }

        public class FileStore
        {
            private readonly ConcurrentDictionary<InMemoryDirectoryHandle, bool> directoryEntries = new();
            private readonly ConcurrentDictionary<InMemoryFileHandle, string?> fileEntries = new();

            public Guid StoreId { get; } = new();

            public bool DirectoryExists(InMemoryDirectoryHandle directory) => this.directoryEntries.GetValueOrDefault(directory);

            public void CreateDirectoryIfNotExists(InMemoryDirectoryHandle directory) => this.directoryEntries.TryAdd(directory, true);

            public void DeleteDirectory(InMemoryDirectoryHandle directory) => this.directoryEntries.TryRemove(directory, out _);

            public IEnumerable<InMemoryDirectoryHandle> FindDirectories(Predicate<InMemoryDirectoryHandle> predicate)
            {
                foreach (var (directoryHandle, directoryExists) in this.directoryEntries)
                {
                    if (!directoryExists)
                    {
                        continue;
                    }

                    if (predicate(directoryHandle))
                    {
                        yield return directoryHandle;
                    }
                }
            }

            public bool FileExists(InMemoryFileHandle file) => this.fileEntries.GetValueOrDefault(file) is not null;

            public void CreateFileIfNotExists(InMemoryFileHandle file) => this.fileEntries.TryAdd(file, "");

            public void DeleteFile(InMemoryFileHandle file) => this.fileEntries.TryRemove(file, out _);

            public IEnumerable<InMemoryFileHandle> FindFiles(Predicate<InMemoryFileHandle> predicate)
            {
                foreach (var (fileHandle, fileText) in this.fileEntries)
                {
                    if (fileText is null)
                    {
                        continue;
                    }

                    if (predicate(fileHandle))
                    {
                        yield return fileHandle;
                    }
                }
            }

            public void WriteFile(InMemoryFileHandle file, string text) => this.fileEntries.AddOrUpdate(file, text, (_, _) => text);

            public string ReadFile(InMemoryFileHandle file) => this.fileEntries.GetValueOrDefault(file) ?? throw new InvalidOperationException($"File '{file.Uri}' does not exist.");
        }
    }
}
