// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.IO.Abstraction;

namespace Bicep.IO.Utils
{
    public static class TgzFileExtractor
    {
        public static FrozenDictionary<string, BinaryData> ExtractFromFileHandle(IFileHandle tgzFileHandle)
        {
            if (!tgzFileHandle.Exists())
            {
                throw new InvalidOperationException($"The file {tgzFileHandle.Uri} does not exist.");
            }

            return ExtractFromStream(tgzFileHandle.OpenRead());
        }

        public static FrozenDictionary<string, BinaryData> ExtractFromStream(Stream tgzStream)
        {
            var entries = new Dictionary<string, BinaryData>();
            using var tgzReader = new TgzReader(tgzStream);

            while (tgzReader.GetNextEntry() is { } entry)
            {
                entries.Add(entry.Name, entry.Data);
            }

            return entries.ToFrozenDictionary();
        }
    }
}
