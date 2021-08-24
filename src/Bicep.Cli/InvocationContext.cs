// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.TypeSystem;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(IResourceTypeProvider resourceTypeProvider, TextWriter outputWriter, TextWriter errorWriter, EmitterSettings emitterSettings)
        {
            ResourceTypeProvider = resourceTypeProvider;
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            EmitterSettings = emitterSettings;
        }

        public IResourceTypeProvider ResourceTypeProvider { get; }
        public TextWriter OutputWriter { get; } 
        public TextWriter ErrorWriter { get; }
        public EmitterSettings EmitterSettings { get; }
    }
}
