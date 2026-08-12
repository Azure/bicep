// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    /// <summary>
    /// Represents the effective Bicep configuration for a source file.
    /// This is the result of merging all layers in a configuration chain
    /// (built-in defaults, base configs, and the leaf config).
    /// </summary>
    public interface IBicepConfiguration
    {
        IBicepCloudConfiguration Cloud { get; }

        IBicepModuleAliasesConfiguration ModuleAliases { get; }

        IBicepModuleAliasesMockConfiguration ModuleAliasesMock { get; }

        IBicepExtensionsConfiguration Extensions { get; }

        IBicepImplicitExtensionsConfiguration ImplicitExtensions { get; }

        IBicepAnalyzersConfiguration Analyzers { get; }

        IBicepFormattingConfiguration Formatting { get; }

        ExperimentalFeaturesEnabled ExperimentalFeaturesEnabled { get; }

        string? CacheRootDirectory { get; }

        bool ExperimentalFeaturesWarning { get; }

        /// <summary>
        /// The URI of the leaf bicepconfig.json file that was discovered for this configuration.
        /// Null when no user-defined config file was found and built-in defaults are in effect.
        /// </summary>
        IOUri? ConfigFileUri { get; }

        /// <summary>
        /// True when no user-defined bicepconfig.json was found and built-in defaults are in effect.
        /// </summary>
        bool IsBuiltIn { get; }

        /// <summary>
        /// Returns all diagnostics produced while loading this configuration.
        /// For an inherited chain, this includes diagnostics from every file in the chain.
        /// Each diagnostic's message already includes the source config file URI so callers
        /// can report exactly which file caused the problem.
        /// </summary>
        IEnumerable<IDiagnostic> GetDiagnostics();
    }
}
