// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry;

namespace Bicep.Wasm
{
    public class EmptyModuleRegistryProvider : ArtifactRegistryProvider
    {
        public EmptyModuleRegistryProvider()
            : base(Array.Empty<IArtifactRegistry>())
        {
        }
    }
}
