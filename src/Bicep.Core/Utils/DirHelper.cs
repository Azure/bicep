// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;

namespace Bicep.Core.Utils
{
    public static class DirHelper
    {
        /// <summary>
        /// Returns path inside TEMP/TMP that is controlled entirely by Bicep.
        /// </summary>
        public static string GetTempPath()
        {
            string profilePath = Path.Combine(Path.GetTempPath(), LanguageConstants.LanguageFileExtension); // bicep extension as a hidden folder name
            Directory.CreateDirectory(profilePath);
            return profilePath;
        }
    }
}
