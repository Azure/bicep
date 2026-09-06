// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Text;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    public static class BicepConfigurationExtensions
    {
        /// <summary>
        /// Produces a copy of the configuration with the specified sections/values overridden.
        /// Any argument left null retains the value from <paramref name="configuration"/>.
        /// </summary>
        public static BicepConfiguration With(
            this IBicepConfiguration configuration,
            IBicepCloudConfiguration? cloud = null,
            IBicepModuleAliasesConfiguration? moduleAliases = null,
            IBicepModuleAliasesMockConfiguration? moduleAliasesMock = null,
            IBicepExtensionsConfiguration? extensions = null,
            IBicepImplicitExtensionsConfiguration? implicitExtensions = null,
            IBicepAnalyzersConfiguration? analyzers = null,
            string? cacheRootDirectory = null,
            bool? experimentalFeaturesWarning = null,
            ExperimentalFeaturesEnabled? experimentalFeaturesEnabled = null,
            IBicepFormattingConfiguration? formatting = null,
            IBicepDocumentationConfiguration? documentation = null,
            IOUri? configFileIdentifier = null,
            IEnumerable<IDiagnostic>? diagnostics = null)
        {
            return new BicepConfiguration(
                cloud: cloud ?? configuration.Cloud,
                moduleAliases: moduleAliases ?? configuration.ModuleAliases,
                moduleAliasesMock: moduleAliasesMock ?? configuration.ModuleAliasesMock,
                extensions: extensions ?? configuration.Extensions,
                implicitExtensions: implicitExtensions ?? configuration.ImplicitExtensions,
                analyzers: analyzers ?? configuration.Analyzers,
                formatting: formatting ?? configuration.Formatting,
                documentation: documentation ?? configuration.Documentation,
                experimentalFeaturesEnabled: experimentalFeaturesEnabled ?? configuration.ExperimentalFeaturesEnabled,
                cacheRootDirectory: cacheRootDirectory ?? configuration.CacheRootDirectory,
                experimentalFeaturesWarning: experimentalFeaturesWarning ?? configuration.ExperimentalFeaturesWarning,
                configFileUri: configFileIdentifier ?? configuration.ConfigFileUri,
                diagnostics: diagnostics ?? configuration.GetDiagnostics());
        }

        /// <summary>
        /// Serializes the effective configuration to indented UTF-8 JSON.
        /// </summary>
        public static string ToUtf8Json(this IBicepConfiguration configuration)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter, new() { Indented = true }))
            {
                writer.WriteStartObject();

                writer.WritePropertyName(BicepConfiguration.CloudKey);
                ((IWritableConfigurationSection)configuration.Cloud).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.ModuleAliasesKey);
                ((IWritableConfigurationSection)configuration.ModuleAliases).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.ModuleAliasesMockKey);
                ((IWritableConfigurationSection)configuration.ModuleAliasesMock).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.ExtensionsKey);
                ((IWritableConfigurationSection)configuration.Extensions).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.ImplicitExtensionsKey);
                ((IWritableConfigurationSection)configuration.ImplicitExtensions).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.AnalyzersKey);
                ((IWritableConfigurationSection)configuration.Analyzers).WriteTo(writer);

                if (configuration.CacheRootDirectory is string cacheRootDir)
                {
                    writer.WriteString(BicepConfiguration.CacheRootDirectoryKey, cacheRootDir);
                }

                writer.WriteBoolean(BicepConfiguration.ExperimentalFeaturesWarningKey, configuration.ExperimentalFeaturesWarning);

                writer.WritePropertyName(BicepConfiguration.ExperimentalFeaturesEnabledKey);
                configuration.ExperimentalFeaturesEnabled.WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.FormattingKey);
                ((IWritableConfigurationSection)configuration.Formatting).WriteTo(writer);

                writer.WritePropertyName(BicepConfiguration.DocumentationKey);
                ((IWritableConfigurationSection)configuration.Documentation).WriteTo(writer);

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }
    }
}
