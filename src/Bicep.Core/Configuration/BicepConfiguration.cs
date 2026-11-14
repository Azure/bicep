// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Bicep.Core.Configuration.LegacyModuleAliasMocks;
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

        public const string CompilerKey = "bicep";

        public const string BuiltInConfigurationResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        public static IBicepConfiguration BuiltIn => BuiltInLazy.Value;

        private static readonly Lazy<IBicepConfiguration> BuiltInLazy =
            new(() => Bind(BuiltInConfigurationElement));

        internal static readonly JsonElement BuiltInConfigurationElement = GetBuiltInConfigurationElement();

        private static JsonElement GetBuiltInConfigurationElement()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BuiltInConfigurationResourceName);
            if (stream is null)
            {
                throw new InvalidOperationException("Could not get manifest resource stream for built-in configuration.");
            }

            return JsonElementFactory.CreateElementFromStream(stream);
        }

        public BicepConfiguration(
            IBicepCloudConfiguration cloud,
            IBicepModuleAliasesConfiguration moduleAliases,
            IBicepModuleAliasesMockConfiguration moduleAliasesMock,
            IBicepExtensionsConfiguration extensions,
            IBicepImplicitExtensionsConfiguration implicitExtensions,
            IBicepAnalyzersConfiguration analyzers,
            IBicepFormattingConfiguration formatting,
            IBicepDocumentationConfiguration documentation,
            IBicepCompilerConfiguration compiler,
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
            Compiler = compiler;
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
            var cloud = BindSection(element, CloudKey, CloudConfiguration.Bind);
            var moduleAliases = BindSection(element, ModuleAliasesKey, x => ModuleAliasesConfiguration.Bind(x, configFileUri));
            var moduleAliasesMock = element.TryGetProperty(ModuleAliasesMockKey, out _)
                ? BindSection(element, ModuleAliasesMockKey, x => ModuleAliasesMockConfiguration.Bind(x, configFileUri))
                : ModuleAliasesMockConfiguration.Bind(JsonElementFactory.CreateElement(new ModuleAliasesMock()), configFileUri);
            var analyzers = BindSection(element, AnalyzersKey, x => x.ValueKind == JsonValueKind.Object
                ? new AnalyzersConfiguration(x)
                : throw new ConfigurationException($"The \"{AnalyzersKey}\" property has an invalid value."));
            var cacheRootDirectory = element.TryGetProperty(CacheRootDirectoryKey, out _)
                ? BindSection(element, CacheRootDirectoryKey, x => x.GetString())
                : null;
            var experimentalFeaturesWarning = element.TryGetProperty(ExperimentalFeaturesWarningKey, out _) &&
                BindSection(element, ExperimentalFeaturesWarningKey, x => x.GetBoolean());
            var experimentalFeaturesEnabled = BindSection(element, ExperimentalFeaturesEnabledKey, ExperimentalFeaturesEnabled.Bind);
            var formatting = BindSection(element, FormattingKey, FormattingConfiguration.Bind);
            var documentation = element.TryGetProperty(DocumentationKey, out _)
                ? BindSection(element, DocumentationKey, DocumentationConfiguration.Bind)
                : new DocumentationConfiguration(new());
            var compiler = element.TryGetProperty(CompilerKey, out _)
                ? BindSection(element, CompilerKey, CompilerConfiguration.Bind)
                : new CompilerConfiguration(new(), null);

            var extensions = BindSection(element, ExtensionsKey, ExtensionsConfiguration.Bind);
            var implicitExtensions = BindSection(element, ImplicitExtensionsKey, ImplicitExtensionsConfiguration.Bind);

            return new BicepConfiguration(
                cloud: cloud,
                moduleAliases: moduleAliases,
                moduleAliasesMock: moduleAliasesMock,
                extensions: extensions,
                implicitExtensions: implicitExtensions,
                analyzers: analyzers,
                formatting: formatting,
                documentation: documentation,
                compiler: compiler,
                experimentalFeaturesEnabled: experimentalFeaturesEnabled,
                cacheRootDirectory: cacheRootDirectory,
                experimentalFeaturesWarning: experimentalFeaturesWarning,
                configFileUri: configFileUri,
                diagnostics: null);
        }

        /// <summary>
        /// Binds the <paramref name="key"/> property of <paramref name="element"/>. A value of the wrong shape becomes a
        /// <see cref="ConfigurationException"/> that names the property, instead of a JSON exception about .NET types.
        /// </summary>
        private static T BindSection<T>(JsonElement element, string key, Func<JsonElement, T> bind)
        {
            try
            {
                return bind(element.GetProperty(key));
            }
            catch (JsonException exception)
            {
                // Deserialization errors carry the failing path within the section, e.g. "$.version".
                var path = exception.Path is { Length: > 1 } innerPath && innerPath.StartsWith("$.", StringComparison.Ordinal)
                    ? $"{key}.{innerPath[2..]}"
                    : key;

                throw new ConfigurationException($"The \"{path}\" property has an invalid value.");
            }
            catch (InvalidOperationException)
            {
                throw new ConfigurationException($"The \"{key}\" property has an invalid value.");
            }
        }

        public IBicepCloudConfiguration Cloud { get; }

        public IBicepModuleAliasesConfiguration ModuleAliases { get; }

        public IBicepModuleAliasesMockConfiguration ModuleAliasesMock { get; }

        public IBicepExtensionsConfiguration Extensions { get; }

        public IBicepImplicitExtensionsConfiguration ImplicitExtensions { get; }

        public IBicepAnalyzersConfiguration Analyzers { get; }

        public IBicepFormattingConfiguration Formatting { get; }

        public IBicepDocumentationConfiguration Documentation { get; }

        public IBicepCompilerConfiguration Compiler { get; }

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
