// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Core.Configuration
{
    public class ConfigurationException : BicepException
    {
        public ConfigurationException(string message)
            : base(message)
        {
        }
    }
}
