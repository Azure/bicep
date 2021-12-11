// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core;
using Bicep.Core.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class ConfigurationHelper
    {
        public static bool IsBicepConfigFile(DocumentUri documentUri)
        {
            try
            {
                return string.Equals(Path.GetFileName(documentUri.Path), LanguageConstants.BicepConfigurationFileName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Encountered issue while trying to get the file name: {e.Message}");
                return false;
            }
        }

        public static bool TryGetConfiguration(IConfigurationManager configurationManager, DocumentUri documentUri, [NotNullWhen(true)] out RootConfiguration? rootConfiguration)
        {
            try
            {
                rootConfiguration = configurationManager.GetConfiguration(documentUri.ToUri());
                return true;
            }
            catch(Exception e)
            {
                rootConfiguration = null;
                Trace.WriteLine($"Encountered issue while getting configuration: {e.Message}");
                return false;
            }
        }
    }
}
