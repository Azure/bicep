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

        public string CacheRootDirectory => GetCacheRootDirectory(this.configuration.CacheRootDirectory);

        public bool SymbolicNameCodegenEnabled => this.configuration.ExperimentalFeaturesEnabled.SymbolicNameCodegen;

        public bool ExtensibilityEnabled => this.configuration.ExperimentalFeaturesEnabled.Extensibility;

        public bool ResourceTypedParamsAndOutputsEnabled => this.configuration.ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs;

        public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

        public bool SourceMappingEnabled => this.configuration.ExperimentalFeaturesEnabled.SourceMapping;

        public bool UserDefinedTypesEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedTypes;

        public bool UserDefinedFunctionsEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedFunctions;

        public bool PrettyPrintingEnabled => this.configuration.ExperimentalFeaturesEnabled.PrettyPrinting;

        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static TraceVerbosity TracingVerbosity => ReadEnumEnvvar("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

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
