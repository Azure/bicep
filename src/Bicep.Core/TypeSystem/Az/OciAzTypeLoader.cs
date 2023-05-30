// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using Azure.Bicep.Types;
using System.Formats.Tar;
using System.IO.Compression;
using System;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Az
{
    public class OciAzTypeLoader : TypeLoader
    {
        private readonly ImmutableDictionary<string, byte[]> typesCache;

        public OciAzTypeLoader(ImmutableDictionary<string, byte[]> typesCache)
        {
            this.typesCache = typesCache;
        }

        public static OciAzTypeLoader FromTgz(string pathToGzip)
        {
            var typesCache = ImmutableDictionary.CreateBuilder<string, byte[]>();
            using var fileStream = File.OpenRead(pathToGzip);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gzipStream);
            while (tarReader.GetNextEntry() is { } entry)
            {
                if (entry.DataStream is null)
                {
                    throw new ArgumentException($"Failed to restore {entry.Name} from OCI provider data", nameof(entry.Name));
                }
                using var br = new BinaryReader(entry.DataStream);
                typesCache.Add(entry.Name, br.ReadBytes((int)entry.DataStream.Length));
            }
            return new(typesCache.ToImmutableDictionary());
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
