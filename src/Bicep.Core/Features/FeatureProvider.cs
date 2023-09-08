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

        public bool UserDefinedFunctionsEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedFunctions;

        public bool PrettyPrintingEnabled => this.configuration.ExperimentalFeaturesEnabled.PrettyPrinting;

        public bool TestFrameworkEnabled => this.configuration.ExperimentalFeaturesEnabled.TestFramework;

        public bool AssertsEnabled => configuration.ExperimentalFeaturesEnabled.Assertions;

        public bool CompileTimeImportsEnabled => configuration.ExperimentalFeaturesEnabled.CompileTimeImports;

        public bool MicrosoftGraphPreviewEnabled => this.configuration.ExperimentalFeaturesEnabled.MicrosoftGraphPreview;

        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static TraceVerbosity TracingVerbosity => ReadEnumEnvVar("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

        public bool DynamicTypeLoadingEnabled => configuration.ExperimentalFeaturesEnabled.DynamicTypeLoading;

        public bool PublishSourceEnabled => configuration.ExperimentalFeaturesEnabled.PublishSource;

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
            => bool.TryParse(Environment.GetEnvironmentVariable(envVar), out var value) ? value : defaultValue;

        public static string ReadEnvVar(string envVar, string defaultValue)
            => Environment.GetEnvironmentVariable(envVar) ?? defaultValue;

        private static T ReadEnumEnvVar<T>(string envVar, T defaultValue) where T : struct
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
