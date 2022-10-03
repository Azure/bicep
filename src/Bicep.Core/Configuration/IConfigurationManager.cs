// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Reflection;
using System.Text.Json;
using Bicep.Core.Json;

namespace Bicep.Core.Configuration
{
    public interface IConfigurationManager
    {
        public const string BuiltInConfigurationResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        /// <summary>
        /// Gets the configuration for the source file with the given URI.
        /// If no custom configuration is found, the built-in configuration is returned.
        /// </summary>
        /// <param name="sourceFileUri">The URI of the source file to get configuration for.</param>
        /// <returns>The configuration for the source file.</returns>
        RootConfiguration GetConfiguration(Uri sourceFileUri);

        /// <summary>
        /// Gets the built-in configuration.
        /// </summary>
        /// <returns>The built-in configuration.</returns>
        static RootConfiguration GetBuiltInConfiguration() => BuiltInConfigurationLazy.Value;

        public static IConfigurationManager WithStaticConfiguration(RootConfiguration configuration) 
            => new ConstantConfigurationManager(configuration);

        protected static readonly JsonElement BuiltInConfigurationElement = GetBuiltInConfigurationElement();

        private static readonly Lazy<RootConfiguration> BuiltInConfigurationLazy =
            new(() => RootConfiguration.Bind(BuiltInConfigurationElement));

        private static JsonElement GetBuiltInConfigurationElement()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BuiltInConfigurationResourceName);

            if (stream is null)
            {
                throw new InvalidOperationException("Could not get manifest resource stream for built-in configuration.");
            }

            return JsonElementFactory.CreateElement(stream);
        }

        private class ConstantConfigurationManager : IConfigurationManager
        {
            private readonly RootConfiguration configuration;

            internal ConstantConfigurationManager(RootConfiguration configuration)
            {
                this.configuration = configuration;
            }

            public RootConfiguration GetConfiguration(Uri sourceFileUri) => configuration;
        }
    }
}
