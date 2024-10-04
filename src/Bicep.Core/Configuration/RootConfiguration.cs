// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        public const string CloudKey = "cloud";

        public const string ModuleAliasesKey = "moduleAliases";

        public const string ExtensionsKey = "extensions";

        public const string ImplicitExtensionsKey = "implicitExtensions";

        public const string AnalyzersKey = "analyzers";

        public const string CacheRootDirectoryKey = "cacheRootDirectory";

        public const string ExperimentalFeaturesEnabledKey = "experimentalFeaturesEnabled";

        public const string FormattingKey = "formatting";

        public const string BicepKey = "bicep";

        public RootConfiguration(
            CloudConfiguration cloud,
            ModuleAliasesConfiguration moduleAliases,
            ExtensionsConfiguration extensions,
            ImplicitExtensionsConfiguration implicitExtensions,
            AnalyzersConfiguration analyzers,
            string? cacheRootDirectory,
            ExperimentalFeaturesEnabled experimentalFeaturesEnabled,
            FormattingConfiguration formatting,
            BicepConfigurationSection bicep,
            Uri? configFileUri,
            IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.Extensions = extensions;
            this.ImplicitExtensions = implicitExtensions;
            this.Analyzers = analyzers;
            this.CacheRootDirectory = ExpandCacheRootDirectory(cacheRootDirectory);
            this.ExperimentalFeaturesEnabled = experimentalFeaturesEnabled;
            this.Formatting = formatting;
            this.Bicep = bicep;
            this.ConfigFileUri = configFileUri;
            this.DiagnosticBuilders = diagnosticBuilders?.ToImmutableArray() ?? [];
        }

        public static RootConfiguration Bind(JsonElement element, Uri? configFileUri = null, IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders = null)
        {
            var cloud = CloudConfiguration.Bind(element.GetProperty(CloudKey));
            var moduleAliases = ModuleAliasesConfiguration.Bind(element.GetProperty(ModuleAliasesKey), configFileUri);
            var analyzers = new AnalyzersConfiguration(element.GetProperty(AnalyzersKey));
            var cacheRootDirectory = element.TryGetProperty(CacheRootDirectoryKey, out var e) ? e.GetString() : default;
            var experimentalFeaturesEnabled = ExperimentalFeaturesEnabled.Bind(element.GetProperty(ExperimentalFeaturesEnabledKey));
            var formatting = FormattingConfiguration.Bind(element.GetProperty(FormattingKey));
            var bicep = BicepConfigurationSection.Bind(element.GetProperty(BicepKey));

            var extensions = ExtensionsConfiguration.Bind(element.GetProperty(ExtensionsKey));
            var implicitExtensions = ImplicitExtensionsConfiguration.Bind(element.GetProperty(ImplicitExtensionsKey));

            return new(cloud, moduleAliases, extensions, implicitExtensions, analyzers, cacheRootDirectory, experimentalFeaturesEnabled, formatting, bicep, configFileUri, diagnosticBuilders);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public ExtensionsConfiguration Extensions { get; }

        public ImplicitExtensionsConfiguration ImplicitExtensions { get; }

        public AnalyzersConfiguration Analyzers { get; }

        public string? CacheRootDirectory { get; }

        public ExperimentalFeaturesEnabled ExperimentalFeaturesEnabled { get; }

        public FormattingConfiguration Formatting { get; }

        public BicepConfigurationSection Bicep { get; }

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

                writer.WritePropertyName(ExtensionsKey);
                this.Extensions.WriteTo(writer);

                writer.WritePropertyName(ImplicitExtensionsKey);
                this.ImplicitExtensions.WriteTo(writer);

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

                writer.WritePropertyName(BicepKey);
                this.Bicep.WriteTo(writer);

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
