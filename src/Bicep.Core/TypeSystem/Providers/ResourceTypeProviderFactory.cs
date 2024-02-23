// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics;
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
        private readonly ConcurrentDictionary<ResourceTypeLoaderKey, ResultWithDiagnostic<IResourceTypeProvider>> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(ResourceTypesProviderDescriptor providerDescriptor)
        {
            var key = new ResourceTypeLoaderKey(providerDescriptor.Name, providerDescriptor.Version);
            return cachedResourceTypeLoaders.GetOrAdd(key, _ =>
            {
                try
                {
                    return GetDynamicallyLoadedResourceTypesProvider(providerDescriptor);
                }
                catch (Exception ex)
                {
                    var fullyQualifiedArtifactReference = providerDescriptor.ArtifactReference?.FullyQualifiedReference ?? throw new UnreachableException($"the reference is validated prior to a call to {nameof(this.GetResourceTypeProvider)}");
                    var invalidArtifactException = ex as InvalidArtifactException ?? new InvalidArtifactException(ex.Message, ex, InvalidArtifactExceptionKind.NotSpecified);
                    return new(x => x.ArtifactRestoreFailedWithMessage(fullyQualifiedArtifactReference, invalidArtifactException.Message));
                }
            });
        }
        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
           => azResourceTypeProviderLazy.Value;
        private ResultWithDiagnostic<IResourceTypeProvider> GetDynamicallyLoadedResourceTypesProvider(ResourceTypesProviderDescriptor providerDescriptor)
        {
            var fullyQualifiedArtifactReference = providerDescriptor.ArtifactReference?.FullyQualifiedReference ?? throw new UnreachableException($"the reference is validated prior to a call to {nameof(this.GetResourceTypeProvider)}");
            if (providerDescriptor.TypesTgzUri is null)
            {
                return new(x => x.ArtifactRestoreFailedWithMessage(
                    fullyQualifiedArtifactReference,
                    $"Provider {providerDescriptor.Name} requires a types base URI."));
            }

            var typesLoader = OciTypeLoader.FromDisk(fileSystem, providerDescriptor.TypesTgzUri);
            if (providerDescriptor.Name == AzNamespaceType.BuiltInName)
            {
                return new(new AzResourceTypeProvider(new AzResourceTypeLoader(typesLoader), providerDescriptor.Version));
            }
            return new(new ThirdPartyResourceTypeProvider(new ThirdPartyResourceTypeLoader(typesLoader), providerDescriptor.Version));
        }
    }
}
