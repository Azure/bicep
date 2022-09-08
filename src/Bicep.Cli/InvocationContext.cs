// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(
            TextWriter outputWriter,
            TextWriter errorWriter,
            INamespaceProviderManager? namespaceProviderManager = null,
            IFeatureProviderManager? featureProviderManager = null,
            IContainerRegistryClientFactory? clientFactory = null,
            ITemplateSpecRepositoryFactory? templateSpecRepositoryFactory = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory(new TokenCredentialFactory());
            TemplateSpecRepositoryFactory = templateSpecRepositoryFactory ?? new TemplateSpecRepositoryFactory(new TokenCredentialFactory());
            FeatureProviderManager = featureProviderManager;
            NamespaceProviderManager = namespaceProviderManager;
        }

        public INamespaceProviderManager? NamespaceProviderManager { get; }

        public TextWriter OutputWriter { get; }

        public TextWriter ErrorWriter { get; }

        public IFeatureProviderManager? FeatureProviderManager { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; }
    }
}
