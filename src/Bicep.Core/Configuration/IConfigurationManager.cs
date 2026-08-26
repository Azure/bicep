// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

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
        IBicepConfiguration GetConfiguration(IOUri sourceFileUri);

        /// <summary>
        /// Gets the built-in configuration.
        /// </summary>
        /// <returns>The built-in configuration.</returns>
        static IBicepConfiguration GetBuiltInConfiguration() => BuiltInConfigurationLazy.Value;

        public static IConfigurationManager WithStaticConfiguration(IBicepConfiguration configuration)
            => new ConstantConfigurationManager(configuration);

        static readonly JsonElement BuiltInConfigurationElement = GetBuiltInConfigurationElement();

        private static readonly Lazy<IBicepConfiguration> BuiltInConfigurationLazy =
            new(() => BicepConfiguration.Bind(BuiltInConfigurationElement));

        private static JsonElement GetBuiltInConfigurationElement()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BuiltInConfigurationResourceName);

            if (stream is null)
            {
                throw new InvalidOperationException("Could not get manifest resource stream for built-in configuration.");
            }

            return JsonElementFactory.CreateElementFromStream(stream);
        }

        private class ConstantConfigurationManager : IConfigurationManager
        {
            private readonly IBicepConfiguration configuration;

            internal ConstantConfigurationManager(IBicepConfiguration configuration)
            {
                this.configuration = configuration;
            }

            public IBicepConfiguration GetConfiguration(IOUri sourceFileUri) => configuration;
        }
    }
}
