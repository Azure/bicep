// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics;

namespace Bicep.Core.Features;

internal class EnvironmentVariablesFeatureProviderSource : IFeatureProviderSource
{
    sbyte IFeatureProviderSource.Priority => 0;

    public string? CacheRootDirectory => Environment.GetEnvironmentVariable("BICEP_CACHE_DIRECTORY");

    public bool? SymbolicNameCodegenEnabled => ReadBooleanEnvVar("BICEP_SYMBOLIC_NAME_CODEGEN_EXPERIMENTAL");

    public bool? ImportsEnabled => ReadBooleanEnvVar("BICEP_IMPORTS_ENABLED_EXPERIMENTAL");

    public bool? ResourceTypedParamsAndOutputsEnabled => ReadBooleanEnvVar("BICEP_RESOURCE_TYPED_PARAMS_AND_OUTPUTS_EXPERIMENTAL");

    public bool? SourceMappingEnabled => ReadBooleanEnvVar("BICEP_SOURCEMAPPING_ENABLED");

    public bool? paramsFilesEnabledLazy => ReadBooleanEnvVar("BICEP_PARAMS_FILES_ENABLED");

    private static bool? ReadBooleanEnvVar(string envVar)
    {
        if (bool.TryParse(Environment.GetEnvironmentVariable(envVar), out bool value))
        {
            Trace.WriteLine("Enabling experimental features via environment variables is deprecated. Please migrate to enabling/disabling features via bicepconfig.json");
            return value;
        }

        return null;
    }
}
