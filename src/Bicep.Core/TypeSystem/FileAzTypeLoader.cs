// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Serialization;
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

        public static FileAzTypeLoader FromFile(string pathToIndex)
        {
            var typesCacheBuilder = ImmutableDictionary.CreateBuilder<string, byte[]>();

            var uniqueTypePaths = getAllUniqueTypesRelativePaths(pathToIndex);
            var directoryName = Path.GetDirectoryName(pathToIndex);

            var indexByteArray = File.ReadAllBytes(pathToIndex);
            typesCacheBuilder.Add("index.json", indexByteArray);

            foreach ( var uniqueTypePath in uniqueTypePaths)
            {
                var typeByteArray = File.ReadAllBytes(directoryName + "\\" + uniqueTypePath);
                typesCacheBuilder.Add("types.json", typeByteArray);
            }


            typesCacheBuilder.Add("index.json", indexByteArray);

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
        private static HashSet<string> getAllUniqueTypesRelativePaths(string pathToIndex)
        {
            FileStream indexStream = new(pathToIndex, FileMode.Open, FileAccess.Read);

            var index = TypeSerializer.DeserializeIndex(indexStream);

            HashSet<string> uniqueTypeRelativePaths = new();

            foreach (var typeInformation in index.Resources)
            {
                uniqueTypeRelativePaths.Add(typeInformation.Value.RelativePath);
            }

            return uniqueTypeRelativePaths;
        }
    }
}
