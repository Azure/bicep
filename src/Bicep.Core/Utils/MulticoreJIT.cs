// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;

namespace Bicep.Core.Utils
{
    public static class MulticoreJIT
    {
        public static string GetMulticoreJITPath()
        {
            string profilePath = Path.Combine(Path.GetTempPath(), LanguageConstants.LanguageFileExtension); // bicep extension as a hidden folder name
            Directory.CreateDirectory(profilePath);
            return profilePath;
        }
    }
}
