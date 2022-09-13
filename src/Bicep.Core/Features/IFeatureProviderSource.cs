// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features;

public interface IFeatureProviderSource
{
    /// <summary>
    /// A priority ranking for this source. Sources that report numerically lower priority rankings will be queried
    /// first (e.g., a source that returns 10 for this property will override a source that reports 11 for this)
    /// property. The values 0 and 1 are used by the environment variable and bicepconfig.json sources, so use a
    /// negative value to take precedence over the sources built into Bicep.Core.
    /// </summary>
    sbyte Priority { get; }

    string? AssemblyVersion => default;

    string? CacheRootDirectory => default;

    bool? RegistryEnabled => default;

    bool? SymbolicNameCodegenEnabled => default;

    bool? ImportsEnabled => default;

    bool? ResourceTypedParamsAndOutputsEnabled => default;

    bool? SourceMappingEnabled => default;

    bool? ParamsFilesEnabled => default;
}
