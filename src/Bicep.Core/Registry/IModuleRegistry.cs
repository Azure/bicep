// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using System;
using System.Collections.Generic;
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
        /// Gets the capabilities of this registry.
        /// </summary>
        RegistryCapabilities Capabilities { get; }

        /// <summary>
        /// Attempts to parse the specified unqualified reference or returns a failure builder.
        /// </summary>
        /// <param name="reference">The unqualified module reference</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails when null is returned</param>
        ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Returns true if the specified module is already cached in the local cache. 
        /// </summary>
        /// <param name="reference">The reference to the module.</param>
        bool IsModuleRestoreRequired(ModuleReference reference);

        /// <summary>
        /// Returns a URI to the entry point module.
        /// </summary>
        /// <param name="parentModuleUri">The parent URI</param>
        /// <param name="reference">The module reference</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails when null is returned</param>
        /// <returns></returns>
        Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Downloads the specified modules from the registry and caches them locally.
        /// Returns a mapping of module references to error builders for modules that failed to be downloaded.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references);

        /// <summary>
        /// Publishes the module at the specified path to the registry.
        /// </summary>
        /// <param name="moduleReference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        Task PublishModule(ModuleReference moduleReference, Stream compiled);
    }
}
