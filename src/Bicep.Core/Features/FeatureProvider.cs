// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        private readonly RootConfiguration configuration;

        private readonly IFileExplorer fileExplorer;

        public FeatureProvider(RootConfiguration configuration, IFileExplorer fileExplorer)
        {
            this.configuration = configuration;
            this.fileExplorer = fileExplorer;
        }

        public IDirectoryHandle CacheRootDirectory => GetCacheRootDirectory(this.configuration.CacheRootDirectory);

        public bool OciEnabled => this.configuration.ExperimentalFeaturesEnabled.OciEnabled || ReadBooleanEnvVar("BICEP_EXPERIMENTAL_OCI", defaultValue: false);

        public bool SymbolicNameCodegenEnabled => this.configuration.ExperimentalFeaturesEnabled.SymbolicNameCodegen;

        public bool ExtendableParamFilesEnabled => this.configuration.ExperimentalFeaturesEnabled.ExtendableParamFiles;

        public bool ResourceTypedParamsAndOutputsEnabled => this.configuration.ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs;

        public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

        public bool SourceMappingEnabled => this.configuration.ExperimentalFeaturesEnabled.SourceMapping;

        public bool LegacyFormatterEnabled => configuration.ExperimentalFeaturesEnabled.LegacyFormatter;

        public bool TestFrameworkEnabled => this.configuration.ExperimentalFeaturesEnabled.TestFramework;

        public bool AssertsEnabled => configuration.ExperimentalFeaturesEnabled.Assertions;

        public static readonly bool TracingEnabled = ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static readonly TraceVerbosity TracingVerbosity = ReadEnumEnvVar("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

        public static bool HasTracingVerbosity(TraceVerbosity verbosity) => TracingVerbosity >= verbosity;

        public bool WaitAndRetryEnabled => configuration.ExperimentalFeaturesEnabled.WaitAndRetry;

        public bool LocalDeployEnabled => configuration.ExperimentalFeaturesEnabled.LocalDeploy;

        public bool ResourceInfoCodegenEnabled => this.configuration.ExperimentalFeaturesEnabled.ResourceInfoCodegen;

        public bool ModuleExtensionConfigsEnabled => configuration.ExperimentalFeaturesEnabled.ModuleExtensionConfigs;

        public bool DesiredStateConfigurationEnabled => configuration.ExperimentalFeaturesEnabled.DesiredStateConfiguration;

        public bool UserDefinedConstraintsEnabled => configuration.ExperimentalFeaturesEnabled.UserDefinedConstraints;

        public bool DeployCommandsEnabled => configuration.ExperimentalFeaturesEnabled.DeployCommands;

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(envVar);
            if (value is null)
            {
                return defaultValue;
            }

            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            {
                if (intValue == 1)
                {
                    return true;
                }

                if (intValue == 0)
                {
                    return false;
                }
            }

            return defaultValue;
        }

        public static string ReadEnvVar(string envVar, string defaultValue)
            => Environment.GetEnvironmentVariable(envVar) ?? defaultValue;

        private static T ReadEnumEnvVar<T>(string envVar, T defaultValue) where T : struct
        {
            var str = Environment.GetEnvironmentVariable(envVar);
            return Enum.TryParse<T>(str, true, out var value) ? value : defaultValue;
        }

        private IDirectoryHandle GetCacheRootDirectory(string? customPath) =>
            this.GetCacheRootDirectoryFromLocalPath(string.IsNullOrWhiteSpace(customPath)
                ? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.bicep"
                : customPath);

        private IDirectoryHandle GetCacheRootDirectoryFromLocalPath(string localPath) =>
            this.fileExplorer.GetDirectory(IOUri.FromFilePath(localPath));
    }
}
