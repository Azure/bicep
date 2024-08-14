// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry;

namespace Bicep.Wasm
{
    public class EmptyModuleRegistryProvider : IArtifactRegistryProvider
    {
        public ImmutableArray<IArtifactRegistry> Registries(Uri _) => [];
    }
}
