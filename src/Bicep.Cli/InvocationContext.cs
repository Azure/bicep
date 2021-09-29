// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
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
            IFeatureProvider? features = null,
            IContainerRegistryClientFactory? clientFactory = null,
            ITemplateSpecRepositoryFactory? templateSpecRepositoryFactory = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            Features = features ?? new FeatureProvider();
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory();
            TemplateSpecRepositoryFactory = templateSpecRepositoryFactory ?? new TemplateSpecRepositoryFactory();
            NamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoader, Features);
        }

        public INamespaceProvider NamespaceProvider { get; }

        public TextWriter OutputWriter { get; } 

        public TextWriter ErrorWriter { get; }

        public EmitterSettings EmitterSettings => new EmitterSettings(Features);

        public IFeatureProvider Features { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; }
    }
}
