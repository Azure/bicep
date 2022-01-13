// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;

namespace Bicep.RegistryModuleTool.Options
{
    public class VerboseOption : Option<bool>
    {
        public VerboseOption()
            : base("--verbose", "Show verbose information")
        {
        }
        public override bool Equals(object? obj) => obj is VerboseOption;

        public override int GetHashCode() => typeof(VerboseOption).GetHashCode();
    }
}
