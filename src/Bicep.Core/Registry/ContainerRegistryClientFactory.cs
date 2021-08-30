// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.RegistryClient;
using System;

namespace Bicep.Core.Registry
{
    public class ContainerRegistryClientFactory : IContainerRegistryClientFactory
    {
        public BicepRegistryBlobClient CreateBlobClient(Uri registryUri, string repository, TokenCredential credential)
        {
            var options = new ContainerRegistryClientOptions();

            // ensure User-Agent mentions us
            options.Diagnostics.ApplicationId = $"{LanguageConstants.LanguageId}/{ThisAssembly.AssemblyFileVersion}";

            return new(registryUri, credential, repository, options);
        }
    }
}
