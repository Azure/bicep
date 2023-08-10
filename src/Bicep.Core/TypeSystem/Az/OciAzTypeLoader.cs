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
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            using var fileStream = File.OpenRead(pathToGzip);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gzipStream);

            var buffer = new byte[4096]; // Use a larger buffer size for improved I/O performance

            while (tarReader.GetNextEntry() is { } entry)
            {
                if (entry.DataStream is null)
                {
                    var errorMessage = $"Failed to restore {entry.Name} from OCI provider data";
                    throw new ArgumentException(errorMessage, nameof(entry.Name));
                }

                using var memoryStream = new MemoryStream();
                int bytesRead;
                while ((bytesRead = entry.DataStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                var typeData = memoryStream.ToArray();
                typesCacheBuilder.Add(entry.Name, typeData);
            }

            var typesCache = typesCacheBuilder.ToImmutableDictionary();
            return new OciAzTypeLoader(typesCache);
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
