// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.IO.Compression;
using Azure.Bicep.Types;
using Azure.Bicep.Types.K8s;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.K8s
{
    public class K8sResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly ExtensionResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;

        public K8sResourceTypeLoader()
        {
            typeLoader = new K8sTypeLoader();
            var typeIndex = typeLoader.LoadTypeIndex();
            resourceTypeFactory = new ExtensionResourceTypeFactory(typeIndex.Settings);
            availableTypes = typeIndex.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }
    }

    public class K8sTypeLoader : TypeLoader
    {
        protected override Stream GetContentStreamAtPath(string path)
        {
            var manifestResourceStream = typeof(K8sTypeLoader).Assembly.GetManifestResourceStream(path + ".deflated");
            if (manifestResourceStream == null)
            {
                throw new ArgumentException("Unable to locate manifest resource at path " + path, "path");
            }

            return new DeflateStream(manifestResourceStream, CompressionMode.Decompress);
        }
    }
}
