// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        private Lazy<string> cacheRootDirectoryLazy = new(() => GetCacheRootDirectory(Environment.GetEnvironmentVariable("BICEP_CACHE_DIRECTORY")), LazyThreadSafetyMode.PublicationOnly);
        public string CacheRootDirectory => cacheRootDirectoryLazy.Value;

        public bool RegistryEnabled => true;

        private Lazy<bool> symbolicNameCodegenEnabledLazy = new(() => ReadBooleanEnvVar("BICEP_SYMBOLIC_NAME_CODEGEN_EXPERIMENTAL", defaultValue: false), LazyThreadSafetyMode.PublicationOnly);
        public bool SymbolicNameCodegenEnabled => symbolicNameCodegenEnabledLazy.Value;

        private Lazy<bool> importsEnabledLazy = new(() => ReadBooleanEnvVar("BICEP_IMPORTS_ENABLED_EXPERIMENTAL", defaultValue: false), LazyThreadSafetyMode.PublicationOnly);
        public bool ImportsEnabled => importsEnabledLazy.Value;

        private Lazy<bool> resourceTypedParamsAndOutputsEnabledLazy = new(() => ReadBooleanEnvVar("BICEP_RESOURCE_TYPED_PARAMS_AND_OUTPUTS_EXPERIMENTAL", defaultValue: false), LazyThreadSafetyMode.PublicationOnly);

        public bool ResourceTypedParamsAndOutputsEnabled => resourceTypedParamsAndOutputsEnabledLazy.Value;

        public string AssemblyVersion => ThisAssembly.AssemblyFileVersion;

        public bool SourceMappingEnabled => ReadBooleanEnvVar("BICEP_SOURCEMAPPING_ENABLED", defaultValue: false);

        private Lazy<bool> paramsFilesEnabledLazy = new(() => ReadBooleanEnvVar("BICEP_PARAMS_FILES_ENABLED", defaultValue: false), LazyThreadSafetyMode.PublicationOnly);
        public bool ParamsFilesEnabled => paramsFilesEnabledLazy.Value;
        
        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        public static TraceVerbosity TracingVerbosity => ReadEnumEnvvar<TraceVerbosity>("BICEP_TRACING_VERBOSITY", TraceVerbosity.Basic);

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
