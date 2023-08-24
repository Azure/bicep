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
    /// An implementation of a Bicep artifact registry.
    /// </summary>
    public interface IArtifactRegistry
    {
        /// <summary>
        /// Gets the scheme used by this registry in artifact references.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets the capabilities of this registry for the specified artifact reference.
        /// </summary>
        /// <param name="reference">The module reference</param>
        RegistryCapabilities GetCapabilities(ArtifactReference reference);

        /// <summary>
        /// Attempts to parse the specified unqualified reference or returns a failure builder.
        /// </summary>
        /// <param name="aliasName">The alias name</param>
        /// <param name="reference">The unqualified artifact reference</param>
        /// <param name="artifactReference">set to the parsed artifact reference if parsing succeeds</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails</param>
        bool TryParseArtifactReference(
            string? aliasName,
            string reference,
            [NotNullWhen(true)] out ArtifactReference? artifactReference,
            [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Returns true if the specified artifact is already cached in the local cache.
        /// </summary>
        /// <param name="reference">The reference to the artifact.</param>
        bool IsArtifactRestoreRequired(ArtifactReference reference);

        /// <summary>
        /// Returns a URI to the entry point module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="localUri">set to the local entry module entry point URI if parsing succeeds</param>
        /// <param name="failureBuilder">set to an error builder if parsing fails</param>
        /// <returns></returns>
        bool TryGetLocalArtifactEntryPointUri(ArtifactReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Returns true if the specified module exists in the registry.
        /// </summary>
        /// <param name="reference">The reference to the module.</param>
        Task<bool> CheckArtifactExists(ArtifactReference reference);

        /// <summary>
        /// Downloads the specified modules from the registry and caches them locally.
        /// Returns a mapping of module references to error builders for modules that failed to be downloaded.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references);

        /// <summary>
        /// Invalidate the specified cached modules from the registry.
        /// Returns a mapping of module references to error builders for modules that failed to be invalidated.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references);

        /// <summary>
        /// Publishes the module at the specified path to the registry.
        /// </summary>
        /// <param name="moduleReference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        Task PublishArtifact(ArtifactReference moduleReference, Stream compiled, string? documentationUri, string? description);

        /// <summary>
        /// Returns documentationUri for the module.
        /// </summary>
        /// <param name="moduleReference">The module reference</param>
        string? GetDocumentationUri(ArtifactReference moduleReference);

        /// <summary>
        /// Returns description for the module.
        /// </summary>
        /// <param name="moduleReference">The module reference</param>
        Task<string?> TryGetDescription(ArtifactReference moduleReference);
    }
}
