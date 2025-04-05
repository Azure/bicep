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
        private readonly IFileHandle fileHandle;

        public TgzFileHandle(IFileHandle fileHandle)
        {
            this.fileHandle = fileHandle;
        }

        public bool Exists() => this.fileHandle.Exists();

        public FrozenDictionary<string, string> Extract()
        {
            if (!this.Exists())
            {
                throw new InvalidOperationException($"The file {this.fileHandle.Uri} does not exist.");
            }

            var entries = new Dictionary<string, string>();
            using var stream = this.fileHandle.OpenRead();
            using var tgzReader = new TgzReader(stream);

            while (tgzReader.GetNextEntry() is { } entry)
            {
                entries.Add(entry.Name, entry.Contents);
            }

            return entries.ToFrozenDictionary();
        }
    }
}
