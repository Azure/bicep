// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Features
{
    public interface IFeatureProvider
    {
        string AssemblyVersion { get; }

        string CacheRootDirectory { get; }

        bool RegistryEnabled { get; }

        bool SymbolicNameCodegenEnabled { get; }

        bool ImportsEnabled { get; }

        bool ResourceTypedParamsAndOutputsEnabled { get; }

        bool SourceMappingEnabled { get; }

        bool ParamsFilesEnabled { get; }

        static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        static TraceVerbosity TracingVerbosity => ReadEnumEnvvar<TraceVerbosity>("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
            => bool.TryParse(Environment.GetEnvironmentVariable(envVar), out var value) ? value : defaultValue;

        private static T ReadEnumEnvvar<T>(string envVar, T defaultValue) where T : struct
        {
            var str = Environment.GetEnvironmentVariable(envVar);
            return Enum.TryParse<T>(str, true, out var value) ? value : defaultValue;
        }
    }
}
