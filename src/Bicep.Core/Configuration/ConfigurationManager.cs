// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        public const string BuiltInConfigurationResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        private static readonly IConfiguration builtInRawConfiguration = BuildBuiltInRawConfiguration();

        private static readonly Lazy<RootConfiguration> builtInConfigurationLazy =
            new(() => RootConfiguration.Bind(builtInRawConfiguration, BuiltInConfigurationResourceName));

        private static readonly Lazy<RootConfiguration> builtInConfigurationWithAnalyzersDisabledLazy =
            new(() => RootConfiguration.Bind(builtInRawConfiguration, BuiltInConfigurationResourceName, disableAnalyzers: true));

        private readonly IFileSystem fileSystem;

        public ConfigurationManager(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public RootConfiguration GetBuiltInConfiguration(bool disableAnalyzers = false) => disableAnalyzers
            ? builtInConfigurationWithAnalyzersDisabledLazy.Value
            : builtInConfigurationLazy.Value;

        public RootConfiguration GetConfiguration(Uri sourceFileUri)
        {
            var customConfigurationPath = DiscoverConfigurationFile(fileSystem.Path.GetDirectoryName(sourceFileUri.LocalPath));

            if (customConfigurationPath is not null)
            {
                IConfiguration? customConfiguration;

                try
                {
                    using var stream = fileSystem.FileStream.Create(customConfigurationPath, FileMode.Open, FileAccess.Read);
                    var builder = new ConfigurationBuilder()
                        .AddInMemoryCollection(builtInRawConfiguration.AsEnumerable())
                        .AddJsonStream(stream);

                    customConfiguration = builder.Build();
                }
                catch (JsonException exception)
                {
                    throw new ConfigurationException($"Failed to parse the Bicep configuration file \"{customConfigurationPath}\" as valid JSON: \"{exception.Message}\".");
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
                {
                    throw new ConfigurationException($"Could not load the Bicep configuration file \"{customConfigurationPath}\": \"{exception.Message}\".");
                }

                return RootConfiguration.Bind(customConfiguration, customConfigurationPath);
            }

            return GetBuiltInConfiguration();
        }

        private static IConfiguration BuildBuiltInRawConfiguration()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BuiltInConfigurationResourceName);
            Debug.Assert(stream is not null, "Default configuration file should exist as embedded resource.");

            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(stream);

            return builder.Build();
        }

        private string? DiscoverConfigurationFile(string? currentDirectory)
        {
            try
            {
                while (!string.IsNullOrEmpty(currentDirectory))
                {
                    var configurationPath = this.fileSystem.Path.Combine(currentDirectory, LanguageConstants.BicepConfigurationFileName);

                    if (this.fileSystem.File.Exists(configurationPath))
                    {
                        return configurationPath;
                    }

                    currentDirectory = this.fileSystem.Directory.GetParent(currentDirectory)?.FullName;
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
            {
                throw new ConfigurationException($"Error while discovering Bicep configuration file: \"{exception.Message}\".");
            }

            return null;
        }
    }
}
