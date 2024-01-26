// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        private const string CloudKey = "cloud";

        private const string ModuleAliasesKey = "moduleAliases";

        private const string ProviderAliasesKey = "providerAliases";

        private const string ProvidersConfigurationKey = "providers";

        private const string AnalyzersKey = "analyzers";

        private const string CacheRootDirectoryKey = "cacheRootDirectory";

        private const string ExperimentalFeaturesEnabledKey = "experimentalFeaturesEnabled";

        private const string FormattingKey = "formatting";

        public RootConfiguration(
            CloudConfiguration cloud,
            ModuleAliasesConfiguration moduleAliases,
            ProviderAliasesConfiguration providerAliases,
            ProvidersConfiguration providersConfig,
            AnalyzersConfiguration analyzers,
            string? cacheRootDirectory,
            ExperimentalFeaturesEnabled experimentalFeaturesEnabled,
            FormattingConfiguration formatting,
            Uri? configFileUri,
            IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.ProviderAliases = providerAliases;
            this.ProvidersConfig = providersConfig;
            this.Analyzers = analyzers;
            this.CacheRootDirectory = ExpandCacheRootDirectory(cacheRootDirectory);
            this.ExperimentalFeaturesEnabled = experimentalFeaturesEnabled;
            this.Formatting = formatting;
            this.ConfigFileUri = configFileUri;
            this.DiagnosticBuilders = diagnosticBuilders?.ToImmutableArray() ?? ImmutableArray<DiagnosticBuilder.DiagnosticBuilderDelegate>.Empty;
        }

        public static RootConfiguration Bind(JsonElement element, Uri? configFileUri = null, IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders = null)
        {
            var cloud = CloudConfiguration.Bind(element.GetProperty(CloudKey));
            var moduleAliases = ModuleAliasesConfiguration.Bind(element.GetProperty(ModuleAliasesKey), configFileUri);
            var providerAliases = ProviderAliasesConfiguration.Bind(element.GetProperty(ProviderAliasesKey), configFileUri);
            var providersConfig = ProvidersConfiguration.Bind(element.GetProperty(ProvidersConfigurationKey), configFileUri);
            var analyzers = new AnalyzersConfiguration(element.GetProperty(AnalyzersKey));
            var cacheRootDirectory = element.TryGetProperty(CacheRootDirectoryKey, out var e) ? e.GetString() : default;
            var experimentalFeaturesEnabled = ExperimentalFeaturesEnabled.Bind(element.GetProperty(ExperimentalFeaturesEnabledKey));
            var formatting = FormattingConfiguration.Bind(element.GetProperty(FormattingKey));

            return new(
                cloud,
                moduleAliases,
                providerAliases,
                providersConfig,
                analyzers,
                cacheRootDirectory,
                experimentalFeaturesEnabled,
                formatting,
                configFileUri,
                diagnosticBuilders);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public ProviderAliasesConfiguration ProviderAliases { get; }

        public ProvidersConfiguration ProvidersConfig { get; }
        
        public AnalyzersConfiguration Analyzers { get; }

        public string? CacheRootDirectory { get; }

        public ExperimentalFeaturesEnabled ExperimentalFeaturesEnabled { get; }

        public FormattingConfiguration Formatting { get; }

        public Uri? ConfigFileUri { get; }

        public ImmutableArray<DiagnosticBuilder.DiagnosticBuilderDelegate> DiagnosticBuilders { get; }

        public bool IsBuiltIn => ConfigFileUri is null;

        public string ToUtf8Json()
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter, new() { Indented = true }))
            {
                writer.WriteStartObject();

                writer.WritePropertyName(CloudKey);
                this.Cloud.WriteTo(writer);

                writer.WritePropertyName(ModuleAliasesKey);
                this.ModuleAliases.WriteTo(writer);

                writer.WritePropertyName(ProviderAliasesKey);
                this.ProviderAliases.WriteTo(writer);

                writer.WritePropertyName(AnalyzersKey);
                this.Analyzers.WriteTo(writer);

                if (CacheRootDirectory is string cacheRootDir)
                {
                    writer.WriteString(CacheRootDirectoryKey, cacheRootDir);
                }

                writer.WritePropertyName(ExperimentalFeaturesEnabledKey);
                this.ExperimentalFeaturesEnabled.WriteTo(writer);

                writer.WritePropertyName(FormattingKey);
                this.Formatting.WriteTo(writer);

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        private static string? ExpandCacheRootDirectory(string? cacheRootDirectory)
        {
            /*
             Note: the method is a simple workaround for https://github.com/Azure/bicep/issues/10935. To reduce
             complexity, it does not handle all cross-platform edge cases, such as "~username" on Unix based systems.
             In the future, we may want to read CacheRootDirectory from a environment variable instead whose value
             must be a full path.
            */
            if (string.IsNullOrEmpty(cacheRootDirectory) || cacheRootDirectory[0] != '~')
            {
                return cacheRootDirectory;
            }

            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (cacheRootDirectory.Length == 1)
            {
                return homeDirectory;
            }

            if (cacheRootDirectory[1] is '/' or '\\')
            {
                return $"{homeDirectory}{cacheRootDirectory[1..]}";
            }

            return cacheRootDirectory;
        }
    }
}
