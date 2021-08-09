// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using System.Collections.Immutable;

namespace Bicep.Wasm
{
    public class EmptyModuleRegistryProvider : IModuleRegistryProvider
    {
        public ImmutableArray<IModuleRegistry> Registries => ImmutableArray<IModuleRegistry>.Empty;
    }
}
