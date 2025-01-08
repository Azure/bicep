// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Intermediate;
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

        public bool SymbolicNameCodegenEnabled => this.configuration.ExperimentalFeaturesEnabled.SymbolicNameCodegen;

        public bool ExtensibilityEnabled => this.configuration.ExperimentalFeaturesEnabled.Extensibility;

        public bool ExtendableParamFilesEnabled => this.configuration.ExperimentalFeaturesEnabled.ExtendableParamFiles;

        public bool ResourceTypedParamsAndOutputsEnabled => this.configuration.ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs;

        public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

        public bool SourceMappingEnabled => this.configuration.ExperimentalFeaturesEnabled.SourceMapping;

        public bool LegacyFormatterEnabled => configuration.ExperimentalFeaturesEnabled.LegacyFormatter;

        public bool TestFrameworkEnabled => this.configuration.ExperimentalFeaturesEnabled.TestFramework;

        public bool AssertsEnabled => configuration.ExperimentalFeaturesEnabled.Assertions;

        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static TraceVerbosity TracingVerbosity => ReadEnumEnvVar("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

        public bool OptionalModuleNamesEnabled => configuration.ExperimentalFeaturesEnabled.OptionalModuleNames;

        public bool LocalDeployEnabled => configuration.ExperimentalFeaturesEnabled.LocalDeploy;

        public bool ResourceDerivedTypesEnabled => configuration.ExperimentalFeaturesEnabled.ResourceDerivedTypes;

        public bool SecureOutputsEnabled => configuration.ExperimentalFeaturesEnabled.SecureOutputs;

        public bool ResourceInfoCodegenEnabled => this.configuration.ExperimentalFeaturesEnabled.ResourceInfoCodegen;

        public bool ExtensibilityV2EmittingEnabled => ReadBooleanEnvVar("BICEP_EXTENSIBILITY_V2_EMITTING_ENABLED", defaultValue: false);

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
            => bool.TryParse(Environment.GetEnvironmentVariable(envVar), out var value) ? value : defaultValue;

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
            this.fileExplorer.GetDirectory(IOUri.FromLocalFilePath(localPath));
    }
}
