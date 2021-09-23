// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        public string CacheRootDirectory
        {
            get
            {
                var customPath = Environment.GetEnvironmentVariable("BICEP_CACHE_DIRECTORY");
                if(string.IsNullOrWhiteSpace(customPath))
                {
                    return GetDefaultCachePath();
                }

                return customPath;
            }
        }

        public bool RegistryEnabled => ReadBooleanEnvVar("BICEP_REGISTRY_ENABLED_EXPERIMENTAL", defaultValue: false);

        public bool SymbolicNameCodegenEnabled => ReadBooleanEnvVar("BICEP_SYMBOLIC_NAME_CODEGEN_EXPERIMENTAL", defaultValue: false);

        private Lazy<bool> importsEnabledLazy = new(() => ReadBooleanEnvVar("BICEP_IMPORTS_ENABLED_EXPERIMENTAL", defaultValue: false), LazyThreadSafetyMode.PublicationOnly);
        public bool ImportsEnabled => importsEnabledLazy.Value;

        public static bool TracingEnabled => ReadBooleanEnvVar("BICEP_TRACING_ENABLED", defaultValue: false);

        private static bool ReadBooleanEnvVar(string envVar, bool defaultValue)
            => bool.TryParse(Environment.GetEnvironmentVariable(envVar), out var value) ? value : defaultValue;

        private static string GetDefaultCachePath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(basePath, ".bicep");
        }
    }
}
