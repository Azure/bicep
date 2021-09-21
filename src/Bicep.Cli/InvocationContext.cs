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
            AzResourceTypeProvider azResourceTypeProvider,
            TextWriter outputWriter,
            TextWriter errorWriter,
            string assemblyFileVersion,
            IFeatureProvider? features = null,
            IContainerRegistryClientFactory? clientFactory = null,
            ITemplateSpecRepositoryFactory? templateSpecRepositoryFactory = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            AssemblyFileVersion = assemblyFileVersion;
            Features = features ?? new FeatureProvider();
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory();
            TemplateSpecRepositoryFactory = templateSpecRepositoryFactory ?? new TemplateSpecRepositoryFactory();
            NamespaceProvider = new DefaultNamespaceProvider(azResourceTypeProvider, Features);
        }

        public INamespaceProvider NamespaceProvider { get; }

        public TextWriter OutputWriter { get; } 

        public TextWriter ErrorWriter { get; }

        public string AssemblyFileVersion { get; }

        public EmitterSettings EmitterSettings
            => new EmitterSettings(AssemblyFileVersion, enableSymbolicNames: Features.SymbolicNameCodegenEnabled);

        public IFeatureProvider Features { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; }
    }
}
