// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
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
