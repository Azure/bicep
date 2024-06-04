// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Abstractions;
using System.IO.Compression;
using Azure.Bicep.Types;
using Bicep.Core.Registry;

namespace Bicep.Core.TypeSystem
{


    public class OciTypeLoader : TypeLoader
    {
        private readonly ImmutableDictionary<string, byte[]> typesCache;
        private OciTypeLoader(ImmutableDictionary<string, byte[]> typesCache)
        {
            this.typesCache = typesCache;
        }

        public static OciTypeLoader FromStream(Stream stream)
        {
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gzipStream);

            var buffer = new byte[4096]; // Use a larger buffer size for improved I/O performance

            while (tarReader.GetNextEntry() is { } entry)
            {
                if (entry.DataStream is null)
                {
                    throw new InvalidOperationException($"Stream for {entry.Name} is null.");
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
                Trace.WriteLine($"{nameof(GetContentStreamAtPath)} threw an exception. Requested path: '{path}' not found.");
                throw new InvalidArtifactException($"The path: {path} was not found in artifact contents", InvalidArtifactExceptionKind.InvalidArtifactContents);
            }
        }
    }
}
