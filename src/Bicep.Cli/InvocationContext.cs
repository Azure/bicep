// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(
            IAzResourceTypeLoader azResourceTypeLoader,
            TextWriter outputWriter,
            TextWriter errorWriter,
            IFeatureProviderFactory? featureProviderFactory = null,
            IContainerRegistryClientFactory? clientFactory = null,
            ITemplateSpecRepositoryFactory? templateSpecRepositoryFactory = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            FeatureProviderFactory = featureProviderFactory;
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory(new TokenCredentialFactory());
            TemplateSpecRepositoryFactory = templateSpecRepositoryFactory ?? new TemplateSpecRepositoryFactory(new TokenCredentialFactory());
            NamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoader);
        }

        public INamespaceProvider NamespaceProvider { get; }

        public TextWriter OutputWriter { get; }

        public TextWriter ErrorWriter { get; }

        public IFeatureProviderFactory? FeatureProviderFactory { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; }
    }
}
