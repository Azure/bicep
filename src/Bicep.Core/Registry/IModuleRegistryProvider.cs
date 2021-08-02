// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Represents the configured module registries for the current instance of Bicep.
    /// </summary>
    public interface IModuleRegistryProvider
    {
        ImmutableArray<IModuleRegistry> Registries { get; }
    }
}
