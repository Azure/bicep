// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Configuration;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        private readonly RootConfiguration configuration;

        public FeatureProvider(RootConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CacheRootDirectory => GetCacheRootDirectory(configuration.CacheRootDirectory);

        public bool RegistryEnabled => true;

        public bool SymbolicNameCodegenEnabled => configuration.ExperimentalFeaturesEnabled.SymbolicNameCodegen ?? false;

        public bool ExtensibilityEnabled => configuration.ExperimentalFeaturesEnabled.Extensibility ?? false;

        public bool ResourceTypedParamsAndOutputsEnabled => configuration.ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs ?? false;

        public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

        public bool SourceMappingEnabled => configuration.ExperimentalFeaturesEnabled.SourceMapping ?? false;

        public bool ParamsFilesEnabled => configuration.ExperimentalFeaturesEnabled.ParamsFiles ?? false;

        public bool UserDefinedTypesEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedTypes ?? false;

        public bool UserDefinedFunctionsEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedFunctions ?? false;

        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static TraceVerbosity TracingVerbosity => ReadEnumEnvvar<TraceVerbosity>("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

        public bool DynamicTypeLoading => configuration.ExperimentalFeaturesEnabled.DynamicTypeLoading ?? false;

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
            => bool.TryParse(Environment.GetEnvironmentVariable(envVar), out var value) ? value : defaultValue;

        private static T ReadEnumEnvvar<T>(string envVar, T defaultValue) where T : struct
        {
            var str = Environment.GetEnvironmentVariable(envVar);
            return Enum.TryParse<T>(str, true, out var value) ? value : defaultValue;
        }

        private static string GetDefaultCachePath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(basePath, ".bicep");
        }

        private static string GetCacheRootDirectory(string? customPath)
        {
            if (string.IsNullOrWhiteSpace(customPath))
            {
                return GetDefaultCachePath();
            }

            return customPath;
        }
    }
}
