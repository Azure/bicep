// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.EventSources.ManifestInstaller;
using Newtonsoft.Json;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private record ResourceTypeLoaderKey(string Name, string Version);
        private readonly ResourceTypeLoaderKey BuiltInAzResourceTypeLoaderKey = new(AzNamespaceType.BuiltInName, AzNamespaceType.Settings.ArmTemplateProviderVersion);
        private readonly Dictionary<ResourceTypeLoaderKey, IResourceTypeProvider> cachedResourceTypeLoaders;

        public ResourceTypeProviderFactory()
        {
            cachedResourceTypeLoaders = new() {
                {BuiltInAzResourceTypeLoaderKey, new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader()),AzNamespaceType.Settings.ArmTemplateProviderVersion)},
            };
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(ResourceTypesProviderDescriptor providerDescriptor, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled)
            {
                return new(cachedResourceTypeLoaders[BuiltInAzResourceTypeLoaderKey]);
            }
            var key = new ResourceTypeLoaderKey(providerDescriptor.Alias, providerDescriptor.Version);

            if (cachedResourceTypeLoaders.ContainsKey(key))
            {
                return new(cachedResourceTypeLoaders[key]);
            }

            if (providerDescriptor.Path == null)
            {
                // should never happen since builtin providers are handled prior and path is required for non-builtin providers
                throw new ArgumentNullException("Provider filepath is null");
            }

            // compose the path to the OCI manifest based on the cache root directory and provider version
            var ociManifestPath = Path.Combine(providerDescriptor.Path, "manifest");
            if (!File.Exists(ociManifestPath))
            {
                return new(x => x.ArtifactFilePathCouldNotBeResolved(ociManifestPath));
            }

            // Read the OCI manifest
            var manifestFileContents = File.ReadAllText(ociManifestPath);
            OciManifest? ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents);

            if (ociManifest is null)
            {
                return new(x => x.ErrorOccurredReadingFile(ociManifestPath)); ;
            }

            // Register a new types loader
            IResourceTypeProvider newResourceTypeLoader = providerDescriptor.Alias switch
            {
                AzNamespaceType.BuiltInName => new AzResourceTypeProvider(new AzResourceTypeLoader(OciTypeLoader.FromTgz(providerDescriptor.Path)), providerDescriptor.Version),
                _ => throw new NotImplementedException($"The provider {providerDescriptor.Alias} is not supported."),
            };

            return new(cachedResourceTypeLoaders[key] = newResourceTypeLoader);

        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
        {
            return cachedResourceTypeLoaders[BuiltInAzResourceTypeLoaderKey];
        }
    }
}
