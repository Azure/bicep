// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Core.Configuration
{
    public class ConfigurationException : BicepException
    {
        public ConfigurationException(string message, ConfigurationPropertyPath propertyPath = default)
            : base(message)
        {
            PropertyPath = propertyPath;
        }

        /// <summary>
        /// The path of the property that caused the error, relative to the element being bound
        /// (for example ["version"] within the "bicep" section), or <see cref="ConfigurationPropertyPath.Root"/> if the element itself is invalid.
        /// </summary>
        public ConfigurationPropertyPath PropertyPath { get; }
    }
}
