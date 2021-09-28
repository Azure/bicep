// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Bicep.Core.Configuration
{
    public class DefaultBicepConfigHelper
    {
        private const string bicepConfigResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        public static Stream? GetManifestResourceStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(bicepConfigResourceName);
        }

        public static string GetDefaultBicepConfig()
        {
            var streamReader = new StreamReader(GetManifestResourceStream() ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            return streamReader.ReadToEnd();
        }
    }
}
