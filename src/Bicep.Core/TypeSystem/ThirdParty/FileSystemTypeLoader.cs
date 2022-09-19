// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using ConcreteResourceType = Azure.Bicep.Types.Concrete.ResourceType;

namespace Bicep.Core.TypeSystem.ThirdParty
{
    public class FileSystemTypeLoader : ITypeLoader
    {
        private const string TypeIndexResourceName = "index.json";
        private readonly string basePath;

        public FileSystemTypeLoader(string basePath)
        {
            this.basePath = basePath;
        }

        private TypeBase LoadType(TypeLocation typeLocation)
        {
            var content = GetContentAtPath(typeLocation.RelativePath);

            var types = TypeSerializer.Deserialize(content);

            return types[typeLocation.Index];
        }

        public ConcreteResourceType LoadResourceType(TypeLocation typeLocation)
        {
            if (LoadType(typeLocation) is not ConcreteResourceType resourceType)
            {
                throw new ArgumentException($"Unable to locate resource type at index {typeLocation.Index} in \"{typeLocation.RelativePath}\" resource");
            }

            return resourceType;
        }

        public ResourceFunctionType LoadResourceFunctionType(TypeLocation typeLocation)
        {
            if (LoadType(typeLocation) is not ResourceFunctionType resourceFunctionType)
            {
                throw new ArgumentException($"Unable to locate resource function type at index {typeLocation.Index} in \"{typeLocation.RelativePath}\" resource");
            }

            return resourceFunctionType;
        }

        public string GetContentAtPath(string? path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var fullPath = Path.Combine(basePath, path);
            return File.ReadAllText(fullPath);
        }

        public TypeIndex GetIndexedTypes()
        {
            var content = GetContentAtPath(TypeIndexResourceName);

            return TypeIndexer.DeserializeIndex(content);
        }
    }
}
