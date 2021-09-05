// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;

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

        public bool RegistryEnabled => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_REGISTRY_ENABLED_EXPERIMENTAL"), out var enabled) ? enabled : false;

        public bool SymbolicNameCodegenEnabled => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_SYMBOLIC_NAME_CODEGEN_EXPERIMENTAL"), out var enabled) ? enabled : false;

        private static string GetDefaultCachePath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(basePath, ".bicep", "artifacts");
        }
    }
}
