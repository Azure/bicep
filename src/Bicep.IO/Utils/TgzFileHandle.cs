// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Formats.Tar;
using System.IO.Compression;
using Bicep.IO.Abstraction;

namespace Bicep.IO.Utils
{
    public class TgzFileHandle
    {
        public TgzFileHandle(IFileHandle fileHandle)
        {
            this.FileHandle = fileHandle;
        }

        public IFileHandle FileHandle { get; }

        public bool Exists() => this.FileHandle.Exists();

        public FrozenDictionary<string, string> Extract()
        {
            if (!this.Exists())
            {
                throw new InvalidOperationException($"The file {this.FileHandle.Uri} does not exist.");
            }

            var entries = new Dictionary<string, string>();
            using var stream = this.FileHandle.OpenRead();
            using var tgzReader = new TgzReader(stream);

            while (tgzReader.GetNextEntry() is { } entry)
            {
                entries.Add(entry.Name, entry.Contents);
            }

            return entries.ToFrozenDictionary();
        }
    }
}
