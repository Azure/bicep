// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Configuration
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the built-in configuration.
        /// </summary>
        /// <param name="disableAnalyzers">Indicates whether the analyzer settings should be disabled.</param>
        /// <returns>The built-in configuration.</returns>
        RootConfiguration GetBuiltInConfiguration(bool disableAnalyzers = false);

        /// <summary>
        /// Gets the configuration for the source file with the given URI.
        /// If no custom configuration is found, the built-in configuration is returned.
        /// </summary>
        /// <param name="sourceFileUri">The URI of the source file to get configuration for.</param>
        /// <returns>The configuration for the source file.</returns>
        RootConfiguration GetConfiguration(Uri sourceFileUri);
    }
}
