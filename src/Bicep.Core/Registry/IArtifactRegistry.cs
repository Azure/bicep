// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Providers;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

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
        /// <param name="artifactType">The artifact type. Either "module" or "provider"</param>
        ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? aliasName, string reference);

        /// <summary>
        /// Returns true if the specified artifact is already cached in the local cache.
        /// </summary>
        /// <param name="reference">The reference to the artifact.</param>
        bool IsArtifactRestoreRequired(ArtifactReference reference);

        /// <summary>
        /// Returns a URI to the entry point module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <returns></returns>
        ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference);

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
        /// <param name="reference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        /// <param name="bicepSources">The source archive (binary stream of SourceArchive)</param>
        Task PublishModule(ArtifactReference reference, Stream compiled, Stream? bicepSources, string? documentationUri, string? description);

        /// <summary>
        /// Publishes a provider types package to the registry.
        /// </summary>
        /// <param name="reference">The provider reference</param>
        /// <param name="typesTgz">The types.tgz file stream (binary stream of <see cref="TypesV1Archive"/>)</param>
        Task PublishProvider(ArtifactReference reference, Stream typesTgz);

        /// <summary>
        /// Returns documentationUri for the module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        string? GetDocumentationUri(ArtifactReference reference);

        /// <summary>
        /// Returns description for the module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        Task<string?> TryGetDescription(ArtifactReference reference);

        /// <summary>
        /// Returns the source code for the module, if available.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <returns>A source archive</returns>
        ResultWithException<SourceArchive> TryGetSource(ArtifactReference reference);
    }
}
