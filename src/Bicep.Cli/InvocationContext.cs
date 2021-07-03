// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using System;
using System.IO;

namespace Bicep.Cli
{
    public class InvocationContext
    {
        public IResourceTypeProvider ResourceTypeProvider { get; set; } = AzResourceTypeProvider.CreateWithAzTypes();
        public TextWriter OutputWriter { get; set; } = Console.Out;
        public TextWriter ErrorWriter { get; set; } = Console.Error;
        public string AssemblyFileVersion { get; set; } = ThisAssembly.AssemblyFileVersion;
    }
}
