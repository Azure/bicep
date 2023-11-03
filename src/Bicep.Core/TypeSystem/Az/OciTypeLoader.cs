// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using Azure.Bicep.Types;

namespace Bicep.Core.TypeSystem
{
    public class OciTypeLoader : TypeLoader
    {
        private readonly ImmutableDictionary<string, byte[]> typesCache;
        private const string typesArtifactFilename = "types.tgz";
        private OciTypeLoader(ImmutableDictionary<string, byte[]> typesCache)
        {
            this.typesCache = typesCache;
        }

        public static OciTypeLoader FromTgz(string typesFileDir)
        {
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            using var fileStream = File.OpenRead(Path.Combine(typesFileDir, typesArtifactFilename));
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gzipStream);

            var buffer = new byte[4096]; // Use a larger buffer size for improved I/O performance

            while (tarReader.GetNextEntry() is { } entry)
            {
                if (entry.DataStream is null)
                {
                    var errorMessage = $"Failed to restore {entry.Name} from OCI provider data";
                    throw new InvalidOperationException(errorMessage);
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
            return new OciTypeLoader(typesCache);
        }

        protected override Stream GetContentStreamAtPath(string path)
        {
            if (typesCache.TryGetValue(path, out var bytes))
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
