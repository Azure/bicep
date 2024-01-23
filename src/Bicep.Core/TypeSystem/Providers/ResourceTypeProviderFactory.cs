// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.IO.Abstractions;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.ThirdParty;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private static readonly Lazy<IResourceTypeProvider> azResourceTypeProviderLazy
            = new(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader()), AzNamespaceType.Settings.ArmTemplateProviderVersion));

        private record ResourceTypeLoaderKey(string Name, string Version);
        private readonly ConcurrentDictionary<ResourceTypeLoaderKey, IResourceTypeProvider> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProviderFromFilePath(ResourceTypesProviderDescriptor providerDescriptor)
        {



            var key = new ResourceTypeLoaderKey(providerDescriptor.Name, providerDescriptor.Version);
            IResourceTypeProvider result;
            try
            {
                result = cachedResourceTypeLoaders.GetOrAdd(key, _ =>
                {

                    if (providerDescriptor.TypesBaseUri is null)
                    {
                        throw new ArgumentException($"Provider {providerDescriptor.Name} requires a types base URI.");
                    }
                    if (providerDescriptor.Name != AzNamespaceType.BuiltInName)
                    {
                        return new ThirdPartyResourceTypeProvider(
                                    new ThirdPartyResourceTypeLoader(
                                        OciTypeLoader.FromDisk(fileSystem, providerDescriptor.TypesBaseUri)), 
                                        providerDescriptor.Version);
                    }
                    
                    return new AzResourceTypeProvider(
                                new AzResourceTypeLoader(
                                    OciTypeLoader.FromDisk(fileSystem, providerDescriptor.TypesBaseUri)),
                                    providerDescriptor.Version);
                });
            }
            catch (Exception ex)
            {
                var invalidArtifactException = ex as InvalidArtifactException ?? new InvalidArtifactException(ex.Message, ex, InvalidArtifactExceptionKind.NotSpecified);
                return new(x => x.ArtifactRestoreFailedWithMessage(providerDescriptor.ArtifactReference, invalidArtifactException.Message));
            }
            return new(result);
        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
            => azResourceTypeProviderLazy.Value;
    }
}
