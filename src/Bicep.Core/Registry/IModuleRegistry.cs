// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// An implementation of a Bicep module registry.
    /// </summary>
    public interface IModuleRegistry
    {
        /// <summary>
        /// Gets the scheme used by this registry in module references.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets the capabilities of this registry for the specified module reference.
        /// </summary>
        /// <param name="reference">The module reference</param>
        RegistryCapabilities GetCapabilities(ModuleReference reference);

        /// <summary>
        /// Attempts to parse the specified unqualified reference or returns a failure builder.
        /// </summary>
        /// <param name="aliasName">The alias name</param>
        /// <param name="reference">The unqualified module reference</param>
        /// <param name="moduleReference">set to the parsed module reference if parsing succeeds</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails</param>
        bool TryParseModuleReference(string? aliasName, string reference, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Returns true if the specified module is already cached in the local cache.
        /// </summary>
        /// <param name="reference">The reference to the module.</param>
        bool IsModuleRestoreRequired(ModuleReference reference);

        /// <summary>
        /// Returns a URI to the entry point module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="localUri">set to the local entry module entry point URI if parsing succeeds</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails</param>
        /// <returns></returns>
        bool TryGetLocalModuleEntryPointUri(ModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Downloads the specified modules from the registry and caches them locally.
        /// Returns a mapping of module references to error builders for modules that failed to be downloaded.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references);

       /// <summary>
        /// Invalidate the specified cached modules from the registry.
        /// Returns a mapping of module references to error builders for modules that failed to be invalidated.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<ModuleReference> references);

        /// <summary>
        /// Publishes the module at the specified path to the registry.
        /// </summary>
        /// <param name="moduleReference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        Task PublishModule(ModuleReference moduleReference, Stream compiled);
    }
}
