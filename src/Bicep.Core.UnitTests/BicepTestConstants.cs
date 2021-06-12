// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;

namespace Bicep.Core.UnitTests
{
    public static class BicepTestConstants 
    {
        public const string DevAssemblyFileVersion = "dev";
        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";
        public static readonly FileResolver FileResolver = new ();        
    }
}
