// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    /// <summary>
    /// Concrete implementation of <see cref="IBicepConfiguration"/>.
    /// Holds the fully merged effective configuration for a source file,
    /// constructed from section interfaces to support inheritance and testability.
    /// </summary>
    public class BicepConfiguration : IBicepConfiguration
    {
        public const string CloudKey = "cloud";

        public const string ModuleAliasesKey = "moduleAliases";

        public const string ModuleAliasesMockKey = "moduleAliasesMock";

        public const string ExtensionsKey = "extensions";

        public const string ImplicitExtensionsKey = "implicitExtensions";

        public const string AnalyzersKey = "analyzers";

        public const string CacheRootDirectoryKey = "cacheRootDirectory";

        public const string ExperimentalFeaturesWarningKey = "experimentalFeaturesWarning";

        public const string ExperimentalFeaturesEnabledKey = "experimentalFeaturesEnabled";

        public const string FormattingKey = "formatting";

        public const string DocumentationKey = "documentation";

        public BicepConfiguration(
            IBicepCloudConfiguration cloud,
            IBicepModuleAliasesConfiguration moduleAliases,
            IBicepModuleAliasesMockConfiguration moduleAliasesMock,
            IBicepExtensionsConfiguration extensions,
            IBicepImplicitExtensionsConfiguration implicitExtensions,
            IBicepAnalyzersConfiguration analyzers,
            IBicepFormattingConfiguration formatting,
            IBicepDocumentationConfiguration documentation,
            ExperimentalFeaturesEnabled experimentalFeaturesEnabled,
            string? cacheRootDirectory,
            bool experimentalFeaturesWarning,
            IOUri? configFileUri,
            IEnumerable<IDiagnostic>? diagnostics)
        {
            Cloud = cloud;
            ModuleAliases = moduleAliases;
            ModuleAliasesMock = moduleAliasesMock;
            Extensions = extensions;
            ImplicitExtensions = implicitExtensions;
            Analyzers = analyzers;
            Formatting = formatting;
            Documentation = documentation;
            ExperimentalFeaturesEnabled = experimentalFeaturesEnabled;
            CacheRootDirectory = ExpandCacheRootDirectory(cacheRootDirectory);
            ExperimentalFeaturesWarning = experimentalFeaturesWarning;
            ConfigFileUri = configFileUri;
            Diagnostics = diagnostics?.ToImmutableArray() ?? [];
        }

        /// <summary>
        /// Binds a JSON element representing a fully merged Bicep configuration into a
        /// <see cref="BicepConfiguration"/> instance.
        /// </summary>
        public static BicepConfiguration Bind(JsonElement element, IOUri? configFileUri = null)
        {
            var cloud = CloudConfiguration.Bind(element.GetProperty(CloudKey));
            var moduleAliases = ModuleAliasesConfiguration.Bind(element.GetProperty(ModuleAliasesKey), configFileUri);
            var moduleAliasesMock = element.TryGetProperty(ModuleAliasesMockKey, out var mockElement)
                 ? ModuleAliasesMockConfiguration.Bind(mockElement, configFileUri)
                  : ModuleAliasesMockConfiguration.Bind(JsonElementFactory.CreateElement(new ModuleAliasesMock()), configFileUri);
            var analyzers = new AnalyzersConfiguration(element.GetProperty(AnalyzersKey));
            var cacheRootDirectory = element.TryGetProperty(CacheRootDirectoryKey, out var e) ? e.GetString() : default;
            var experimentalFeaturesWarning = element.TryGetProperty(ExperimentalFeaturesWarningKey, out var value) && value.GetBoolean();
            var experimentalFeaturesEnabled = ExperimentalFeaturesEnabled.Bind(element.GetProperty(ExperimentalFeaturesEnabledKey));
            var formatting = FormattingConfiguration.Bind(element.GetProperty(FormattingKey));
            var documentation = element.TryGetProperty(DocumentationKey, out var documentationElement)
                ? DocumentationConfiguration.Bind(documentationElement)
                : new DocumentationConfiguration(new());

            var extensions = ExtensionsConfiguration.Bind(element.GetProperty(ExtensionsKey));
            var implicitExtensions = ImplicitExtensionsConfiguration.Bind(element.GetProperty(ImplicitExtensionsKey));

            return new BicepConfiguration(
                cloud: cloud,
                moduleAliases: moduleAliases,
                moduleAliasesMock: moduleAliasesMock,
                extensions: extensions,
                implicitExtensions: implicitExtensions,
                analyzers: analyzers,
                formatting: formatting,
                documentation: documentation,
                experimentalFeaturesEnabled: experimentalFeaturesEnabled,
                cacheRootDirectory: cacheRootDirectory,
                experimentalFeaturesWarning: experimentalFeaturesWarning,
                configFileUri: configFileUri,
                diagnostics: null);
        }

        public IBicepCloudConfiguration Cloud { get; }

        public IBicepModuleAliasesConfiguration ModuleAliases { get; }

        public IBicepModuleAliasesMockConfiguration ModuleAliasesMock { get; }

        public IBicepExtensionsConfiguration Extensions { get; }

        public IBicepImplicitExtensionsConfiguration ImplicitExtensions { get; }

        public IBicepAnalyzersConfiguration Analyzers { get; }

        public IBicepFormattingConfiguration Formatting { get; }

        public IBicepDocumentationConfiguration Documentation { get; }

        public ExperimentalFeaturesEnabled ExperimentalFeaturesEnabled { get; }

        public string? CacheRootDirectory { get; }

        public bool ExperimentalFeaturesWarning { get; }

        public IOUri? ConfigFileUri { get; }

        public bool IsBuiltIn => ConfigFileUri is null;

        public ImmutableArray<IDiagnostic> Diagnostics { get; }

        public IEnumerable<IDiagnostic> GetDiagnostics() => Diagnostics;

        /// <summary>
        /// Expands a leading '~' in the cache root directory path to the user's home directory.
        /// </summary>
        private static string? ExpandCacheRootDirectory(string? cacheRootDirectory)
        {
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
