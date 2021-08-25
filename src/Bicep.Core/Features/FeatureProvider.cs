// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        public string CacheRootPath
        {
            get
            {
                var customPath = Environment.GetEnvironmentVariable("BICEP_CACHE");
                if(string.IsNullOrWhiteSpace(customPath))
                {
                    return GetDefaultCachePath();
                }

                return customPath;
            }
        }

        public bool RegistryEnabled => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_REGISTRY_ENABLED_EXPERIMENTAL"), out var enabled) ? enabled : false;

        private static string GetDefaultCachePath()
        {
            // TODO: Will NOT work if user profile is not loaded on Windows! (Az functions load executables like that)
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(basePath, ".bicep", "artifacts");
        }
    }
}
