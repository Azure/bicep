// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using System;
using System.Collections.Immutable;

namespace Bicep.Wasm
{
    public class EmptyModuleRegistryProvider : IModuleRegistryProvider
    {
        public ImmutableArray<IModuleRegistry> Registries(Uri _) => ImmutableArray<IModuleRegistry>.Empty;
    }
}
