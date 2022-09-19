// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(
            IAzResourceTypeLoader azResourceTypeLoader,
            TextWriter outputWriter,
            TextWriter errorWriter,
            IFeatureProvider? features = null,
            IContainerRegistryClientFactory? clientFactory = null,
            ITemplateSpecRepositoryFactory? templateSpecRepositoryFactory = null,
            IFileResolver? fileResolver = null,
            IConfigurationManager? configurationManager = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            Features = features ?? new FeatureProvider();
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory(new TokenCredentialFactory());
            TemplateSpecRepositoryFactory = templateSpecRepositoryFactory ?? new TemplateSpecRepositoryFactory(new TokenCredentialFactory());
            FileResolver = fileResolver ?? new FileResolver();
            ConfigurationManager = configurationManager ?? new ConfigurationManager(new FileSystem());
            NamespaceProvider = new RegistryAwareNamespaceProvider(new DefaultNamespaceProvider(azResourceTypeLoader, Features), FileResolver, ClientFactory, Features, ConfigurationManager);
        }

        public INamespaceProvider NamespaceProvider { get; }

        public TextWriter OutputWriter { get; }

        public TextWriter ErrorWriter { get; }

        public EmitterSettings EmitterSettings => new EmitterSettings(Features);

        public IFeatureProvider Features { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; }

        public IFileResolver FileResolver { get; }

        public IConfigurationManager ConfigurationManager { get; }
    }
}
