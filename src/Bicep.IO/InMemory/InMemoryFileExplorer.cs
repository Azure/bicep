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

        public IDirectoryHandle GetDirectory(IOUri uri) => new InMemoryDirectoryHandle(this.fileStore, EnsureLocalFileUri(uri));

        public IFileHandle GetFile(IOUri uri) => new InMemoryFileHandle(this.fileStore, EnsureLocalFileUri(uri));

        private static IOUri EnsureLocalFileUri(IOUri uri)
        {
            if (!uri.IsFile)
            {
                throw new ArgumentException($"The in-memory file explorer only supports local file URIs.");
            }

            return uri;
        }

        public class FileStore
        {
            private readonly ConcurrentDictionary<InMemoryDirectoryHandle, bool> directoryEntries = new();
            private readonly ConcurrentDictionary<InMemoryFileHandle, FileEntry> fileEntries = new();
            private long writeCounter;

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

            public bool FileExists(InMemoryFileHandle file) => this.fileEntries.ContainsKey(file);

            public void CreateFileIfNotExists(InMemoryFileHandle file) => this.fileEntries.TryAdd(file, this.CreateEntry(BinaryData.Empty));

            public void DeleteFile(InMemoryFileHandle file) => this.fileEntries.TryRemove(file, out _);

            public IEnumerable<InMemoryFileHandle> FindFiles(Predicate<InMemoryFileHandle> predicate)
            {
                foreach (var fileHandle in this.fileEntries.Keys)
                {
                    if (predicate(fileHandle))
                    {
                        yield return fileHandle;
                    }
                }
            }

            public void WriteFile(InMemoryFileHandle file, string text) => this.WriteFile(file, BinaryData.FromString(text));

            public void WriteFile(InMemoryFileHandle file, BinaryData data) => this.fileEntries[file] = this.CreateEntry(data);

            public BinaryData ReadFile(InMemoryFileHandle file) => this.fileEntries.TryGetValue(file, out var entry)
                ? entry.Data
                : throw new FileNotFoundException($"File '{file.Uri}' does not exist.");

            public FileVersion? TryGetFileVersion(InMemoryFileHandle file) => this.fileEntries.TryGetValue(file, out var entry) ? entry.Version : null;

            // The counter is shared by all files, so a file that is deleted and written again gets a new version.
            private FileEntry CreateEntry(BinaryData data) => new(data, new(Interlocked.Increment(ref this.writeCounter), data.ToMemory().Length));

            private sealed record FileEntry(BinaryData Data, FileVersion Version);
        }
    }
}
