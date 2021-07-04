// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public InvocationContext(IResourceTypeProvider resourceTypeProvider, TextWriter outputWriter, TextWriter errorWriter, string assemblyFileVersion)
        {
            ResourceTypeProvider = resourceTypeProvider;
            OutputWriter = outputWriter;
            ErrorWriter = errorWriter;
            AssemblyFileVersion = assemblyFileVersion;
        }

        public IResourceTypeProvider ResourceTypeProvider { get; }
        public TextWriter OutputWriter { get; } 
        public TextWriter ErrorWriter { get; }
        public string AssemblyFileVersion { get; }
    }
}
