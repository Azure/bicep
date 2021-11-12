// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Security;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        public const string BuiltInConfigurationResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        private static readonly JsonElement BuiltInConfigurationElement = GetBuildInConfigurationElement();

        private static readonly Lazy<RootConfiguration> BuiltInConfigurationLazy =
            new(() => RootConfiguration.Bind(BuiltInConfigurationElement));

        private static readonly Lazy<RootConfiguration> BuiltInConfigurationWithAnalyzersDisabledLazy =
            new(() => RootConfiguration.Bind(BuiltInConfigurationElement, disableAnalyzers: true));

        private readonly IFileSystem fileSystem;

        public ConfigurationManager(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public RootConfiguration GetBuiltInConfiguration(bool disableAnalyzers = false) => disableAnalyzers
            ? BuiltInConfigurationWithAnalyzersDisabledLazy.Value
            : BuiltInConfigurationLazy.Value;

        public RootConfiguration GetConfiguration(Uri sourceFileUri)
        {
            var configurationPath = DiscoverConfigurationFile(fileSystem.Path.GetDirectoryName(sourceFileUri.LocalPath));

            if (configurationPath is not null)
            {
                try
                {
                    using var stream = fileSystem.FileStream.Create(configurationPath, FileMode.Open, FileAccess.Read);
                    var element = BuiltInConfigurationElement.Merge(JsonElementFactory.CreateElement(stream));

                    return RootConfiguration.Bind(element, configurationPath);
                }
                catch (JsonException exception)
                {
                    throw new ConfigurationException($"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: \"{exception.Message}\".");
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
                {
                    throw new ConfigurationException($"Could not load the Bicep configuration file \"{configurationPath}\": \"{exception.Message}\".");
                }
            }

            return GetBuiltInConfiguration();
        }

        private static JsonElement GetBuildInConfigurationElement()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BuiltInConfigurationResourceName);

            if (stream is null)
            {
                throw new InvalidOperationException("Could not get manifest resource stream for build-in configuration.");
            }

            return JsonElementFactory.CreateElement(stream);
        }

        private string? DiscoverConfigurationFile(string? currentDirectory)
        {
            while (!string.IsNullOrEmpty(currentDirectory))
            {
                var configurationPath = this.fileSystem.Path.Combine(currentDirectory, LanguageConstants.BicepConfigurationFileName);

                if (this.fileSystem.File.Exists(configurationPath))
                {
                    return configurationPath;
                }

                try
                {
                    // Catching Directory.GetParent alone because it is the only one that throws IO related exceptions.
                    // Path.Combine only throws ArgumentNullException which indicates a bug in our code.
                    // File.Exists will not throw exceptions regardless the existence of path or if the user has permissions to read the file.
                    currentDirectory = this.fileSystem.Directory.GetParent(currentDirectory)?.FullName;
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
                {
                    // TODO: add telemetry here so that users can understand if there's an issue finding Bicep config.
                    // The exception could happen in senarios where users may not have read permission on the parent folder.
                    // We should not throw ConfigurationException in such cases since it will block compilation.
                    return null;
                }
            }

            return null;
        }
    }
}
