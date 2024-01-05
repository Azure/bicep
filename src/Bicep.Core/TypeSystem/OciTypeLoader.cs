// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Formats.Tar;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using Azure.Bicep.Types;

namespace Bicep.Core.TypeSystem
{
    public class InvalidOciResourceTypesProviderArtifactException : Exception
    {
        public InvalidOciResourceTypesProviderArtifactException(string message) : base(message) { }
    }


    public class OciTypeLoader : TypeLoader
    {
        private readonly ImmutableDictionary<string, byte[]> typesCache;
        public const string TypesArtifactFilename = "types.tgz";
        private OciTypeLoader(ImmutableDictionary<string, byte[]> typesCache)
        {
            this.typesCache = typesCache;
        }

        private static OciTypeLoader FromDiskHelper(IFileSystem fs, Uri? typesTgzUri)
        {
            if (typesTgzUri is null)
            {
                throw new ArgumentNullException(nameof(typesTgzUri));
            }

            using var stream = fs.File.OpenRead(typesTgzUri.LocalPath);

            return FromStream(stream);
        }

        public static OciTypeLoader FromDisk(IFileSystem fs, Uri? typesTgzUri)
        {
            try
            {
                return FromDiskHelper(fs, typesTgzUri);
            }
            catch (Exception e)
            {
                throw new InvalidOciResourceTypesProviderArtifactException(e.Message);
            }
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
                throw new InvalidOciResourceTypesProviderArtifactException($"{path} not found.");
            }
        }
    }
}
