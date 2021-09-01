// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(
            IResourceTypeProvider resourceTypeProvider,
            TextWriter outputWriter,
            TextWriter errorWriter,
            IFeatureProvider? features = null,
            IContainerRegistryClientFactory? clientFactory = null)
        {
            // keep the list of services in this class in sync with the logic in the AddInvocationContext() extension method
            ResourceTypeProvider = resourceTypeProvider;
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            Features = features ?? new FeatureProvider();
            ClientFactory = clientFactory ?? new ContainerRegistryClientFactory();
        }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public TextWriter OutputWriter { get; } 

        public TextWriter ErrorWriter { get; }

        public EmitterSettings EmitterSettings
            => new EmitterSettings(ThisAssembly.AssemblyFileVersion, enableSymbolicNames: Features.SymbolicNameCodegenEnabled);

        public IFeatureProvider Features { get; }

        public IContainerRegistryClientFactory ClientFactory { get; }
    }
}
