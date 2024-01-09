// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private static readonly Lazy<IResourceTypeProvider> azResourceTypeProviderLazy
            = new(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader()), AzNamespaceType.Settings.ArmTemplateProviderVersion));

        private record ResourceTypeLoaderKey(string Name, string Version);
        private readonly Dictionary<ResourceTypeLoaderKey, IResourceTypeProvider> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProviderFromFilePath(ResourceTypesProviderDescriptor providerDescriptor)
        {
            var key = new ResourceTypeLoaderKey(providerDescriptor.Alias, providerDescriptor.Version);

            if (cachedResourceTypeLoaders.ContainsKey(key))
            {
                return new(cachedResourceTypeLoaders[key]);
            }

            IResourceTypeProvider newResourceTypeLoader;
            try
            {
                if (providerDescriptor.Name != AzNamespaceType.BuiltInName)
                {
                    // Note (asilverman): the line of code below is meant for 3rd party provider resolution logic which is not yet implemented.
                    throw new NotImplementedException($"Provider {providerDescriptor.Name} not supported.");
                }
                newResourceTypeLoader =
                new AzResourceTypeProvider(
                    new AzResourceTypeLoader(
                        OciTypeLoader.FromDisk(fileSystem, providerDescriptor.TypesBaseUri)),
                        providerDescriptor.Version);
            }
            catch (Exception ex)
            {
                var invalidArtifactException = ex.As<InvalidArtifactException?>() ?? new InvalidArtifactException(ex.Message, ex, InvalidArtifactExceptionKind.NotSpecified);
                return new(x => x.ArtifactRestoreFailedWithMessage(providerDescriptor.ArtifactReference, invalidArtifactException.Message));
            }

            return new(cachedResourceTypeLoaders[key] = newResourceTypeLoader);
        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
            => azResourceTypeProviderLazy.Value;
    }
}
