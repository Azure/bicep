// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Core.Configuration
{
    public class ConfigurationException(string message) : BicepException(message)
    {
    }
}
