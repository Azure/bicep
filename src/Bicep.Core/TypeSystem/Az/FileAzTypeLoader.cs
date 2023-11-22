// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Text;
using Azure.Bicep.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.TypeSystem.Az
{
    public class FileAzTypeLoader : TypeLoader
    {
        private readonly ImmutableDictionary<string, byte[]> typesCache;

        public FileAzTypeLoader(ImmutableDictionary<string, byte[]> typesCache)
        {
            this.typesCache = typesCache;
        }

        public static FileAzTypeLoader FromFile(string indexJsonString, string pathToType)
        {
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            var indexByteArray = Encoding.UTF8.GetBytes(indexJsonString);
            var typeByteArray = File.ReadAllBytes(pathToType);

            typesCacheBuilder.Add("index.json", indexByteArray);
            typesCacheBuilder.Add("types.json", typeByteArray);

            var typesCache = typesCacheBuilder.ToImmutableDictionary();

            return new FileAzTypeLoader(typesCache);
        }

        public static FileAzTypeLoader ToResourceTypes(string pathToIndex, string pathToType)
        {
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            var indexByteArray = File.ReadAllBytes(pathToIndex);
            var typeByteArray = File.ReadAllBytes(pathToIndex);

            typesCacheBuilder.Add(Path.GetFileName(pathToIndex), indexByteArray);
            typesCacheBuilder.Add(Path.GetFileName(pathToType), typeByteArray);

            var typesCache = typesCacheBuilder.ToImmutableDictionary();

            return new FileAzTypeLoader(typesCache);
        }


        protected override Stream GetContentStreamAtPath(string path)
        {
            if (this.typesCache.TryGetValue($"{path}", out var bytes))
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
