// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string configurationPath, string error)
            : base($"Could not load the bicep configuration file \"{configurationPath}\". {error}")
        {
        }
    }
}
