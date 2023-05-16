// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using Azure.Bicep.Types;
using System.Formats.Tar;
using System.IO.Compression;
using System;
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem.Az
{
    public class OciAzTypeLoader : TypeLoader
    {
        private readonly Dictionary<string, byte[]> typesCache = new(StringComparer.OrdinalIgnoreCase);

        public OciAzTypeLoader(string pathToGzip) : base()
        {
            using var fileStream = File.OpenRead(pathToGzip);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gzipStream, leaveOpen: true);
            while (tarReader.GetNextEntry(copyData: true) is { } entry)
            {
                if (entry.DataStream is null)
                {
                    throw new ArgumentException($"Failed to restore {entry.Name} from OCI provider data", nameof(entry.Name));

                }
                using var ms = new MemoryStream();
                entry.DataStream.CopyTo(ms);
                typesCache.Add(entry.Name, ms.ToArray());
            }
        }

        protected override Stream GetContentStreamAtPath(string path)
        {
            if (this.typesCache.TryGetValue($"./{path}", out var bytes))
            {
                return new MemoryStream(bytes);
            }
            else
            {
                throw new ArgumentException($"Failed to restore {path} from OCI provider data", nameof(path));
            }
        }
    }
}
